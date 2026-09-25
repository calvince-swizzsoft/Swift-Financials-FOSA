using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.BackOfficeModule;
using Domain.MainBoundedContext.BackOfficeModule.Aggregates.LoanCaseAgg;
using Domain.MainBoundedContext.AccountsModule.Aggregates.JournalAgg;
using Domain.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;

namespace Application.MainBoundedContext.BackOfficeModule.Services
{
    public sealed class LoanDepositSecurityException : InvalidOperationException
    { public LoanDepositSecurityException(string message) : base(message) {} }
    public sealed class LoanDepositSecurityQuote
    {
        public bool Enabled {get;set;}
        public bool Waived {get;set;}
        public decimal Amount {get;set;}
        public decimal Deposits {get;set;}
        public decimal Committed {get;set;}
        public decimal Available {get;set;}
        public int RequiredGuarantors {get;set;}
        public Guid? AccountId {get;set;}
    }
    public static class OwnDepositSecurityRules
    {
        public static bool Qualifies(decimal amount,decimal deposits,decimal commitments)
        {return amount>0m && decimal.Round(amount,2)==amount && commitments>=0m && amount<Math.Max(0m,deposits-commitments);}
        // Full principal remains reserved until rejection or explicit release after repayment.
        public const string ReservedSql="SELECT COALESCE(SUM(COALESCE(DepositSecurityAmount,0)),0) FROM dbo.swiftFin_LoanCases WHERE CustomerId=@Customer AND Status<>@Rejected AND (@Excluded IS NULL OR Id<>@Excluded)";
        public static void ValidatePostings(IRepository<Journal> repository,IEnumerable<Journal> journals,ServiceHeader h)
        {
            var entries=journals.SelectMany(j=>j.JournalEntries).Where(e=>e.CustomerAccountId.HasValue).GroupBy(e=>e.CustomerAccountId.Value).OrderBy(g=>g.Key);
            foreach(var group in entries)
            {
                var debit=group.Sum(e=>e.Amount);
                if(debit<=0m)continue;
                // Same account lock used by reservation acquisition; kept by the caller's transaction.
                repository.DatabaseSqlQuery<Guid>("SELECT Id FROM dbo.swiftFin_CustomerAccounts WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Account",h,new SqlParameter("@Account",group.Key)).ToList();
                var held=repository.DatabaseSqlQuery<decimal>("SELECT COALESCE(SUM(COALESCE(DepositSecurityAmount,0)),0) FROM dbo.swiftFin_LoanCases WHERE DepositSecurityAccountId=@Account AND Status<>@Rejected",h,new SqlParameter("@Account",group.Key),new SqlParameter("@Rejected",(int)LoanCaseStatus.Rejected)).Single();
                if(held<=0m)continue;
                var balance=repository.DatabaseSqlQuery<decimal>("SELECT COALESCE(-SUM(Amount),0) FROM dbo.swiftFin_JournalEntries WHERE CustomerAccountId=@Account",h,new SqlParameter("@Account",group.Key)).Single();
                if(balance-debit<held)throw new LoanDepositSecurityException("These BOSA deposits secure an active loan. The transaction would spend reserved deposits; repay and release the security first.");
            }
        }
    }
    public partial class LoanCaseAppService
    {
        public LoanDepositSecurityQuote GetOwnDepositSecurity(Guid customerId,Guid productId,decimal amount,ServiceHeader header)
        {
            using(var scope=_dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var product=_loanProductAppService.FindLoanProduct(productId,header);
                if(product==null || product.IsLocked)throw new LoanDepositSecurityException("Select an active loan product.");
                return EvaluateOwnDepositSecurity(customerId,product,amount,header,null);
            }
        }
        private decimal ReservedOwnDeposits(Guid customerId,Guid? excluded,ServiceHeader h)
        {
            return _loanCaseRepository.DatabaseSqlQuery<decimal>(OwnDepositSecurityRules.ReservedSql,h,new SqlParameter("@Customer",customerId),new SqlParameter("@Rejected",(int)LoanCaseStatus.Rejected),new SqlParameter("@Excluded",(object)excluded ?? DBNull.Value)).Single();
        }
        private LoanDepositSecurityQuote EvaluateOwnDepositSecurity(Guid customerId,LoanProductDTO product,decimal amount,ServiceHeader h,Guid? excluded)
        {
            var q=new LoanDepositSecurityQuote{Enabled=product?.WaiveGuarantorsBelowOwnDeposits==true,Amount=amount,RequiredGuarantors=product?.LoanRegistrationMinimumGuarantors ?? 0};
            if(!q.Enabled)return q;
            if(product.LoanRegistrationLoanProductSection!=(int)LoanProductSection.BOSA || product.LoanRegistrationGuarantorSecurityMode!=(int)GuarantorSecurityMode.Investments)
                throw new LoanDepositSecurityException("Own-deposit waiver requires BOSA investment security.");
            var eligible=_loanProductAppService.FindAppraisalProducts(product.Id,h)?.InvestmentProductCollection;
            // A single designated deposit account gives an unambiguous, enforceable reservation.
            if(eligible==null || eligible.Count!=1 || eligible[0].IsLocked)throw new LoanDepositSecurityException("Configure exactly one unlocked BOSA deposit appraisal product for the waiver.");
            var accounts=_customerAccountAppService.FindCustomerAccountsByCustomerId(customerId,h) ?? new List<CustomerAccountDTO>();
            var matching=accounts.Where(a=>a.CustomerAccountTypeTargetProductId==eligible[0].Id && a.CustomerAccountTypeProductCode==(int)ProductCode.Investment && a.Status==(int)CustomerAccountStatus.Normal && a.RecordStatus==(int)RecordStatus.Approved).ToList();
            if(matching.Count!=1)return q;
            var account=matching[0];
            _loanCaseRepository.DatabaseSqlQuery<Guid>("SELECT Id FROM dbo.swiftFin_CustomerAccounts WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Account",h,new SqlParameter("@Account",account.Id)).ToList();
            _customerAccountAppService.FetchCustomerAccountBalances(matching,h);
            q.AccountId=account.Id;q.Deposits=account.BookBalance;
            q.Committed=GuarantorRegistrationRules.CommittedShares(FindLoanGuarantorsByCustomerId(customerId,h) ?? new List<LoanGuarantorDTO>(),excluded)+ReservedOwnDeposits(customerId,excluded,h);
            q.Available=Math.Max(0m,q.Deposits-q.Committed);
            q.Waived=OwnDepositSecurityRules.Qualifies(amount,q.Deposits,q.Committed);
            if(q.Waived)q.RequiredGuarantors=0;
            return q;
        }
        private void ReserveOwnDeposits(LoanCase loan,decimal amount,ServiceHeader h)
        {
            var product=_loanProductAppService.FindLoanProduct(loan.LoanProductId,h);
            if(product==null)throw new LoanDepositSecurityException("Loan product is missing.");
            // Existing opted-in loans retain their security obligation if configuration is later disabled.
            product.WaiveGuarantorsBelowOwnDeposits=loan.WaiveGuarantorsBelowOwnDeposits;
            var q=EvaluateOwnDepositSecurity(loan.CustomerId,product,amount,h,loan.Id);
            if(!q.Waived || !q.AccountId.HasValue)throw new LoanDepositSecurityException("Guarantor waiver is unavailable: the loan must be strictly below uncommitted BOSA deposits. Reassess the amount or register with the required guarantors.");
            if(loan.DepositSecurityAccountId.HasValue && loan.DepositSecurityAccountId!=q.AccountId)throw new LoanDepositSecurityException("The designated deposit account changed. Review the existing security reservation.");
            loan.DepositSecurityAccountId=q.AccountId;loan.DepositSecurityAmount=amount;
        }
        private void ValidateDepositSecurity(LoanCase loan,decimal amount,ServiceHeader h)
        {
            if(loan.WaiveGuarantorsBelowOwnDeposits!=true)return;
            if(loan.DepositSecurityAccountId.HasValue){ReserveOwnDeposits(loan,amount,h);return;}
            var guarantors=FindLoanGuarantorsByLoanCaseId(loan.Id,h) ?? new List<LoanGuarantorDTO>();
            var active=guarantors.Where(g=>g.Status==(int)LoanGuarantorStatus.Attached).ToList();
            if(active.Count<loan.LoanRegistration.MinimumGuarantors || active.Sum(g=>g.AmountGuaranteed)<amount)
                throw new LoanDepositSecurityException("This loan has no own-deposit reservation. The required guarantors must cover the stage amount before proceeding.");
        }
        public bool ReleaseOwnDepositSecurity(Guid loanCaseId,ServiceHeader h)
        {
            using(var scope=_dbContextScopeFactory.CreateWithTransaction(System.Data.IsolationLevel.Serializable))
            {
                var loan=_loanCaseRepository.Get(loanCaseId,h);
                if(loan==null)throw new LoanDepositSecurityException("Loan case not found.");
                if(!loan.DepositSecurityAccountId.HasValue)return true;
                _loanCaseRepository.DatabaseSqlQuery<Guid>("SELECT Id FROM dbo.swiftFin_CustomerAccounts WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Account",h,new SqlParameter("@Account",loan.DepositSecurityAccountId.Value)).ToList();
                if(loan.Status!=(int)LoanCaseStatus.Rejected)
                {
                    if(loan.Status!=(int)LoanCaseStatus.Disbursed)throw new LoanDepositSecurityException("Security remains reserved while the application is active.");
                    var accounts=(_customerAccountAppService.FindCustomerAccountsByCustomerId(loan.CustomerId,h) ?? new List<CustomerAccountDTO>()).Where(a=>a.CustomerAccountTypeTargetProductId==loan.LoanProductId).ToList();
                    _customerAccountAppService.FetchCustomerAccountBalances(accounts,h,true);
                    if(accounts.Count!=1 || accounts.Any(a=>a.PrincipalBalance!=0m || a.InterestBalance!=0m))throw new LoanDepositSecurityException("Repay all principal and interest on the associated loan account before releasing deposits.");
                }
                loan.DepositSecurityAmount=0m;
                return scope.SaveChanges(h)>=0;
            }
        }
    }
}
