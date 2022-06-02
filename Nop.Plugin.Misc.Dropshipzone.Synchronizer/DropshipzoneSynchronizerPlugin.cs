using Nop.Core;
using Nop.Core.Domain.Tasks;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Tasks;
using Nop.Web.Framework.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer
{
    /// <summary>
    /// Rename this file and change to the correct type
    /// </summary>
    public class DropshipzoneSynchronizerPlugin : BasePlugin, IMiscPlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IScheduleTaskService _scheduleTaskService;

        public DropshipzoneSynchronizerPlugin(IWebHelper webHelper, ILocalizationService localizationService, ISettingService settingService, IScheduleTaskService scheduleTaskService)
        {
            this._webHelper = webHelper;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._scheduleTaskService = scheduleTaskService;
        }
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Dropshipzone/Configure";
        }

        public override async System.Threading.Tasks.Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new DropshipzoneSettings());

            if (await _scheduleTaskService.GetTaskByTypeAsync(DropshipzoneDefaults.SynchronizationTask) == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask
                {
                    Enabled = true,
                    Seconds = DropshipzoneDefaults.DefaultSynchronizationPeriod * 60 * 60,
                    Name = DropshipzoneDefaults.SynchronizationTaskName,
                    Type = DropshipzoneDefaults.SynchronizationTask,
                });
            }

            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Nop.Plugin.Misc.Dropshipzone.Synchronizer.UserName"] = "Username:",
                ["Plugins.Nop.Plugin.Misc.Dropshipzone.Synchronizer.Password"] = "Password:",
                ["Plugins.Nop.Plugin.Misc.Dropshipzone.Synchronizer.ToEmail"] = "Email To:",
                ["Plugins.Nop.Plugin.Misc.Dropshipzone.Synchronizer.ToName"] = "Name:",
            });

            await base.InstallAsync();
        }

        public override async System.Threading.Tasks.Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<DropshipzoneSettings>();

            var task = await _scheduleTaskService.GetTaskByTypeAsync(DropshipzoneDefaults.SynchronizationTask);
            if (task != null)
                await _scheduleTaskService.DeleteTaskAsync(task);

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Nop.Plugin.Misc.Dropshipzone.Synchronizer");

            await base.UninstallAsync();
        }
    }
}
