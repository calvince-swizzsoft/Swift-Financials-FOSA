using System;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ApiClientAgg
{
    public static class ApiClientFactory
    {
        public static ApiClient CreateApiClient(string clientId, string clientSecretHash, string name, string scopes)
        {
            var apiClient = new ApiClient();

            apiClient.GenerateNewIdentity();

            apiClient.ClientId = clientId;
            apiClient.ClientSecretHash = clientSecretHash;
            apiClient.Name = name;
            apiClient.Scopes = scopes;
            apiClient.CreatedDate = DateTime.Now;
            apiClient.Activate();

            return apiClient;
        }
    }
}
