using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Bank.
    /// </summary>
    public class UpdateBankCommand : IRequest<ServiceResponse<BankDto>>
    {
        public string Id { get; set; }
        public string BankCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Capital { get; set; }
        public string RegistrationNumber { get; set; }
        public string SignatureURL { get; set; }
        public string LogoUrl { get; set; }
        public string ImmatriculationNumber { get; set; }
        public string TaxPayerNUmber { get; set; }
        public string PBox { get; set; }
        public string WebSite { get; set; }
        public string DateOfCreation { get; set; }
        public string BankInitial { get; set; }
        public string Motto { get; set; }
        public string OrganizationId { get; set; } // Foreign key
        public string? CustomerServiceContact { get; set; }

    }

}
