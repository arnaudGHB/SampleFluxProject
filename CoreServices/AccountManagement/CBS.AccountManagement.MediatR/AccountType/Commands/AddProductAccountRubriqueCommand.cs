using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddAccountTypeCommand.
    /// </summary>
    public class  AddProductAccountRubriqueCommand : IRequest<ServiceResponse<bool>>
    {

        public string ProductAccountingBookId { get; set; }
        public string OperationType { get; set; } //DEPOSIT,WITHDRAWAL TRANSFER
        public List<ProductAccountRubric>  AccountRubriques { get; set; }
 
    }

    /// <summary>
    /// Represents a command to add a new AddAccountTypeCommand.
    /// </summary>
    public class AddAccountRubriqueCommand : IRequest<ServiceResponse<AccountRubricResponseDto>>
    {

        public string ProductAccountingBookId { get; set; }
        public string ServiceType { get; set; } //DEPOSIT,WITHDRAWAL TRANSFER
        public List<ProductAccountRubric> AccountRubriques { get; set; }

    }


}