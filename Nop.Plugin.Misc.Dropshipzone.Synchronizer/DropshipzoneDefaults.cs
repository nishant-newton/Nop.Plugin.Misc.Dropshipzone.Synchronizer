using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Dropshipzone.Synchronizer
{
    public class DropshipzoneDefaults
    {
        public static string SynchronizationTask => "Nop.Plugin.Misc.Dropshipzone.Synchronizer.Services.SynchronizationTask";

        public static int DefaultSynchronizationPeriod => 12;
        public static string SynchronizationTaskName => "Dropshipzone Synchronization";


    }
}
