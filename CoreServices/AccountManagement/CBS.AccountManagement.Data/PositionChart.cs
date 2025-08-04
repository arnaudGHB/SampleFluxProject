namespace CBS.AccountManagement.Data
{
    public class PositionChart
    {
        public string ChartOfAccountManagementPositionId { get; set; }
        public string chartOfAccountID { get; set; }
        public ChartOfAccountManagementPosition ChartOfAccountManagementPosition { get; set; }
        public bool Isperesent { get; set; }
        public bool Is451peresent { get; set; }
        public bool Is18112peresent { get; set; }
    }
}