using AutoMapper;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.AccountClosureRequestAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashDepositRequestAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashTransferRequestAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.CashWithdrawalRequestAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ElectronicJournalAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExpensePayableEntryAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequeAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.ExternalChequePayableAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FiscalCountAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.FixedDepositPayableAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.InHouseChequeAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.SuperSaverPayableAgg;
using Domain.MainBoundedContext.FrontOfficeModule.Aggregates.TruncatedChequeAgg;

namespace Application.MainBoundedContext.DTO.FrontOfficeModule
{
    public class FrontOfficeModuleProfile : Profile
    {
        public FrontOfficeModuleProfile()
        {
            //FiscalCount => FiscalCountDTO
            CreateMap<FiscalCount, FiscalCountDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src =>
                    src.Denomination.OneThousandValue +
                    src.Denomination.FiveHundredValue +
                    src.Denomination.TwoHundredValue +
                    src.Denomination.OneHundredValue +
                    src.Denomination.FiftyValue +
                    src.Denomination.FourtyValue +
                    src.Denomination.TwentyValue +
                    src.Denomination.TenValue +
                    src.Denomination.FiveValue +
                    src.Denomination.OneValue +
                    src.Denomination.FiftyCentValue));

            //ExternalCheque => ExternalChequeDTO
            CreateMap<ExternalCheque, ExternalChequeDTO>()
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedCustomerAccountCustomerSerialNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore());

            //InHouseCheque => InHouseChequeDTO
            CreateMap<InHouseCheque, InHouseChequeDTO>()
                .ForMember(dest => dest.WordifiedAmount, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedAmount, opt => opt.Ignore())
                .ForMember(dest => dest.DebitChartOfAccountName, opt => opt.Ignore())
                .ForMember(dest => dest.DebitCustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.DebitCustomerAccountCustomerFullName, opt => opt.Ignore());

            //FixedDeposit => FixedDepositDTO
            CreateMap<FixedDeposit, FixedDepositDTO>()
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore());

            //FixedDepositPayable => FixedDepositPayableDTO
            CreateMap<FixedDepositPayable, FixedDepositPayableDTO>();

            //CashWithdrawalRequest => CashWithdrawalRequestDTO
            CreateMap<CashWithdrawalRequest, CashWithdrawalRequestDTO>()
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore());

            //ElectronicJournal => ElectronicJournalDTO
            CreateMap<ElectronicJournal, ElectronicJournalDTO>()
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //TruncatedCheque => TruncatedChequeDTO
            CreateMap<TruncatedCheque, TruncatedChequeDTO>()
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.ReasonForReturnCodeDescription, opt => opt.Ignore());

            //ExpensePayable => ExpensePayableDTO
            CreateMap<ExpensePayable, ExpensePayableDTO>()
                .ForMember(dest => dest.TypeDescription, opt => opt.Ignore())
                .ForMember(dest => dest.PaddedVoucherNumber, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore())
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore());

            //ExpensePayableEntry => ExpensePayableEntryDTO
            CreateMap<ExpensePayableEntry, ExpensePayableEntryDTO>()
                .ForMember(dest => dest.ChartOfAccountName, opt => opt.Ignore());

            //ExternalChequePayable => ExternalChequePayableDTO
            CreateMap<ExternalChequePayable, ExternalChequePayableDTO>();

            //AccountClosureRequest => AccountClosureRequestDTO
            CreateMap<AccountClosureRequest, AccountClosureRequestDTO>()
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());

            //SuperSaverPayable => SuperSaverPayableDTO
            CreateMap<SuperSaverPayable, SuperSaverPayableDTO>();

            //CashTransferRequest => CashTransferRequestDTO
            CreateMap<CashTransferRequest, CashTransferRequestDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

            //CashTransferRequestDTO => CashTransferRequestBindingModel
            CreateMap<CashTransferRequestDTO, CashTransferRequestBindingModel>();

            //CashDepositRequest => CashDepositRequestDTO
            CreateMap<CashDepositRequest, CashDepositRequestDTO>()
                .ForMember(dest => dest.CustomerAccountFullAccountNumber, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerIndividualSalutationDescription, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerAccountCustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.StatusDescription, opt => opt.Ignore());
        }
    }
}
