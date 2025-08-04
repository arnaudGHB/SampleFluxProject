using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Accounting.Queries
{
    public class GetAccountNumberCommand : IRequest<ServiceResponse<GetAccountNumberResponse>>
    {

        public string ProductId { get; set; }
    }

    public class GetAccountNumberResponse
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }
    public class GetAccountNumberByIdResponse
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string RootDescription { get; set; }

        public string ChartOfAccountId { get; set; }

        public string PositionNumber { get; set; }

        public string LevelManagement { get; set; }  // Changed to remove underscore for better C# naming convention

        public string AccountNumber { get; set; }

        public string OldAccountNumber { get; set; }  // Changed to remove underscore for better C# naming convention

        public string NewAccountNumber { get; set; }  // Changed to remove underscore for better C# naming convention
    }

    public class GetAccountNumberByIDCommand : IRequest<ServiceResponse<GetAccountNumberByIdResponse>>
    {

        public string Id { get; set; }
    }
}
