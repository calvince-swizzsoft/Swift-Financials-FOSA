using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.FileRegisterAgg
{
    public static class FileRegisterFactory
    {
        public static FileRegister CreateFileRegister(Guid customerId, int status)
        {
            var fileRegister = new FileRegister();

            fileRegister.GenerateNewIdentity();

            fileRegister.CustomerId = customerId;

            fileRegister.Status = status;

            fileRegister.CreatedDate = DateTime.Now;

            return fileRegister;
        }
    }
}
