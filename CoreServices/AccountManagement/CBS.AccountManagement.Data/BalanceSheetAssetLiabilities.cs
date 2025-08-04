namespace CBS.AccountManagement.Data
{
    public class BalanceSheetAccount:BaseEntity
    {
        public string  Id { get; set; }
        public string? Reference { get; set; }
        // Account Holder
        public string? Description { get; set; }

        public double Amount { get; set; }

        public string Cartegory { get; set; }



    }
    public class BalanceSheetAssetLiabilities : BaseEntity
    {
        public string? DocumentTypeId { get; set; }
        public string? DocumentId { get; set; }
        public string? Reference { get; set; }
        public string? HeadingEn { get; set; }
        public string? HeadingFr { get; set; }
    }

    public class BalanceSheetAssetLiabilitiesDto : BalanceSheetAssetLiabilities
    {

   
        public string selectedException { get; set; }
        public string selectedList { get; set; }
        //public BalanceSheetAssetLiabilitiesDto GetExceptionAccounts(string item)
        //{

        //    BalanceSheetAssetLiabilitiesDto model = new BalanceSheetAssetLiabilitiesDto();

        //    if (item.Contains("sauf"))
        //    {
        //        model.selected = item.Split("sauf")[0];
        //        model.selectedException = item.Split("sauf")[1].Split(",").ToList();
        //    }
        //    else
        //    {
        //        model.selectedList = item.Split("/").ToList();
        //    }

        //    return model;

        //}
    }

}