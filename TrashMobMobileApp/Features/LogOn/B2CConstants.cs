namespace TrashMobMobileApp.Features.LogOn
{
    using Microsoft.Identity.Client;

    public class B2CConstants
    {
        // Azure AD B2C Coordinates
        public string Tenant { get; set; } = "Trashmob.onmicrosoft.com";
        
        public string ClientId { get; set; } = "e8d8517e-095b-4797-bce8-8f6f19d71e3c";
        
        public string PolicySignUpSignIn { get; set; } = "B2C_1_signupsignin1";
        
        public string PolicyEditProfile { get; set; } = "B2C_1_editprofile1";
        
        public string PolicyResetPassword { get; set; } = "B2C_1_passwordreset1";

        public string RedirectUri { get; set; } = "msauth://com.trashmobeco.trashmobmobile/EjFLkitZ%2BGotx7AfSESspVmDL3o%3D";

        public string[] ApiScopesArray { get; set; } = { "https://Trashmob.onmicrosoft.com/api/TrashMob.Writes", "https://Trashmob.onmicrosoft.com/api/TrashMob.Read" };

        public string AzureADB2CHostname { get; set; } = "Trashmob.b2clogin.com";

        public string Authority { get; set; }

        public string AuthoritySignInSignUp { get; set; }
        
        public string AuthorityEditProfile { get; set; }
        
        public string AuthorityPasswordReset { get; set; }

        public IPublicClientApplication PublicClientApp { get; set; }
    }
}