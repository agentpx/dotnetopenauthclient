using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DNOAClient.Code
{
    [DataContract]
    public class OAuth2Graph
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        
        [DataMember(Name = "given_name")]
        public string FirstName { get; set; }

        [DataMember(Name = "family_name")]
        public string LastName { get; set; }

        [DataMember(Name = "name")]
        public string FullName { get; set; }
                
        [DataMember(Name = "email")]
        public string Email { get; set; }
                
        [DataMember(Name = "profile")]
        public string Profile { get; set; }
    }
}