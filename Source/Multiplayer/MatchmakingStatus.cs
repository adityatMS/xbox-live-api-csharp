using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Multiplayer
{
    public enum MatchmakingStatus : int
    {
        Unknown = 0,
        None = 1,
        Searching = 2,
        Expired = 3,
        Found = 4,
        Canceled = 5,
    }

}
