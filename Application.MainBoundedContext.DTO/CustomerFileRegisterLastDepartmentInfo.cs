using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.MainBoundedContext.DTO.RegistryModule;

namespace Application.MainBoundedContext.DTO
{
    public class CustomerFileRegisterLastDepartmentInfo
    {
        public FileRegisterDTO FileRegister { get; set; }

        public DepartmentDTO LastDepartment { get; set; }
    }
}
