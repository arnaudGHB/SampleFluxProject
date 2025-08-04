using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
        public class UpdateCustomerCommand : IRequest<ServiceResponse<UpdateCustomer>>
        {

            public string? Id { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
  
            public string? Gender { get; set; }

            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? IDNumber { get; set; }
            public string? CountryId { get; set; }
            public string? RegionId { get; set; }
            public string? TownId { get; set; }
            public string? Address { get; set; }
            public string? ZipCode { get; set; }
            public string? HomePhone { get; set; }
            public string? PersonalPhone { get; set; }
        public string? Language { get; set; }
        public bool IsUseOnlineMobileBanking { get; set; }
            public string? LoginState { get; set; }
            public string? PackageId { get; set; }
            public string? DivisionId { get; set; }
            public string? BranchId { get; set; }
            public string? EconomicActivitesId { get; set; }
            public string? BankId { get; set; }
            public string? Pin { get; set; }

            public string? OrganisationId { get; set; }

            public string? SubdivisionId { get; set; }

            public string? TaxIdentificationNumber { get; set; }

            public int LoanCycle { get; set; }

    

        }

}
