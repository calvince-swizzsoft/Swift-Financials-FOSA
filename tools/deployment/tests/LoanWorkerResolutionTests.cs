using System;
using Application.MainBoundedContext.BackOfficeModule.Services;
using SwiftFinancials.AppServiceContainer;
using Unity;

public class LoanWorkerResolutionTests
{
    public static int Main()
    {
        try
        {
            if (!(Container.Current.Resolve<ILoanAgeingAppService>() is LoanAgeingAppService)) throw new Exception("Loan ageing registration missing.");
            if (!(Container.Current.Resolve<ILoanCaseAppService>() is LoanCaseAppService)) throw new Exception("Loan case resolution failed.");
            if (!(Container.Current.Resolve<ILoanDisbursementBatchAppService>() is LoanDisbursementBatchAppService)) throw new Exception("Loan disbursement resolution failed.");
            Console.WriteLine("PASS: background container resolves loan ageing, loan cases and the full loan disbursement dependency chain. No business operations invoked.");
            return 0;
        }
        catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}
