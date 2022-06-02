using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.Dropshipzone.Synchronizer.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Messages;


namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Services
{
    public class DropshipzoneManager
    {

        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly DropShipZoneApi _dropShipZoneApi;
        private readonly NotificationService _notificationService;

        public DropshipzoneManager
            (
            ISettingService settingService,
            IWorkContext workContext,
            ILogger logger,
            IProductService productService,
            DropShipZoneApi dropShipZoneApi,
            NotificationService notificationService
            )
        {
            this._settingService = settingService;
            this._workContext = workContext;
            this._logger = logger;
            this._productService = productService;
            this._dropShipZoneApi = dropShipZoneApi;
            this._notificationService = notificationService;
        }

        public async Task<IList<(NotifyType Type, string Message)>> SynchronizeAsync(bool synchronizationTask = true, int storeId = 0)
        {
            var messages = new List<(NotifyType, string)>();
            try
            {
                var settings = await _settingService.LoadSettingAsync<DropshipzoneSettings>();
                if(!string.IsNullOrEmpty(settings.UserName))
                {
                    //get all the products from the database
                    //var model = await _productModelFactory.PrepareProductSearchModelAsync(new ProductSearchModel());
                    //var searchModel = new ProductSearchModel();                 

                    var products = await _productService.SearchProductsAsync(showHidden: true);

                    await _logger.InformationAsync($"Dropshipzone synchronition started. Total products to be synched: {products.Count}");

                    var updatedProducts = new List<Product>();

                    var productMessage = new List<ProductMessage>();


                    foreach (var product in products)
                    {
                        var message = new ProductMessage
                        {
                            Sku = product.Sku,
                            OldOldPrice = product.OldPrice,
                            OldPrice = product.Price,
                            OldProductCost = product.ProductCost,
                            OldPublish = product.Published,
                            OldStockQuantity = product.StockQuantity
                        };

                        try
                        {
                            var supplierProduct = await _dropShipZoneApi.GetProductBySkuAsync(product.Sku);
                            if (supplierProduct != null)
                            {
                                var qty = Convert.ToDecimal(supplierProduct.stock_qty);
                                if (qty > 20)
                                    qty = 20;

                                product.OldPrice = Convert.ToDecimal(supplierProduct.RrpPrice);
                                product.ProductCost = Convert.ToDecimal(supplierProduct.price);
                                product.Published = true;
                                product.StockQuantity = Decimal.ToInt32(qty);

                                message.NewOldPrice = product.OldPrice;
                                message.NewProductCost = product.ProductCost;
                                message.NewStockQuantity = product.StockQuantity;

                                if (supplierProduct.discontinued != "No" || product.StockQuantity == 0)
                                {
                                    product.Published = false;
                                }

                                decimal averageShipping = 0;

                                if (supplierProduct.freeshipping == "0") //its not free shipping
                                {
                                    var totalShipping = Convert.ToDecimal(supplierProduct.zone_rates.act)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.nsw_m)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.nsw_r)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.nt_m)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.nt_r)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.qld_m)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.qld_r)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.remote)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.sa_m)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.sa_r)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.tas_m)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.tas_r)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.vic_m)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.vic_r)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.wa_m)
                                                        + Convert.ToDecimal(supplierProduct.zone_rates.wa_r);

                                    averageShipping = Convert.ToDecimal(totalShipping / 16);
                                }

                                decimal sellingPrice = product.ProductCost + (product.ProductCost * 0.1m) + 10 + averageShipping;
                                product.Price = sellingPrice;

                                message.NewPrice = product.Price;
                                message.NewPublish = product.Published;

                                

                                message.Comment = "API OK ||";

                                updatedProducts.Add(product);
                                productMessage.Add(message);
                            }
                            else
                            {
                                messages.Add((NotifyType.Warning, $"Dropshipzone synchronization warning. No product found for SKU {product.Sku}"));

                            }

                        }
                        catch (Exception exception)
                        {
                            //await _logger.ErrorAsync($"Dropshipzone API error: {exception.Message}. Product SKU {product.Sku}", exception, await _workContext.GetCurrentCustomerAsync());

                            messages.Add((NotifyType.Error, $"Dropshipzone API synchronization error: {exception.Message} for SKU {product.Sku}"));
                        }
                    }

                    foreach(var product in updatedProducts)
                    {
                        try
                        {
                            await _productService.UpdateProductAsync(product);
                        }
                        catch (Exception ex)
                        {
                            //await _logger.ErrorAsync($"Dropshipzone DB update error: {ex.Message}.", ex, await _workContext.GetCurrentCustomerAsync());
                            if (productMessage.Any(x => x.Sku == product.Sku))
                            {
                                productMessage.Where(x => x.Sku == product.Sku).Select(y =>
                                {
                                    y.Comment += $"Database error {ex.Message}";
                                    return y;
                                });
                            }
                            messages.Add((NotifyType.Error, $"Dropshipzone Product update synchronization error: {ex.Message} for SKU {product.Sku}"));
                        }
                    }

                    _dropShipZoneApi.Dispose();

                    var okMessage = $"Dropshipzone ran succesfully with email sent";

                    try
                    {
                        await _notificationService.SendNotification(productMessage);
                    }
                    catch (Exception ex)
                    {

                        await _logger.ErrorAsync($"Dropshipzone Error in email sending: {ex.Message}.", ex, await _workContext.GetCurrentCustomerAsync());
                        okMessage += " resulted in error";
                    }

                    messages.Add((NotifyType.Success, $"{okMessage}. Total updated products {updatedProducts.Count}"));
                    foreach(var message in messages)
                    {
                        var item1 = message.Item1;
                        var content = message.Item2;
                        switch (item1)
                        {
                            case NotifyType.Error:
                                await _logger.ErrorAsync(content);
                                break;
                            case NotifyType.Success:
                                await _logger.InformationAsync(content);
                                break;
                            case NotifyType.Warning:
                                await _logger.WarningAsync(content);
                                break;
                            default:
                                await _logger.InformationAsync(content);

                                break;
                        }
                    }                    
                }
            }
            catch (Exception exception)
            {

                await _logger.ErrorAsync($"Dropshipzone synchronization error: {exception.Message}.", exception, await _workContext.GetCurrentCustomerAsync());
                messages.Add((NotifyType.Error, $"Dropshipzone synchronization error: {exception.Message}"));
            }
            return messages;
        }
    }
}
