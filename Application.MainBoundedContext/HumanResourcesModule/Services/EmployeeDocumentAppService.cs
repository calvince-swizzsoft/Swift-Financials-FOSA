using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.HumanResourcesModule;
using Application.Seedwork;
using Domain.MainBoundedContext.HumanResourcesModule.Aggregates.EmployeeDocumentAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Infrastructure.Crosscutting.Framework.Utils;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Application.MainBoundedContext.HumanResourcesModule.Services
{
    public class EmployeeDocumentAppService : IEmployeeDocumentAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<EmployeeDocument> _employeeDocumentRepository;

        public EmployeeDocumentAppService(
           IDbContextScopeFactory dbContextScopeFactory,
           IRepository<EmployeeDocument> employeeDocumentRepository)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (employeeDocumentRepository == null)
                throw new ArgumentNullException(nameof(employeeDocumentRepository));

            _dbContextScopeFactory = dbContextScopeFactory;
            _employeeDocumentRepository = employeeDocumentRepository;
        }

        public EmployeeDocumentDTO AddNewEmployeeDocument(
    EmployeeDocumentDTO employeeDocumentDTO,
    string fileUploadDirectory,
    ServiceHeader serviceHeader)
        {
            if (employeeDocumentDTO == null ||
                string.IsNullOrWhiteSpace(fileUploadDirectory) ||
                string.IsNullOrWhiteSpace(employeeDocumentDTO.FileName))
                return null;

            var path = Path.Combine(fileUploadDirectory, employeeDocumentDTO.FileName);

            if (System.IO.File.Exists(path))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    // Get MIME type of the file
                    employeeDocumentDTO.FileMIMEType = System.Web.MimeMapping.GetMimeMapping(path);

                    // Read file content into a byte array
                    byte[] fileBuffer;
                    using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            fileBuffer = memoryStream.ToArray();
                        }
                    }

                    // Create EmployeeDocument with FileBuffer
                    var employeeDocument = EmployeeDocumentFactory.CreateEmployeeDocument(
                        employeeDocumentDTO.EmployeeId,
                        employeeDocumentDTO.FileName,
                        employeeDocumentDTO.FileTitle,
                        employeeDocumentDTO.FileDescription,
                        employeeDocumentDTO.FileMIMEType,
                        fileBuffer);

                    // Set CreatedBy field
                    employeeDocument.CreatedBy = serviceHeader.ApplicationUserName;

                    // Add to repository and save changes
                    _employeeDocumentRepository.Add(employeeDocument, serviceHeader);
                    dbContextScope.SaveChanges(serviceHeader);

                    // Return projected DTO
                    return employeeDocument.ProjectedAs<EmployeeDocumentDTO>();
                }
            }
            else
            {
                return null;
            }
        }


        public bool UpdateEmployeeDocument(EmployeeDocumentDTO employeeDocumentDTO, string fileUploadDirectory, ServiceHeader serviceHeader)
        {
            if (employeeDocumentDTO == null || string.IsNullOrWhiteSpace(fileUploadDirectory) || string.IsNullOrWhiteSpace(employeeDocumentDTO.FileName))
                return false;

            var path = Path.Combine(fileUploadDirectory, employeeDocumentDTO.FileName);

            if (System.IO.File.Exists(path))
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    employeeDocumentDTO.FileMIMEType = System.Web.MimeMapping.GetMimeMapping(path);

                    var persisted = _employeeDocumentRepository.Get(employeeDocumentDTO.Id, serviceHeader);

                    if (persisted != null)
                    {
                        var current = EmployeeDocumentFactory.CreateEmployeeDocument(employeeDocumentDTO.EmployeeId, employeeDocumentDTO.FileName, employeeDocumentDTO.FileTitle, employeeDocumentDTO.FileDescription, employeeDocumentDTO.FileMIMEType, employeeDocumentDTO.FileBuffer);

                        current.ChangeCurrentIdentity(persisted.Id, persisted.SequentialId, persisted.CreatedBy, persisted.CreatedDate);
                        current.CreatedBy = persisted.CreatedBy;


                        _employeeDocumentRepository.Merge(persisted, current, serviceHeader);

                        dbContextScope.SaveChanges(serviceHeader);

                        return true;
                    }
                    else return false;
                }
            }
            else
            {
                 throw new InvalidOperationException(string.Format("Sorry, the document with name '{0}' no longer exists in the specified directory!", employeeDocumentDTO.FileName));
            }
        }

        public List<EmployeeDocumentDTO> FindEmployeeDocuments(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                ISpecification<EmployeeDocument> spec = EmployeeDocumentSpecifications.DefaultSpec();

                var employeeDocuments = _employeeDocumentRepository.AllMatching(spec, serviceHeader);

                if (employeeDocuments != null && employeeDocuments.Any())
                {
                    return employeeDocuments.ProjectedAsCollection<EmployeeDocumentDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeDocumentDTO> FindEmployeeDocuments(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = EmployeeDocumentSpecifications.DefaultSpec();

                ISpecification<EmployeeDocument> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeDocumentPagedCollection = _employeeDocumentRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeDocumentPagedCollection != null)
                {
                    var pageCollection = employeeDocumentPagedCollection.PageCollection.ProjectedAsCollection<EmployeeDocumentDTO>();

                    var itemsCount = employeeDocumentPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeDocumentDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<EmployeeDocumentDTO> FindEmployeeDocuments(string text, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = string.IsNullOrWhiteSpace(text) ? EmployeeDocumentSpecifications.DefaultSpec() : EmployeeDocumentSpecifications.EmployeeDocumentFullText(text);

                ISpecification<EmployeeDocument> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var employeeDocumentPagedCollection = _employeeDocumentRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (employeeDocumentPagedCollection != null)
                {
                    var pageCollection = employeeDocumentPagedCollection.PageCollection.ProjectedAsCollection<EmployeeDocumentDTO>();

                    var itemsCount = employeeDocumentPagedCollection.ItemsCount;

                    return new PageCollectionInfo<EmployeeDocumentDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public EmployeeDocumentDTO FindEmployeeDocument(Guid employeeDocumentId, ServiceHeader serviceHeader)
        {
            if (employeeDocumentId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var employeeDocument = _employeeDocumentRepository.Get(employeeDocumentId, serviceHeader);

                    if (employeeDocument != null)
                    {
                        return employeeDocument.ProjectedAs<EmployeeDocumentDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
