using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.Social
{
    public class SocialRelationshipChangeEventArgs : EventArgs
    {

        public IList<string> XboxUserIds
        {
            get;
            private set;
        }

        public SocialNotificationType SocialNotification
        {
            get;
            private set;
        }

        public string CallerXboxUserId
        {
            get;
            private set;
        }

    }
}
