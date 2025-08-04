using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new UpdateProductAccountingBookCommand.
    /// </summary>
    public class UpdateProductAccountingBookCommand : IRequest<ServiceResponse<UpdateNewProductAccountingBook>>
    {
        public List<ProductAccountBookDetail> ProductAccountBookDetails { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get;  set; }
    }

    public class UpdateNewProductAccountingBook 
    {
        public List<ProductAccountBookDetail> ItemsToCreate { get; set; }
        public bool hasNewAccountingBooks { get; set; } = false;
        public List<AccountNotUpdated> ItemNotUpdated { get; set; }
        public bool WasCompletelySuccessFull { get; set; } = false;
        public string ErrorMessage { get; internal set; }
    }

    public class AccountNotUpdated  
    {
   
        public string AccountId { get; set; }
        public string AccountName { get; set; }
    }






}