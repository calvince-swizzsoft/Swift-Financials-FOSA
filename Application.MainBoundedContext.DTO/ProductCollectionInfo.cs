using Application.MainBoundedContext.DTO.AccountsModule;
using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO
{
    public class ProductCollectionInfo
    {
        public List<InvestmentProductDTO> InvestmentProductCollection { get; set; }

        public List<LoanProductDTO> LoanProductCollection { get; set; }

        public List<SavingsProductDTO> SavingsProductCollection { get; set; }

        public List<LoanProductDTO> EligibileIncomeDeductionLoanProductCollection { get; set; }

        public List<InvestmentProductDTO> EligibileIncomeDeductionInvestmentProductCollection { get; set; }

        public List<SavingsProductDTO> EligibileIncomeDeductionSavingsProductCollection { get; set; }


        // Additional Attribute
        public List<LoanProductDTO> ConcessionLoanProductCollection { get; set; }
    }
}
