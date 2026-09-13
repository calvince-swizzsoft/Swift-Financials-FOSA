using System.Collections.Generic;
using System.Linq;
using Application.MainBoundedContext.DTO.AccountsModule;
namespace Application.MainBoundedContext.AccountsModule.Services
{
    // Catalogue revision is an application identifier, not a claimed SASRA effective version.
    public static class SasraStandardDefinitions
    {
        public const string CatalogueVersion = "CATALOGUE-2026-09-12";
        public static List<SasraVersionDTO> ForProfile(string profile)
        {
            var result = new List<SasraVersionDTO>();
            if(profile=="DT")
            {
                result.Add(Definition(profile,"FORM 1","Capital Adequacy","https://www.sasra.go.ke/download/form-1-capital-adequacy/"));
                result.Add(Definition(profile,"FORM 2","Liquidity Statement","https://www.sasra.go.ke/dts-regulatory-return-forms/"));
                result.Add(Definition(profile,"FORM 2B","Daily Liquidity","https://www.sasra.go.ke/download/form-2b-daily-liquidity/"));
                result.Add(Definition(profile,"FORM 3","Statement of Deposit Return","https://www.sasra.go.ke/download/form3-statement-of-deposit-return/"));
                result.Add(Definition(profile,"FORM 4","Risk Classification of Assets and Provisioning","https://www.sasra.go.ke/dts-regulatory-return-forms/"));
                result.Add(Definition(profile,"FORM 4B","Sectoral Lending","https://www.sasra.go.ke/download-category/regulatory-reporting-forms/page/2/"));
                result.Add(Definition(profile,"FORM 5","Investment Return","https://www.sasra.go.ke/dts-regulatory-return-forms/"));
                result.Add(Definition(profile,"FORM 6","Statement of Financial Position","https://www.sasra.go.ke/download/form-6-statement-of-financial-position/"));
                result.Add(Definition(profile,"FORM 7","Statement of Comprehensive Income","https://www.sasra.go.ke/download/form-7-statement-of-comprehensive-income/"));
                result.Add(Definition(profile,"FORM 9","Insider Lending and Performance Report","https://www.sasra.go.ke/dts-regulatory-return-forms/"));
            }
            else if(profile=="NWDT")
            {
                result.Add(Definition(profile,"FORM 2A","Capital Adequacy","https://www.sasra.go.ke/download/form_2a-capital-adequacy/"));
                result.Add(Definition(profile,"FORM 2B","Form 2B (NW-DT)","https://www.sasra.go.ke/download/form-2b/"));
                result.Add(Definition(profile,"FORM 2C","Form 2C (NW-DT)","https://www.sasra.go.ke/download/form-2c/"));
                result.Add(Definition(profile,"FORM 2D","Form 2D (NW-DT)","https://www.sasra.go.ke/download/form-2d/"));
                result.Add(Definition(profile,"FORM 2E","Form 2E (NW-DT)","https://www.sasra.go.ke/download/form-2e/"));
                result.Add(Definition(profile,"FORM 2F","Form 2F (NW-DT)","https://www.sasra.go.ke/download/form-2f/"));
                result.Add(Definition(profile,"FORM 2G","Form 2G (NW-DT)","https://www.sasra.go.ke/download/form-2g/"));
                result.Add(Definition(profile,"FORM 2H","Form 2H (NW-DT)","https://www.sasra.go.ke/download/form-2h/"));
            }
            if(profile=="DT"||profile=="NWDT")
            {
                var source=profile=="DT"?"https://www.sasra.go.ke/dts-regulatory-return-forms/":"https://www.sasra.go.ke/nw-dts-regulatory-returns-forms/";
                result.Add(Definition(profile,"FORM 11","Common Source of Complaints",source));
                result.Add(Definition(profile,"FORM 12","Complaints Monitoring and Evaluation",source));
            }
            return result;
        }
        static SasraVersionDTO Definition(string profile,string code,string title,string url)
        {
            return new SasraVersionDTO { Profile=profile,ReportCode=code,Title=title,Version=CatalogueVersion,SourceUrl=url,
                Lines=new List<SasraLineDTO>{new SasraLineDTO{Code="TITLE",Description=title,Source="Header",Sign=1}} };
        }
    }
}
