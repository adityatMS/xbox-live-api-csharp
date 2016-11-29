using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public enum MultiplayMetrics : int
    {
        Unknown = 0,
        BandwidthUp = 1,
        BandwidthDown = 2,
        Bandwidth = 3,
        Latency = 4,
    }

}
