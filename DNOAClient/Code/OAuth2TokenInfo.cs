using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DNOAClient.Code
{
    [DataContract]
    public class OAuth2TokenInfo
    {
        [DataMember(Name = "audience")]
        public string Audience { get; set; }

        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }

        [DataMember(Name="user")]
        public string User { get; set; }
    }
}