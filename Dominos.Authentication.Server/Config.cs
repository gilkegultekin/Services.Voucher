using IdentityServer4.Models;
using System.Collections.Generic;

namespace Dominos.Authentication.Server
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("voucher_api", "Dominos Voucher API")
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("voucher_api", "Dominos Voucher API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            var secret = new Secret("secret".Sha256());

            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        secret
                    },
                    AllowedScopes = { "voucher_api" }
                }
            };
        }
    }
}
