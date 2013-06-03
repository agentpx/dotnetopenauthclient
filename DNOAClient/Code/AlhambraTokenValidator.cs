using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DNOAClient.Code
{
    public class AlhambraTokenValidator
    {
        public void ValidateToken(OAuth2TokenInfo tokenInfo, string expectedAudience)
        {
            if (string.IsNullOrEmpty(tokenInfo.Audience) || (tokenInfo.Audience != expectedAudience))
            {
                var e = new HttpException("Token with invalid audience");
                throw e;
            }

            if (tokenInfo.ExpiresIn == null) return;

            var expiresIn = tokenInfo.ExpiresIn;

            int intExpiresIn;
            var isInt = int.TryParse(expiresIn, out intExpiresIn);

            //incomplete validation
            if (!isInt || intExpiresIn < 0)
            {
            }
        }
    }
}