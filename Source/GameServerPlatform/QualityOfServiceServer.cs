using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Xbox.Services.GameServerPlatform
{
    public class QualityOfServiceServer
    {

        public string TargetLocation
        {
            get;
            private set;
        }

        public string SecureDeviceAddressBase64
        {
            get;
            private set;
        }

        public string ServerFullQualifiedDomainName
        {
            get;
            private set;
        }

    }
}
