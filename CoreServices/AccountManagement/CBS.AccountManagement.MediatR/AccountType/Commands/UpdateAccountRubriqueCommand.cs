using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a AddAccountRubriqueCommand.
    /// </summary>
    public class UpdateAccountRubriqueCommand : IRequest<ServiceResponse<AccountRubricResponseDto>>
    {
        public string OperationAccountTypeId { get; set; }
        public string OperationAccountType { get; set; }

        //public List<AccountRubricDto> AccountRubriques { get; set; }

    }
}