using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.Queries
{
    public class GetCustomersByMatriculesQuery : IRequest<ServiceResponse<List<CustomerDto>>>
    {
        public List<string> Matricules { get; set; }
        public bool IsMatricule { get; set; }
        public string? OperationType { get; set; }
    }
}
