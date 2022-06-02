using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.Dropshipzone.Synchronizer.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Controllers
{
    public class DropshipzoneController : BasePaymentController
    {
        private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;

        public DropshipzoneController(IEmailAccountService emailAccountService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILogger logger,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            INotificationService notificationService,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IWorkContext workContext)
        {
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _notificationService = notificationService;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = new ConfigurationModel();
            await PrepareModelAsync(model);

            return View("~/Plugins/Dropshipzone.Synchronizer/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();

            var dszSettings = await _settingService.LoadSettingAsync<DropshipzoneSettings>(storeId);            
            dszSettings.UserName = model.UserName;
            dszSettings.Password = model.Password;
            dszSettings.ToEmail = model.ToEmail;
            dszSettings.ToName = model.ToName;
            await _settingService.SaveSettingAsync(dszSettings, settings => settings.UserName, clearCache: false);
            await _settingService.SaveSettingAsync(dszSettings, settings => settings.Password, clearCache: false);
            await _settingService.SaveSettingAsync(dszSettings, settings => settings.ToEmail, clearCache: false);
            await _settingService.SaveSettingAsync(dszSettings, settings => settings.ToName, clearCache: false);

            await _settingService.ClearCacheAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();

        }

        private async Task PrepareModelAsync(ConfigurationModel model)
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var dszSettings = await _settingService.LoadSettingAsync<DropshipzoneSettings>(storeId);

            if(string.IsNullOrEmpty(dszSettings.UserName))
            {
                return;
            }

            model.Password = dszSettings.Password;
            model.UserName = dszSettings.UserName;
            model.ToEmail = dszSettings.ToEmail;
            model.ToName = dszSettings.ToName;

            if(storeId > 0)
            {                
                model.Password_OverrideForStore = await _settingService.SettingExistsAsync(dszSettings, settings => settings.Password, storeId);
                model.UserName_OverrideForStore = await _settingService.SettingExistsAsync(dszSettings, settings => settings.UserName, storeId);
                model.ToEmail_OverrideForStore = await _settingService.SettingExistsAsync(dszSettings, settings => settings.ToEmail, storeId);
                model.ToName_OverrideForStore = await _settingService.SettingExistsAsync(dszSettings, settings => settings.ToName, storeId);
            }
        }

    }
}
