namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ApiClientAgg
{
    // An OAuth2 client-credentials client — a machine caller (e.g. the SwizzChannels
    // connector platform) that authenticates as itself, not as a human user, to call
    // WebApplication1's canonical channels API (see Areas/Channels/Controllers).
    public class ApiClient : Domain.Seedwork.Entity
    {
        public string ClientId { get; set; }

        // PBKDF2 hash (Microsoft.AspNet.Identity.PasswordHasher) — the plaintext secret
        // is only ever returned once, at creation time, and never persisted.
        public string ClientSecretHash { get; set; }

        public string Name { get; set; }

        // Comma-separated scope names this client may request.
        public string Scopes { get; set; }

        public bool IsActive { get; private set; }

        public string ModifiedBy { get; set; }

        public System.DateTime? ModifiedDate { get; set; }

        public void Activate()
        {
            if (!IsActive)
                this.IsActive = true;
        }

        public void Deactivate()
        {
            if (IsActive)
                this.IsActive = false;
        }
    }
}
