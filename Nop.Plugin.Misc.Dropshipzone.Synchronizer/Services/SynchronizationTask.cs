using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer.Services
{
    public class SynchronizationTask : IScheduleTask
    {
        private readonly DropshipzoneManager _dropshipzoneManager;

        public SynchronizationTask(DropshipzoneManager dropshipzoneManager)
        {
            this._dropshipzoneManager = dropshipzoneManager;
        }
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            await _dropshipzoneManager.SynchronizeAsync();
        }
    }
}
