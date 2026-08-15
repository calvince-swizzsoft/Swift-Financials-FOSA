using Domain.Seedwork.Specification;

namespace Domain.MainBoundedContext.AdministrationModule.Aggregates.ApiClientAgg
{
    public static class ApiClientSpecifications
    {
        public static Specification<ApiClient> DefaultSpec()
        {
            Specification<ApiClient> specification = new TrueSpecification<ApiClient>();

            return specification;
        }

        public static ISpecification<ApiClient> ApiClientByClientId(string clientId)
        {
            Specification<ApiClient> specification = new TrueSpecification<ApiClient>();

            specification &= new DirectSpecification<ApiClient>(c => c.ClientId == clientId);

            return specification;
        }
    }
}
