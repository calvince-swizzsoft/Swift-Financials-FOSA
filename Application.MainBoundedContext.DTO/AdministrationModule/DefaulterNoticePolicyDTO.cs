using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.AdministrationModule
{
 public class DefaulterNoticeStageDTO
 {
  public string NoticeType{get;set;} public int DaysOverdue{get;set;} public decimal MinimumArrears{get;set;} public string Recipient{get;set;}
  public int ResponseDays{get;set;} public string Channel{get;set;} public bool RequireApproval{get;set;}=true; public string Template{get;set;}
 }
 public class DefaulterNoticePolicyDTO {public bool Enabled{get;set;} public List<DefaulterNoticeStageDTO> Stages{get;set;}=new List<DefaulterNoticeStageDTO>();}
 public class ResolvedDefaulterNoticePolicyDTO
 {
  public Guid LoanCaseId{get;set;} public Guid BranchId{get;set;} public Guid? CompanyId{get;set;} public string CompanyName{get;set;} public int Revision{get;set;}
  public string PolicyJson{get;set;} public DefaulterNoticePolicyDTO Policy{get;set;}
 }
}
