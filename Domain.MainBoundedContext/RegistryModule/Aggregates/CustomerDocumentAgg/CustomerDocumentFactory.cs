using Domain.MainBoundedContext.ValueObjects;
using System;

namespace Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerDocumentAgg
{
    public static class CustomerDocumentFactory
    {
        public static CustomerDocument CreateCustomerDocument(Guid customerId, int type, Collateral collateral, string fileName, string title, string description, string mimeType)
        {
            var customerDocument = new CustomerDocument();

            customerDocument.GenerateNewIdentity();

            customerDocument.CustomerId = customerId;

            customerDocument.Type = type;

            customerDocument.Collateral = collateral;

            customerDocument.FileName = fileName;

            customerDocument.FileTitle = title;

            customerDocument.FileDescription = description;

            customerDocument.FileMIMEType = mimeType;

            customerDocument.CreatedDate = DateTime.Now;

            return customerDocument;
        }
    }
}
