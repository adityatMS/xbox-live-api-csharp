using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Social
{
    public class SocialRelationshipChangeSubscription
    {

        public string XboxUserId
        {
            get;
            private set;
        }

        public uint SubscriptionId
        {
            get;
            private set;
        }

        public string ResourceUri
        {
            get;
            private set;
        }

        public Microsoft.Xbox.Services.RealTimeActivity.RealTimeActivitySubscriptionState State
        {
            get;
            private set;
        }

    }
}
