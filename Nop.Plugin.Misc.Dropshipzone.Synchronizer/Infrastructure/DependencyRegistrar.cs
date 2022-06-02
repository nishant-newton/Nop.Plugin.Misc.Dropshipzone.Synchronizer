using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.Dropshipzone.Synchronizer.Services;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="appSettings">App settings</param>
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<DropshipzoneManager>();
            services.AddScoped<DropShipZoneApi>();
            services.AddScoped<NotificationService>();
        }

        public int Order => 1;
    }
}
