using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClothesEcommerce
{
    public class PaypalConfiguration
    {
      
        //Variables for storing the clientID and clientSecret key
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        //Constructor
        static PaypalConfiguration()
        {
            var config = GetConfig();
            ClientId = "AYGUqDa1UYcY-renWVOKNF1Uoqqn9Tnh6-jOUZVoTJrM4i-ZbyJCcJqzWjN4ZCf1jzpQtqwLjZqHP6RI";
            ClientSecret = "ELsT4xJzYuApZMCLDnhGZTMJ3o9kRAqsOl8Ysvl0sZSoWo3t5xKAc9O8N4i4qfOH1_J2IMPtAXL5-EM-";
        }
        // getting properties from the web.config
        public static Dictionary<string, string> GetConfig()
                {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }
        private static string GetAccessToken()
        {
            // getting accesstocken from paypal               
            string accessToken = new OAuthTokenCredential
                    (ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }
        public static APIContext GetAPIContext()
        {
            // return apicontext object by invoking it with the accesstoken
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}
