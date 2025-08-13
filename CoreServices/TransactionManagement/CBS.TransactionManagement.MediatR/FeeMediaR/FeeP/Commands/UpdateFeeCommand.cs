using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.FeeMediaR.FeeP.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateFeeCommand : IRequest<ServiceResponse<FeeDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string FeeType { get; set; }//Percentage Or Range Or Flat
        public bool IsAppliesOnHoliday { get; set; }
        public decimal MaximumRateAboveMaximumRange { get; set; }
        public decimal MaximumExtraCharge { get; set; }
        public bool IsMoralPerson { get; set; }
        public string OperationFeeType { get; set; }//MemberShip Or Operation


    }

}
