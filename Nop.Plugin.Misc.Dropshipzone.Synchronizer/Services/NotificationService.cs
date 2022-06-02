using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.Dropshipzone.Synchronizer.Models;
using Nop.Services.Configuration;
using Nop.Services.Messages;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Services
{
    public class NotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ISettingService _settingService;

        public NotificationService(IEmailSender emailSender, IEmailAccountService emailAccountService, ISettingService settingService)
        {
            this._emailSender = emailSender;
            this._emailAccountService = emailAccountService;
            this._settingService = settingService;
        }
        public async Task SendNotification(List<ProductMessage> productMessages)
        {
            var settings = await _settingService.LoadSettingAsync<DropshipzoneSettings>();

            var message = new StringBuilder();
            var header = "SKU,Old-StockQuantity,New-StockQuantity,Old-RRP,New-RRP,Old-Price,New-Price,Old-ProductCost,New-ProductCost,Old-Publish,New-Publish,Comment";
            message.Append(header + Environment.NewLine);

            foreach (var item in productMessages)
            {
                //if(start == 0)
                var log = $"{item.Sku},{item.OldStockQuantity},{item.NewStockQuantity},{item.OldOldPrice},{item.NewOldPrice},{item.OldPrice},{item.NewPrice},{item.OldProductCost},{item.NewProductCost},{item.OldPublish},{item.NewPublish},{item.Comment}";

                message.AppendLine(log);

            }

            var allEmail = await _emailAccountService.GetAllEmailAccountsAsync();
            var emailId = allEmail.FirstOrDefault();
            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(emailId.Id);

            var tempPath = Path.GetTempPath();
            var csvFileName = Guid.NewGuid().ToString() + ".csv";

            var attachment = Path.Combine(tempPath, csvFileName);

            using (StreamWriter outputFile = new StreamWriter(attachment))
            {
                await outputFile.WriteAsync(message.ToString());
            }
            await _emailSender.SendEmailAsync(emailAccount, "Dropshipzone Synchronization", "Synchronization worked", emailAccount.Email, "Boontree", settings.ToEmail, settings.ToName, attachmentFilePath: attachment, attachmentFileName: csvFileName);

            try
            {
                if(File.Exists(attachment))
                {
                    File.Delete(attachment);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
