using Domain.Seedwork;

namespace Domain.MainBoundedContext.Aggregates.EnumerationAgg
{
    public class Enumeration : Entity
    {
        public string Key { get; set; }

        public int Value { get; set; }

        public string Description { get; set; }
    }
}
