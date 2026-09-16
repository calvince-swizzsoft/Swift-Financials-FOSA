using System;
using System.Collections.Generic;
using Application.MainBoundedContext.DTO.AccountsModule;
using Infrastructure.Crosscutting.Framework.Utils;
namespace Application.MainBoundedContext.AccountsModule.Services
{
    public interface ISasraSetupAppService
    {
        SasraVersionDTO GetForm3Definition(ServiceHeader header);
        SasraForm3Result PreviewForm3(SasraForm3Request input,ServiceHeader header);
        SasraVersionDTO GetForm5Definition(ServiceHeader header);
        SasraForm5Result PreviewForm5(SasraForm5Request input,ServiceHeader header);
        SasraVersionDTO GetForm2Definition(ServiceHeader header);
        SasraForm2Result PreviewForm2(SasraForm2Request input,ServiceHeader header);
        SasraVersionDTO GetForm1Definition(ServiceHeader header);
        SasraForm1Result PreviewForm1(SasraForm1Request input,ServiceHeader header);
        SasraVersionDTO GetForm7Definition(ServiceHeader header);
        SasraForm7Result PreviewForm7(SasraForm7Request input, ServiceHeader header);
        SasraVersionDTO GetForm6Definition(ServiceHeader header);
        SasraForm6Result PreviewForm6(SasraForm6Request input, ServiceHeader header);
        List<SasraVersionDTO> GetStandardDefinitions(string profile);
        int AddStandardDefinitions(ServiceHeader header);
        SasraProfileDTO GetProfile(ServiceHeader header);
        SasraProfileDTO SaveProfile(SasraProfileDTO input, ServiceHeader header);
        SasraVersionPageDTO GetVersions(int pageIndex,int pageSize,ServiceHeader header);
        SasraVersionDTO GetVersion(Guid id,ServiceHeader header);
        SasraVersionDTO SaveVersion(SasraVersionDTO input,ServiceHeader header);
    }
    public class SasraSetupException : Exception
    {
        public string Field { get; private set; }
        public int Status { get; private set; }
        public SasraSetupException(string field,string message,int status=400):base(message){Field=field;Status=status;}
    }
}
