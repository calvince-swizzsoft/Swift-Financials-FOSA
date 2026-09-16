using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.BackOfficeModule
{
 public class LoanRiskReviewDTO
 {
  public Guid CustomerAccountId{get;set;} public DateTime AsAt{get;set;} public int Revision{get;set;} public int RiskCategory{get;set;}
  public decimal? ProvisioningAdjustment{get;set;} public string Evidence{get;set;} public string BasisHash{get;set;}
 }
 public class SasraForm4Request {public DateTime YearStart{get;set;} public DateTime AsAt{get;set;} public string InstitutionName{get;set;} public string RegistrationNumber{get;set;} }
 public class SasraForm4Account
 {
  public decimal InterestReceivable{get;set;} public bool IsSettled{get;set;} public Guid CustomerAccountId{get;set;} public string Cases{get;set;} public decimal Principal{get;set;} public int? DaysPastDue{get;set;} public int? MinimumCategory{get;set;} public int? Category{get;set;} public bool IsRestructured{get;set;} public LoanRiskReviewDTO Review{get;set;} public string BasisHash{get;set;} public List<string> Issues{get;set;}=new List<string>();
 }
 public class SasraForm4Row {public bool IsRestructured{get;set;} public int Category{get;set;} public string Name{get;set;} public int Accounts{get;set;} public decimal Principal{get;set;} public decimal Adjustment{get;set;} public decimal Exposure{get;set;} public decimal Rate{get;set;} public decimal Provision{get;set;} }
 public class SasraForm4Result
 {
  public bool IsWorkingCopy{get;set;}=true; public decimal UnclassifiedPrincipal{get;set;}
  public string Version{get;set;} public DateTime AsAt{get;set;} public decimal LedgerPrincipal{get;set;} public decimal PrincipalDifference{get;set;} public decimal TotalExposure{get;set;} public decimal TotalProvision{get;set;} public int UnresolvedAccounts{get;set;} public string WorkbookBase64{get;set;}
  public List<SasraForm4Row> Rows{get;set;}=new List<SasraForm4Row>();public List<SasraForm4Account> Accounts{get;set;}=new List<SasraForm4Account>();public List<string> Issues{get;set;}=new List<string>();
 }
}
