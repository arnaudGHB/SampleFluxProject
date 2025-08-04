using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Branch.
    /// </summary>
    public class AddBranchCommand : IRequest<ServiceResponse<BranchDto>>
    {
        public string BranchCode { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public bool IsHavingBank { get; set; }
        public string? Telephone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string BankId { get; set; }
        public string? Capital { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? LogoUrl { get; set; }
        public string? ImmatriculationNumber { get; set; }
        public string? TaxPayerNUmber { get; set; }
        public string? PBox { get; set; }
        public string? WebSite { get; set; }
        public string? DateOfCreation { get; set; }
        public string? BankInitial { get; set; }
        public string? Motto { get; set; }
        public bool ActiveStatus { get; set; }
        public bool IsHeadOffice { get; set; }

        public string? HeadOfficeTelehoneNumber { get; set; }
        public string? HeadOfficeAddress { get; set; }// Foreign key

    }

}
