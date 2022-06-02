using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Misc.Dropshipzone.Synchronizer.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Services
{
    public class DropShipZoneApi : IDisposable
    {
        private readonly ISettingService _settingService;

        private const string BASE_ADDRESS = "https://api.dropshipzone.com.au";
        private readonly string _authAddress = BASE_ADDRESS + "/auth";
        private readonly string _productAddress = BASE_ADDRESS + "/products";
        private readonly ILogger _logger;


        private HttpClient _client;

        private string _apiToken;


        public DropShipZoneApi(ISettingService settingService, ILogger logger)
        {
            this._settingService = settingService;
            this._logger = logger;
            _client = new HttpClient();
        }          

        private async Task<string> GetAuthToken()
        {
            var settings = await _settingService.LoadSettingAsync<DropshipzoneSettings>();

            dynamic authBody = new JObject();
            authBody.email = settings.UserName;
            authBody.password = settings.Password;

            var body = new StringContent(authBody.ToString(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(new Uri(_authAddress), body);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(content);
            _apiToken = authResponse.token;
            await _logger.InformationAsync("Dropshipzone api token acquired");
            return _apiToken;
        }

        public async Task<DszProduct> GetProductBySkuAsync(string sku)
        {
            var counter = 0;
            var delay = 250;
            while(counter <= 3)
            {
                try
                {
                    await Task.Delay(delay);
                    var token = _apiToken;

                    if (string.IsNullOrEmpty(token))
                        token = await GetAuthToken();
                    var uri = new Uri(_productAddress + $"/{sku}");
                    var authHeader = $"jwt {token}";
                    var httpRequestMessage = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = uri,
                        Headers = {
                    { HttpRequestHeader.Authorization.ToString(), authHeader },
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                    { "X-Version", "1" }
                    }
                    };

                    var response = await _client.SendAsync(httpRequestMessage);
                    if(response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        token = string.Empty;
                        counter++;
                    }
                    else if(response.StatusCode == HttpStatusCode.TooManyRequests)
                    {
                        delay = counter > 0 ?  delay * counter : delay;
                      
                        await Task.Delay(delay);
                        counter++;
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                        counter = 10;
                        var content = await response.Content.ReadAsStringAsync();
                        var product = JsonConvert.DeserializeObject<DszProduct[]>(content);
                        if(product != null)
                            return product[0];                        
                    }
                    
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return null;            

        }       

        public void Dispose()
        {
            ((IDisposable)_client).Dispose();
        }
    }
}
