namespace CBS.AccountManagement.Data
{
    public class BalanceSheetAsset : BaseEntity
    {
        public string? DocumentTypeId { get; set; }
        public string? DocumentId { get; set; }
        public string? Reference { get; set; }
        public string? HeadingEN { get; set; }
        public string? HeadingFR { get; set; }
       
    }

    public class BalanceSheetAssetDto: BalanceSheetAsset
    {
         
        public string? ListGrossDr { get; set; }
        public string? ListProvCr { get; set; }
    }
}