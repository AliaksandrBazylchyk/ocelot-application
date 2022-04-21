using Duende.IdentityServer.Models;

namespace ocelot_identityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope(name: "API", displayName: "Ocelot API")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // TODO Hide key into docker env variables
                ClientSecrets = {new Secret("secret_key".Sha256())},
                AllowedScopes = {"API"}
            }
        };
}