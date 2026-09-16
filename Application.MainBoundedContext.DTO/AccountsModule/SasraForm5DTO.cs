using System;
using System.Collections.Generic;
namespace Application.MainBoundedContext.DTO.AccountsModule
{
 public class SasraForm5Request:SasraForm6Request {public string ReportPurpose{get;set;}="Quarterly";public Guid Form1VersionId{get;set;}public Guid Form6VersionId{get;set;}}
 public class SasraForm5Result:SasraForm6Result {public string ReportPurpose{get;set;}public Guid Form1VersionId{get;set;}public int Form1Revision{get;set;}public Guid Form6VersionId{get;set;}public int Form6Revision{get;set;}public List<string> Warnings{get;set;}=new List<string>();}
}
