using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.AccountsModule
{
    public class InsiderAppointmentDTO
    {
        public Guid Id {get;set;} public int Revision {get;set;} public Guid CustomerId {get;set;}
        public string CustomerName {get;set;} public string MemberNumber {get;set;}
        public string Kind {get;set;} public string Position {get;set;}
        public DateTime StartsAt {get;set;} public DateTime? EndsAt {get;set;}
        public string Evidence {get;set;} public bool IsVoided {get;set;}
    }
    public class InsiderDecisionDTO
    {
        public Guid LoanCaseId {get;set;} public int Revision {get;set;}
        public string Decision {get;set;} public DateTime DecisionDate {get;set;}
        public string MinuteReference {get;set;} public string Evidence {get;set;}
        public bool ApplicantAbsent {get;set;} public string SecurityDescription {get;set;}
        public bool SecurityConfirmed {get;set;} public string Remarks {get;set;}
    }
    public class Form9PolicyDTO
    {
        public int Revision {get;set;} public string GrantBasis {get;set;}
        public string PopulationBasis {get;set;} public string OutstandingBasis {get;set;}
        public string DepositBasis {get;set;} public string SectionBBasis {get;set;}
        public string SharedAllocation {get;set;} public string NilReturn {get;set;}
        public bool RulesConfirmed {get;set;} public string Evidence {get;set;}
        public List<Guid> BosaProductIds {get;set;}=new List<Guid>();
    }
    public class Form9Request { public DateTime Month {get;set;} }
    public class Form9Source
    {
        public Guid LoanCaseId {get;set;} public Guid CustomerId {get;set;} public int CaseNumber {get;set;}
        public string Borrower {get;set;} public string MemberNumber {get;set;} public string Product {get;set;}
        public decimal AmountApplied {get;set;} public decimal ApprovedAmount {get;set;}
        public DateTime? ApprovedDate {get;set;} public DateTime? DisbursedDate {get;set;}
        public DateTime CreatedDate {get;set;} public int TermMonths {get;set;} public int Status {get;set;}
    }
    public class Form9Row
    {
        public Guid LoanCaseId {get;set;} public Guid CustomerId {get;set;} public int CaseNumber {get;set;}
        public string Borrower {get;set;} public string MemberNumber {get;set;} public string Position {get;set;} public string Product {get;set;}
        public decimal Applied {get;set;} public decimal Granted {get;set;} public DateTime? DecisionDate {get;set;}
        public decimal? BosaDeposits {get;set;} public string Security {get;set;} public DateTime? FirstDueDate {get;set;}
        public int TermMonths {get;set;} public string Remarks {get;set;} public decimal? Outstanding {get;set;}
        public string Performance {get;set;} public List<string> Issues {get;set;}=new List<string>();
    }
    public class Form9Result
    {
        public Guid Id {get;set;} public DateTime Month {get;set;} public DateTime AsAt {get;set;}
        public DateTime CreatedAt {get;set;} public string CreatedBy {get;set;}
        public string InstitutionName {get;set;} public string RegistrationNumber {get;set;}
        public string TemplateHash {get;set;} public string SnapshotHash {get;set;}
        public Form9PolicyDTO Policy {get;set;} public List<Form9Row> NewLoans {get;set;}=new List<Form9Row>();
        public List<Form9Row> OutstandingLoans {get;set;}=new List<Form9Row>();
        public List<string> Issues {get;set;}=new List<string>();
        public List<string> Warnings {get;set;}=new List<string>();
        public decimal TotalGranted {get;set;} public decimal KnownOutstanding {get;set;}
        public bool IsComplete {get;set;} public string Status {get;set;}="Working draft";
        public string SourceEvidenceJson {get;set;}
    }
    public class Form9Page<T> {public int Total {get;set;} public List<T> Items {get;set;}=new List<T>();}
    public class Form9Candidate
    {
        public Guid CustomerId {get;set;} public string Name {get;set;} public string MemberNumber {get;set;}
        public string Kind {get;set;} public string Position {get;set;} public DateTime? SuggestedStart {get;set;}
        public bool IsLocked {get;set;} public bool HasHistory {get;set;}
    }
    public class Form9Product { public Guid Id {get;set;} public string Description {get;set;} public string Kind {get;set;} public bool IsLocked {get;set;} }
    public class Form9Revision {public int Revision {get;set;} public DateTime CreatedDate {get;set;} public string CreatedBy {get;set;} public string Payload {get;set;}}
    public class Form9Export {public string FileName {get;set;} public string WorkbookBase64 {get;set;} public string Sha256 {get;set;}}
}
