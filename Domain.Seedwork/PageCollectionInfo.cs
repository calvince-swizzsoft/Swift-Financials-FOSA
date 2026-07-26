using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO
{
    public class PageCollectionInfo<T> where T : class
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public List<T> PageCollection { get; set; }

        public int ItemsCount { get; set; }

        public decimal BookBalanceBroughtFoward { get; set; }

        public decimal BookBalanceCarriedForward { get; set; }

        public decimal AvailableBalanceBroughtFoward { get; set; }

        public decimal AvailableBalanceCarriedForward { get; set; }

        public decimal TotalDebits { get; set; }

        public decimal TotalCredits { get; set; }

        public decimal TotalApportioned { get; set; }

        public decimal TotalShortage { get; set; }
        public object TotalCount { get; set; }
        public object TotalPages { get; set; }
    }
}