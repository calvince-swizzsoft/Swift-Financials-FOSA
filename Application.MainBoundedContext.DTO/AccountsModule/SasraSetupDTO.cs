using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class SasraForm6Request
    {
        public Guid VersionId { get; set; }
        public DateTime YearStart { get; set; }
        public DateTime AsAt { get; set; }
    }
    public class SasraForm6Balance
    {
        public Guid Id { get; set; }
        public int AccountType { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal BeforeYear { get; set; }
    }
    public class SasraForm6Row
    {
        public string Description { get; set; }
        public string Cell { get; set; }
        public string Source { get; set; }
        public string Formula { get; set; }
        public bool IsRatio { get; set; }
        public decimal? Amount { get; set; }
    }
    public class SasraForm6Result
    {
        public Guid VersionId { get; set; }
        public DateTime YearStart { get; set; }
        public DateTime AsAt { get; set; }
        public DateTime GeneratedAtUtc { get; set; }
        public string InstitutionName { get; set; }
        public decimal Difference { get; set; }
        public decimal LedgerDifference { get; set; }
        public List<SasraForm6Row> Rows { get; set; } = new List<SasraForm6Row>();
        public List<SasraForm6Balance> UnmappedAccounts { get; set; } = new List<SasraForm6Balance>();
        public List<string> Issues { get; set; } = new List<string>();
        public string WorkbookBase64 { get; set; }
    }
    public class SasraForm7Request
    {
        public Guid VersionId { get; set; }
        public DateTime YearStart { get; set; }
        public DateTime AsAt { get; set; }
    }
    public class SasraForm7Balance
    {
        public Guid Id { get; set; }
        public int AccountType { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal ClosingBalance { get; set; }
    }
    public class SasraForm7Row
    {
        public string Description { get; set; }
        public string Cell { get; set; }
        public string Source { get; set; }
        public string Formula { get; set; }
        public decimal? Amount { get; set; }
    }
    public class SasraForm7Result
    {
        public Guid VersionId { get; set; }
        public DateTime YearStart { get; set; }
        public DateTime AsAt { get; set; }
        public DateTime GeneratedAtUtc { get; set; }
        public string InstitutionName { get; set; }
        public decimal Difference { get; set; }
        public decimal LedgerDifference { get; set; }
        public decimal LedgerNetIncome { get; set; }
        public decimal UnclosedSurplus { get; set; }
        public decimal ClosingAdjustment { get; set; }
        public List<SasraForm7Row> Rows { get; set; } = new List<SasraForm7Row>();
        public List<SasraForm7Balance> UnmappedAccounts { get; set; } = new List<SasraForm7Balance>();
        public List<string> Issues { get; set; } = new List<string>();
        public string WorkbookBase64 { get; set; }
    }
    public class SasraForm1Request : SasraForm6Request
    {
        public bool MappingBased { get; set; }
        public Guid Form6VersionId { get; set; }
        public decimal? SurplusAdjustment { get; set; }
        public decimal? InvestmentDeduction { get; set; }
        public decimal? OtherDeductions { get; set; }
        public decimal? OffBalanceSheetAssets { get; set; }
        public bool CapitalEligibilityReviewed { get; set; }
        public string ReviewNotes { get; set; }
    }
    public class SasraForm1Balance : SasraForm6Balance
    {
        public decimal CurrentClosingBalance { get; set; }
    }
    public class SasraForm1Result : SasraForm6Result
    {
        public Guid Form6VersionId { get; set; }
        public int Form6Revision { get; set; }
        public decimal RawCurrentSurplus { get; set; }
        public decimal AdjustedSurplus { get; set; }
        public decimal EligibleCurrentSurplus { get; set; }
        public SasraForm1Request Inputs { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }
    public class SasraProfileDTO
    {
        public string Profile { get; set; }
        public string InstitutionName { get; set; }
        public string RegistrationNumber { get; set; }
        public int Revision { get; set; }
    }
    public class SasraVersionPageDTO
    {
        public List<SasraVersionDTO> Items { get; set; }
        public int Total { get; set; }
    }
    public class SasraVersionDTO
    {
        public Guid Id { get; set; }
        public string Profile { get; set; }
        public string ReportCode { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public int Revision { get; set; }
        public string SourceUrl { get; set; }
        public string WorkbookSha256 { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public string Status { get { return "Draft"; } }
        public List<SasraLineDTO> Lines { get; set; } = new List<SasraLineDTO>();
    }
    public class SasraLineDTO
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Sheet { get; set; }
        public string Cell { get; set; }
        public int Sign { get; set; } = 1;
        public Dictionary<Guid,string> AccountNames { get; set; } = new Dictionary<Guid,string>();
        public List<Guid> AccountIds { get; set; } = new List<Guid>();
    }
}

namespace Application.MainBoundedContext.DTO.AccountsModule
{
 public class SasraForm2Request : SasraForm6Request
 {
  public bool MappingBased { get; set; }
  public System.Guid Form6VersionId { get; set; }
  public System.Collections.Generic.Dictionary<string,decimal?> ManualAmounts { get; set; } = new System.Collections.Generic.Dictionary<string,decimal?>();
  public System.Collections.Generic.Dictionary<string,decimal?> Exclusions { get; set; } = new System.Collections.Generic.Dictionary<string,decimal?>();
  public bool LiquidityReviewed { get; set; }
  public string ReviewNotes { get; set; }
 }
 public class SasraForm2Result : SasraForm6Result
 {
  public System.Guid Form6VersionId { get; set; }
  public int Form6Revision { get; set; }
  public decimal BankOverdrafts { get; set; }
  public SasraForm2Request Inputs { get; set; }
  public System.Collections.Generic.List<string> Warnings { get; set; } = new System.Collections.Generic.List<string>();
 }
}

namespace Application.MainBoundedContext.DTO.AccountsModule
{
 public class SasraForm3Request : SasraForm6Request { public System.Guid Form6VersionId { get; set; } }
 public class SasraForm3AccountBalance
 {
  public System.Guid ChartOfAccountId { get; set; }
  public System.Guid? CustomerAccountId { get; set; }
  public bool HasCustomerAccount { get; set; }
  public decimal Balance { get; set; }
 }
 public class SasraForm3Row
 {
  public string Range { get; set; }
  public string DepositType { get; set; }
  public string Category { get; set; }
  public int Band { get; set; }
  public string Cell { get; set; }
  public string CountCell { get; set; }
  public int AccountCount { get; set; }
  public decimal Amount { get; set; }
 }
 public class SasraForm3Result : SasraForm6Result
 {
  public System.Guid Form6VersionId { get; set; }
  public int Form6Revision { get; set; }
  public decimal Form6Difference { get; set; }
  public int TotalAccounts { get; set; }
  public decimal TotalDeposits { get; set; }
  public decimal MappedLedgerDeposits { get; set; }
  public decimal UnallocatedBalance { get; set; }
  public int UnallocatedGroups { get; set; }
  public int NegativeBalanceAccounts { get; set; }
  public decimal NegativeBalance { get; set; }
  public int ZeroBalanceAccounts { get; set; }
  public System.Collections.Generic.List<SasraForm3Row> DepositRows { get; set; } = new System.Collections.Generic.List<SasraForm3Row>();
  public System.Collections.Generic.List<string> Warnings { get; set; } = new System.Collections.Generic.List<string>();
 }
}
