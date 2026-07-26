using Application.MainBoundedContext.DTO.AccountsModule;
using Application.MainBoundedContext.DTO.FrontOfficeModule;
using Application.MainBoundedContext.DTO.RegistryModule;
using System.Collections.Generic;

namespace Application.MainBoundedContext.DTO
{
    public class BatchImportParseInfo
    {
        public List<CreditBatchEntryDTO> MatchedCollection1 { get; set; }

        public List<DebitBatchEntryDTO> MatchedCollection2 { get; set; }

        public List<ApportionmentWrapper> MatchedCollection3 { get; set; }

        public List<CustomerDTO> MatchedCollection4 { get; set; }

        public List<InHouseChequeDTO> MatchedCollection5 { get; set; }

        public List<AlternateChannelLogDTO> MatchedCollection6 { get; set; }

        public List<WireTransferBatchEntryDTO> MatchedCollection7 { get; set; }

        public List<OverDeductionBatchEntryDTO> MatchedCollection8 { get; set; }

        public List<GeneralLedgerEntryDTO> MatchedCollection9 { get; set; }

        public List<BatchImportEntryWrapper> MismatchedCollection { get; set; }
    }
}
