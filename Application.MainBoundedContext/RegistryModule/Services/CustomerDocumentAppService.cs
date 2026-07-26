using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.RegistryModule.Aggregates.CustomerDocumentAgg;
using Domain.MainBoundedContext.ValueObjects;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using Infrastructure.Crosscutting.Framework.Adapter;
using System.Threading.Tasks;
using System.Linq;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class CustomerDocumentAppService : ICustomerDocumentAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<CustomerDocument> _customerDocumentRepository;

        public CustomerDocumentAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<CustomerDocument> customerDocumentRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (customerDocumentRepository == null)
                throw new ArgumentNullException(nameof(customerDocumentRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _customerDocumentRepository = customerDocumentRepository;
        }

        public CustomerDocumentDTO AddNewCustomerDocument(CustomerDocumentDTO customerDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader)
        {
            if (customerDocumentDTO == null || string.IsNullOrWhiteSpace(fileUploadDirectory) || string.IsNullOrWhiteSpace(customerDocumentDTO.FileName))
                return null;

            var path = Path.Combine(fileUploadDirectory, customerDocumentDTO.FileName);

            if (System.IO.File.Exists(path))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    customerDocumentDTO.FileMIMEType = System.Web.MimeMapping.GetMimeMapping(path);

                    var collateral = new Collateral(customerDocumentDTO.CollateralValue, customerDocumentDTO.CollateralAdvanceRate, customerDocumentDTO.CollateralStatus);

                    var customerDocument = CustomerDocumentFactory.CreateCustomerDocument(customerDocumentDTO.CustomerId, customerDocumentDTO.Type, collateral, customerDocumentDTO.FileName, customerDocumentDTO.FileTitle, customerDocumentDTO.FileDescription, customerDocumentDTO.FileMIMEType);

                    customerDocument.CreatedBy = serviceHeader.ApplicationUserName;

                    _customerDocumentRepository.Add(customerDocument, serviceHeader);

                    dbContextScope.SaveChanges(serviceHeader);

                    return customerDocument.ProjectedAs<CustomerDocumentDTO>();
                }
            }
            else return null;
        }

        public async Task<CustomerDocumentDTO> AddNewCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader)
        {
            var customerDocumentBindingModel = customerDocumentDTO.ProjectedAs<CustomerDocumentBindingModel>();

            customerDocumentBindingModel.ValidateAll();

            if (customerDocumentBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, customerDocumentBindingModel.ErrorMessages));

            var path = Path.Combine(fileUploadDirectory, customerDocumentDTO.FileName);

            if (System.IO.File.Exists(path))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    customerDocumentDTO.FileMIMEType = System.Web.MimeMapping.GetMimeMapping(path);

                    var collateral = new Collateral(customerDocumentDTO.CollateralValue, customerDocumentDTO.CollateralAdvanceRate, customerDocumentDTO.CollateralStatus);

                    var customerDocument = CustomerDocumentFactory.CreateCustomerDocument(customerDocumentDTO.CustomerId, customerDocumentDTO.Type, collateral, customerDocumentDTO.FileName, customerDocumentDTO.FileTitle, customerDocumentDTO.FileDescription, customerDocumentDTO.FileMIMEType);

                    customerDocument.CreatedBy = serviceHeader.ApplicationUserName;

                    _customerDocumentRepository.Add(customerDocument, serviceHeader);

                    return await dbContextScope.SaveChangesAsync(serviceHeader) > 0 ? customerDocument.ProjectedAs<CustomerDocumentDTO>() : null;
                }
            }
            else throw new InvalidOperationException(string.Format("Sorry, the document with name '{0}' does not exists in the specified directory!", customerDocumentDTO.FileName));
        }

        public bool UpdateCustomerDocument(CustomerDocumentDTO customerDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader)
        {
            if (customerDocumentDTO == null || string.IsNullOrWhiteSpace(fileUploadDirectory) || string.IsNullOrWhiteSpace(customerDocumentDTO.FileName))
                return false;

            var path = Path.Combine(fileUploadDirectory, customerDocumentDTO.FileName);

            if (System.IO.File.Exists(path))
            {
                customerDocumentDTO.FileMIMEType = System.Web.MimeMapping.GetMimeMapping(path);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _customerDocumentRepository.Get(customerDocumentDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        var collateral = new Collateral(customerDocumentDTO.CollateralValue, customerDocumentDTO.CollateralAdvanceRate, customerDocumentDTO.CollateralStatus);

                        collateral = persisted.Collateral.Status == (int)CollateralStatus.Attached ? persisted.Collateral : collateral;

                        var current = CustomerDocumentFactory.CreateCustomerDocument(customerDocumentDTO.CustomerId, customerDocumentDTO.Type, collateral, customerDocumentDTO.FileName, customerDocumentDTO.FileTitle, customerDocumentDTO.FileDescription, customerDocumentDTO.FileMIMEType);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                        current.CreatedBy = persisted.CreatedBy;


                        current.RecordStatus = (byte)customerDocumentDTO.RecordStatus;
                        current.ModifiedBy = serviceHeader.ApplicationUserName;
                        current.ModifiedDate = DateTime.Now;

                        _customerDocumentRepository.Merge(persisted, current, serviceHeader);

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }

            else throw new InvalidOperationException(string.Format("Sorry, the document with name '{0}' no longer exists in the specified directory!", customerDocumentDTO.FileName));
        }

        public async Task<bool> UpdateCustomerDocumentAsync(CustomerDocumentDTO customerDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader)
        {
            var customerDocumentBindingModel = customerDocumentDTO.ProjectedAs<CustomerDocumentBindingModel>();

            customerDocumentBindingModel.ValidateAll();

            if (customerDocumentBindingModel.HasErrors) throw new InvalidOperationException(string.Join(Environment.NewLine, customerDocumentBindingModel.ErrorMessages));

            var path = Path.Combine(fileUploadDirectory, customerDocumentDTO.FileName);

            if (System.IO.File.Exists(path))
            {
                customerDocumentDTO.FileMIMEType = System.Web.MimeMapping.GetMimeMapping(path);

                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = await _customerDocumentRepository.GetAsync(customerDocumentDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        var collateral = new Collateral(customerDocumentDTO.CollateralValue, customerDocumentDTO.CollateralAdvanceRate, customerDocumentDTO.CollateralStatus);

                        collateral = persisted.Collateral.Status == (int)CollateralStatus.Attached ? persisted.Collateral : collateral;

                        var current = CustomerDocumentFactory.CreateCustomerDocument(customerDocumentDTO.CustomerId, customerDocumentDTO.Type, collateral, customerDocumentDTO.FileName, customerDocumentDTO.FileTitle, customerDocumentDTO.FileDescription, customerDocumentDTO.FileMIMEType);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                        current.CreatedBy = persisted.CreatedBy;

                        current.RecordStatus = (byte)customerDocumentDTO.RecordStatus;
                        current.ModifiedBy = serviceHeader.ApplicationUserName;
                        current.ModifiedDate = DateTime.Now;

                        _customerDocumentRepository.Merge(persisted, current, serviceHeader);
                    }

                    return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
                }
            }

            else throw new InvalidOperationException(string.Format("Sorry, the document with name '{0}' no longer exists in the specified directory!", customerDocumentDTO.FileName));
        }

        public bool UpdateCollateralStatus(Guid customerDocumentId, int collateralStatus, ServiceHeader serviceHeader)
        {
            if (customerDocumentId != Guid.Empty && Enum.IsDefined(typeof(CustomerDocumentType), collateralStatus))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    var persisted = _customerDocumentRepository.Get(customerDocumentId, serviceHeader);

                    if (persisted != null)
                    {
                        switch ((CustomerDocumentType)persisted.Type)
                        {
                            case CustomerDocumentType.General:
                                break;
                            case CustomerDocumentType.Collateral:

                                var collateral = new Collateral(persisted.Collateral.Value, persisted.Collateral.AdvanceRate, collateralStatus);

                                var current = CustomerDocumentFactory.CreateCustomerDocument(persisted.CustomerId, persisted.Type, collateral, persisted.FileName, persisted.FileTitle, persisted.FileDescription, persisted.FileMIMEType);

                                current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                                current.CreatedBy = persisted.CreatedBy;

                                _customerDocumentRepository.Merge(persisted, current, serviceHeader);

                                break;
                            default:
                                break;
                        }

                        return dbContextScope.SaveChanges(serviceHeader) >= 0;
                    }
                    else return false;
                }
            }
            else return false;
        }

        public async Task<bool> UpdateCollateralStatusAsync(Guid customerDocumentId, int collateralStatus, ServiceHeader serviceHeader)
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                var persisted = await _customerDocumentRepository.GetAsync(customerDocumentId, serviceHeader);

                if (persisted != null)
                {
                    switch ((CustomerDocumentType)persisted.Type)
                    {
                        case CustomerDocumentType.General:
                            break;
                        case CustomerDocumentType.Collateral:

                            var collateral = new Collateral(persisted.Collateral.Value, persisted.Collateral.AdvanceRate, collateralStatus);

                            var current = CustomerDocumentFactory.CreateCustomerDocument(persisted.CustomerId, persisted.Type, collateral, persisted.FileName, persisted.FileTitle, persisted.FileDescription, persisted.FileMIMEType);

                            current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                            current.CreatedBy = persisted.CreatedBy;

                            _customerDocumentRepository.Merge(persisted, current, serviceHeader);

                            break;
                        default:
                            break;
                    }
                }

                return await dbContextScope.SaveChangesAsync(serviceHeader) > 0;
            }
        }

        public List<CustomerDocumentDTO> FindCustomerDocuments(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<CustomerDocument> spec = CustomerDocumentSpecifications.DefaultSpec();

                var customerDocuments = _customerDocumentRepository.AllMatching(spec, serviceHeader);

                if (customerDocuments != null && customerDocuments.Any())
                {
                    return customerDocuments.ProjectedAsCollection<CustomerDocumentDTO>();
                }
                else return null;
            }
        }

        public async Task<List<CustomerDocumentDTO>> FindCustomerDocumentsAsync(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<CustomerDocument> spec = CustomerDocumentSpecifications.DefaultSpec();

                return await _customerDocumentRepository.AllMatchingAsync<CustomerDocumentDTO>(spec, serviceHeader);
            }
        }

        public List<CustomerDocumentDTO> FindCustomerDocuments(Guid customerId, int type, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<CustomerDocument> spec = CustomerDocumentSpecifications.CustomerDocumentWithCustomerIdAndType(customerId, type);

                var customerDocuments = _customerDocumentRepository.AllMatching(spec, serviceHeader);

                if (customerDocuments != null && customerDocuments.Any())
                {
                    return customerDocuments.ProjectedAsCollection<CustomerDocumentDTO>();
                }
                else return null;
            }
        }

        public async Task<List<CustomerDocumentDTO>> FindCustomerDocumentsAsync(Guid customerId, int type, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<CustomerDocument> spec = CustomerDocumentSpecifications.CustomerDocumentWithCustomerIdAndType(customerId, type);

                return await _customerDocumentRepository.AllMatchingAsync<CustomerDocumentDTO>(spec, serviceHeader);
            }
        }

        public PageCollectionInfo<CustomerDocumentDTO> FindCustomerDocuments(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerDocumentSpecifications.DefaultSpec();

                ISpecification<CustomerDocument> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var customerDocumentPagedCollection = _customerDocumentRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (customerDocumentPagedCollection != null)
                {
                    var pageCollection = customerDocumentPagedCollection.PageCollection.ProjectedAsCollection<CustomerDocumentDTO>();

                    var itemsCount = customerDocumentPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CustomerDocumentDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public async Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsAsync(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = CustomerDocumentSpecifications.DefaultSpec();

                ISpecification<CustomerDocument> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _customerDocumentRepository.AllMatchingPagedAsync<CustomerDocumentDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public PageCollectionInfo<CustomerDocumentDTO> FindCustomerDocuments(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? CustomerDocumentSpecifications.DefaultSpec() : CustomerDocumentSpecifications.CustomerDocumentFullText(text);

                ISpecification<CustomerDocument> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var customerDocumentPagedCollection = _customerDocumentRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (customerDocumentPagedCollection != null)
                {
                    var pageCollection = customerDocumentPagedCollection.PageCollection.ProjectedAsCollection<CustomerDocumentDTO>();

                    var itemsCount = customerDocumentPagedCollection.ItemsCount;

                    return new PageCollectionInfo<CustomerDocumentDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public async Task<PageCollectionInfo<CustomerDocumentDTO>> FindCustomerDocumentsAsync(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? CustomerDocumentSpecifications.DefaultSpec() : CustomerDocumentSpecifications.CustomerDocumentFullText(text);

                ISpecification<CustomerDocument> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                return await _customerDocumentRepository.AllMatchingPagedAsync<CustomerDocumentDTO>(spec, pageIndex, pageSize, sortFields, true, serviceHeader);
            }
        }

        public CustomerDocumentDTO FindCustomerDocument(Guid customerDocumentId, ServiceHeader serviceHeader)
        {
            if (customerDocumentId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var customerDocument = _customerDocumentRepository.Get(customerDocumentId, serviceHeader);

                    if (customerDocument != null)
                    {
                        return customerDocument.ProjectedAs<CustomerDocumentDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public async Task<CustomerDocumentDTO> FindCustomerDocumentAsync(Guid customerDocumentId, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _customerDocumentRepository.GetAsync<CustomerDocumentDTO>(customerDocumentId, serviceHeader);
            }
        }
    }
}
