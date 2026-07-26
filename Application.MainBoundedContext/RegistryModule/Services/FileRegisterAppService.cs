using Application.MainBoundedContext.DTO;
using Application.MainBoundedContext.DTO.RegistryModule;
using Application.MainBoundedContext.HumanResourcesModule.Services;
using Application.Seedwork;
using Infrastructure.Crosscutting.Framework.Utils;
using Domain.MainBoundedContext.RegistryModule.Aggregates.FileMovementHistoryAgg;
using Domain.MainBoundedContext.RegistryModule.Aggregates.FileRegisterAgg;
using Domain.Seedwork;
using Domain.Seedwork.Specification;
using Numero3.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.MainBoundedContext.RegistryModule.Services
{
    public class FileRegisterAppService : IFileRegisterAppService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IRepository<FileRegister> _fileRegisterRepository;
        private readonly IRepository<FileMovementHistory> _fileMovementHistoryRepository;
        private readonly IDepartmentAppService _departmentAppService;

        public FileRegisterAppService(
            IDbContextScopeFactory dbContextScopeFactory,
            IRepository<FileRegister> fileRegisterRepository,
            IRepository<FileMovementHistory> fileMovementHistoryRepository,
            IDepartmentAppService departmentAppService)
        {
            if (dbContextScopeFactory == null)
                throw new ArgumentNullException(nameof(dbContextScopeFactory));

            if (fileRegisterRepository == null)
                throw new ArgumentNullException(nameof(fileRegisterRepository));

            if (fileMovementHistoryRepository == null)
                throw new ArgumentNullException(nameof(fileMovementHistoryRepository));

            if (departmentAppService == null)
                throw new ArgumentNullException(nameof(departmentAppService));

            _dbContextScopeFactory = dbContextScopeFactory;
            _fileRegisterRepository = fileRegisterRepository;
            _fileMovementHistoryRepository = fileMovementHistoryRepository;
            _departmentAppService = departmentAppService;
        }

        public bool MultiDestinationDispatch(List<FileMovementHistoryDTO> fileMovementHistoryDTOs, ServiceHeader serviceHeader)
        {
            if (fileMovementHistoryDTOs != null && fileMovementHistoryDTOs.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    fileMovementHistoryDTOs.ForEach(fileMovementHistoryDTO =>
                    {
                        var fileRegisterDTO = FindFileRegisterByCustomerId(fileMovementHistoryDTO.FileRegisterCustomerId, serviceHeader);

                        if (fileRegisterDTO == null)
                        {
                            var newFileRegister = FileRegisterFactory.CreateFileRegister(fileMovementHistoryDTO.FileRegisterCustomerId, (int)FileMovementStatus.Dispatched);

                            _fileRegisterRepository.Add(newFileRegister, serviceHeader);

                            var newFileMovementHistory = FileMovementHistoryFactory.CreateFileMovementHistory(newFileRegister.Id, fileMovementHistoryDTO.SourceDepartmentId, fileMovementHistoryDTO.DestinationDepartmentId, fileMovementHistoryDTO.Remarks, fileMovementHistoryDTO.Carrier);
                            newFileMovementHistory.Sender = serviceHeader.ApplicationUserName;
                            newFileMovementHistory.SendDate = DateTime.Now;

                            _fileMovementHistoryRepository.Add(newFileMovementHistory, serviceHeader);
                        }
                        else
                        {
                            var persistedFileRegister = _fileRegisterRepository.Get(fileRegisterDTO.Id, serviceHeader);

                            persistedFileRegister.Status = (int)FileMovementStatus.Dispatched;

                            var newFileMovementHistory = FileMovementHistoryFactory.CreateFileMovementHistory(persistedFileRegister.Id, fileMovementHistoryDTO.SourceDepartmentId, fileMovementHistoryDTO.DestinationDepartmentId, fileMovementHistoryDTO.Remarks, fileMovementHistoryDTO.Carrier);
                            newFileMovementHistory.Sender = serviceHeader.ApplicationUserName;
                            newFileMovementHistory.SendDate = DateTime.Now;

                            _fileMovementHistoryRepository.Add(newFileMovementHistory, serviceHeader);
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool SingleDestinationDispatch(Guid sourceDepartmentId, Guid destinationDepartmentId, string remarks, string carrier, List<FileRegisterDTO> fileRegisterDTOs, ServiceHeader serviceHeader)
        {
            if (destinationDepartmentId != null && fileRegisterDTOs != null && fileRegisterDTOs.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    fileRegisterDTOs.ForEach(fileRegisterDTO =>
                    {
                        var persistedFileRegister = _fileRegisterRepository.Get(fileRegisterDTO.Id, serviceHeader);

                        if (persistedFileRegister != null)
                        {
                            persistedFileRegister.Status = (int)FileMovementStatus.Dispatched;

                            var newFileMovementHistory = FileMovementHistoryFactory.CreateFileMovementHistory(persistedFileRegister.Id, sourceDepartmentId, destinationDepartmentId, remarks, carrier);
                            newFileMovementHistory.Sender = serviceHeader.ApplicationUserName;
                            newFileMovementHistory.SendDate = DateTime.Now;

                            _fileMovementHistoryRepository.Add(newFileMovementHistory, serviceHeader);
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool ReceiveFiles(List<FileRegisterDTO> fileRegisterDTOs, ServiceHeader serviceHeader)
        {
            if (fileRegisterDTOs != null && fileRegisterDTOs.Any())
            {
                using (var dbContextScope = _dbContextScopeFactory.Create())
                {
                    fileRegisterDTOs.ForEach(fileRegisterDTO =>
                    {
                        var targetFileRegister = _fileRegisterRepository.Get(fileRegisterDTO.Id, serviceHeader);

                        if (targetFileRegister != null)
                        {
                            targetFileRegister.Status = (int)FileMovementStatus.Received;

                            var targetFileMovementHistory = targetFileRegister.History.OrderBy(f => f.CreatedDate).Last();

                            if (targetFileMovementHistory != null)
                            {
                                targetFileMovementHistory.Recipient = serviceHeader.ApplicationUserName;
                                targetFileMovementHistory.ReceiveDate = DateTime.Now;
                            }
                        }
                    });

                    return dbContextScope.SaveChanges(serviceHeader) >= 0;
                }
            }
            else return false;
        }

        public bool RecallFiles(List<FileRegisterDTO> fileRegisterDTOs, ServiceHeader serviceHeader)
        {
            if (fileRegisterDTOs != null && fileRegisterDTOs.Any())
            {
                var registryDepartment = _departmentAppService.FindRegistryDepartment(serviceHeader);

                if (registryDepartment != null)
                {
                    var dispatchList = new List<FileMovementHistoryDTO>();

                    using (var dbContextScope = _dbContextScopeFactory.Create())
                    {
                        fileRegisterDTOs.ForEach(fileRegisterDTO =>
                        {
                            var targetFileRegister = _fileRegisterRepository.Get(fileRegisterDTO.Id, serviceHeader);

                            if (targetFileRegister != null)
                            {
                                var targetFileMovementHistory = targetFileRegister.History.OrderBy(x => x.CreatedDate).Last();

                                if (targetFileMovementHistory != null)
                                {
                                    var newFileMovementHistory = new FileMovementHistoryDTO
                                    {
                                        FileRegisterCustomerId = targetFileRegister.CustomerId,
                                        SourceDepartmentId = targetFileMovementHistory.DestinationDepartmentId,
                                        DestinationDepartmentId = registryDepartment.Id,
                                        Remarks = "Recalled",
                                        Carrier = serviceHeader.ApplicationUserName
                                    };

                                    dispatchList.Add(newFileMovementHistory);
                                }
                            }
                        });
                    }

                    // Step 1: Dispatch to registryDepartment
                    MultiDestinationDispatch(dispatchList, serviceHeader);

                    // Step 2: Receive in registryDepartment
                    ReceiveFiles(fileRegisterDTOs, serviceHeader);

                    return true;
                }
                else return false;
            }
            else return false;
        }

        public FileRegisterDTO FindFileRegister(Guid fileRegisterId, ServiceHeader serviceHeader)
        {
            if (fileRegisterId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var fileRegister = _fileRegisterRepository.Get(fileRegisterId, serviceHeader);

                    if (fileRegister != null) //adapt
                    {
                        return fileRegister.ProjectedAs<FileRegisterDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public FileRegisterDTO FindFileRegisterByCustomerId(Guid customerId, ServiceHeader serviceHeader)
        {
            if (customerId != null && customerId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = FileRegisterSpecifications.FileRegisterWithCustomerId(customerId);

                    ISpecification<FileRegister> spec = filter;

                    var fileRegisters = _fileRegisterRepository.AllMatching(spec, serviceHeader);

                    if (fileRegisters != null && fileRegisters.Any() && fileRegisters.Count() == 1)
                    {
                        return fileRegisters.Single().ProjectedAs<FileRegisterDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }

        public List<FileRegisterDTO> FindFileRegisters(ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var fileRegisters = _fileRegisterRepository.GetAll(serviceHeader);

                if (fileRegisters != null && fileRegisters.Any())
                {
                    return fileRegisters.ProjectedAsCollection<FileRegisterDTO>();
                }
                else return null;
            }
        }

        public PageCollectionInfo<FileRegisterDTO> FindFileRegisters(int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FileRegisterSpecifications.DefaultSpec();

                ISpecification<FileRegister> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fileRegisterPagedCollection = _fileRegisterRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fileRegisterPagedCollection != null)
                {
                    var pageCollection = fileRegisterPagedCollection.PageCollection.ProjectedAsCollection<FileRegisterDTO>();

                    var itemsCount = fileRegisterPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FileRegisterDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FileRegisterDTO> FindFileRegisters(string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FileRegisterSpecifications.FileRegisterFullText(text, customerFilter);

                ISpecification<FileRegister> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fileRegisterPagedCollection = _fileRegisterRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fileRegisterPagedCollection != null)
                {
                    var pageCollection = fileRegisterPagedCollection.PageCollection.ProjectedAsCollection<FileRegisterDTO>();

                    var itemsCount = fileRegisterPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FileRegisterDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public PageCollectionInfo<FileRegisterDTO> FindFileRegisters(int fileMovementStatus, Guid lastDestinationDepartmentId, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (Enum.IsDefined(typeof(FileMovementStatus), fileMovementStatus) && lastDestinationDepartmentId != null && lastDestinationDepartmentId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = FileRegisterSpecifications.FileRegisterWithStatusAndLastFileMovementDestinationDepartmentId(fileMovementStatus, lastDestinationDepartmentId, text, customerFilter);

                    ISpecification<FileRegister> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var fileRegisterPagedCollection = _fileRegisterRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (fileRegisterPagedCollection != null)
                    {
                        var pageCollection = fileRegisterPagedCollection.PageCollection.ProjectedAsCollection<FileRegisterDTO>();

                        var itemsCount = fileRegisterPagedCollection.ItemsCount;

                        return new PageCollectionInfo<FileRegisterDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public PageCollectionInfo<FileRegisterDTO> FindFileRegistersExcludingLastDestinationDepartmentId(Guid lastDestinationDepartmentId, string text, int customerFilter, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            if (lastDestinationDepartmentId != null && lastDestinationDepartmentId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = FileRegisterSpecifications.FileRegisterWithoutLastFileMovementDestinationDepartmentId(lastDestinationDepartmentId);

                    if (!string.IsNullOrEmpty(text))
                        filter = filter & FileRegisterSpecifications.FileRegisterFullText(text, customerFilter);

                    ISpecification<FileRegister> spec = filter;

                    var sortFields = new List<string> { "SequentialId" };

                    var fileRegisterPagedCollection = _fileRegisterRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                    if (fileRegisterPagedCollection != null)
                    {
                        var pageCollection = fileRegisterPagedCollection.PageCollection.ProjectedAsCollection<FileRegisterDTO>();

                        var itemsCount = fileRegisterPagedCollection.ItemsCount;

                        return new PageCollectionInfo<FileRegisterDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                    }
                    else return null;
                }
            }
            else return null;
        }

        public CustomerFileRegisterLastDepartmentInfo FindFileRegisterAndLastDepartmentByCustomerId(Guid customerId, ServiceHeader serviceHeader)
        {
            var fileRegister = FindFileRegisterByCustomerId(customerId, serviceHeader);

            if (fileRegister != null)
            {
                var fileMovementHistory = FindFileMovementHistoryByFileRegisterId(fileRegister.Id, serviceHeader);

                if (fileMovementHistory != null && fileMovementHistory.Any())
                {
                    var lastHistory = fileMovementHistory.OrderByDescending(x => x.CreatedDate).First();

                    var destinationDepartment = _departmentAppService.FindDepartment(lastHistory.DestinationDepartmentId, serviceHeader);

                    return new CustomerFileRegisterLastDepartmentInfo { FileRegister = fileRegister, LastDepartment = destinationDepartment };
                }
                else return null;
            }
            else return null;
        }

        public PageCollectionInfo<FileMovementHistoryDTO> FindFileMovementHistoryByFileRegisterId(Guid fileRegisterId, int pageIndex, int pageSize, ServiceHeader serviceHeader)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var filter = FileMovementHistorySpecifications.FileMovementHistoryWithFileRegisterId(fileRegisterId);

                ISpecification<FileMovementHistory> spec = filter;

                var sortFields = new List<string> { "SequentialId" };

                var fileMovementHistoryPagedCollection = _fileMovementHistoryRepository.AllMatchingPaged(spec, pageIndex, pageSize, sortFields, true, serviceHeader);

                if (fileMovementHistoryPagedCollection != null)
                {
                    var pageCollection = fileMovementHistoryPagedCollection.PageCollection.ProjectedAsCollection<FileMovementHistoryDTO>();

                    var itemsCount = fileMovementHistoryPagedCollection.ItemsCount;

                    return new PageCollectionInfo<FileMovementHistoryDTO> { PageCollection = pageCollection, ItemsCount = itemsCount };
                }
                else return null;
            }
        }

        public List<FileMovementHistoryDTO> FindFileMovementHistoryByFileRegisterId(Guid fileRegisterId, ServiceHeader serviceHeader)
        {
            if (fileRegisterId != Guid.Empty)
            {
                using (_dbContextScopeFactory.CreateReadOnly())
                {
                    var filter = FileMovementHistorySpecifications.FileMovementHistoryWithFileRegisterId(fileRegisterId);

                    ISpecification<FileMovementHistory> spec = filter;

                    var fileMovementHistoryCollection = _fileMovementHistoryRepository.AllMatching(spec, serviceHeader);

                    if (fileMovementHistoryCollection != null)
                    {
                        return fileMovementHistoryCollection.ProjectedAsCollection<FileMovementHistoryDTO>();
                    }
                    else return null;
                }
            }
            else return null;
        }
    }
}
