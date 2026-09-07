using System;
namespace Application.MainBoundedContext.DTO
{
    public class AssetDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
        public Guid AssetTypeId { get; set; }
        public string AssetType { get; set; }
        public Guid BranchId { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public DateTime DepreciationStartDate { get; set; }
        public DateTime? LastDepreciationDate { get; set; }
        public decimal AccumulatedDepreciation { get; set; }
        public decimal NetBookValue { get { return PurchasePrice - AccumulatedDepreciation; } }
        public string Supplier { get; set; }
        public string Department { get; set; }
        public string PicturePath { get; set; }
        public Guid GLAccountId { get; set; }
        public string GLAccount { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string TagNumber { get; set; }
        public string Location { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal ResidualValue { get; set; }
    }
    public class AssetDepreciationDTO
    {
        public Guid AssetId { get; set; }
        public DateTime PeriodDate { get; set; }
        public decimal Amount { get; set; }
        public decimal AccumulatedDepreciation { get; set; }
        public decimal NetBookValue { get; set; }
        public bool Posted { get; set; }
        public Guid? JournalId { get; set; }
    }
}
