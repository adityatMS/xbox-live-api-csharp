using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public class MultiplayerPeerToHostRequirements
    {

        public MultiplayMetrics HostSelectionMetric
        {
            get;
            private set;
        }

        public ulong BandwidthUpMinimumInKilobitsPerSecond
        {
            get;
            private set;
        }

        public ulong BandwidthDownMinimumInKilobitsPerSecond
        {
            get;
            private set;
        }

        public TimeSpan LatencyMaximum
        {
            get;
            private set;
        }

    }
}
