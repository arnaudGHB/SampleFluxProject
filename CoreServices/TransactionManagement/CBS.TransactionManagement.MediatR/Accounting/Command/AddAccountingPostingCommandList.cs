using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;


namespace CBS.TransactionManagement.MediatR.Accounting.Command
{
    public class AddAccountingPostingCommandList : IRequest<ServiceResponse<bool>>
    {
        public string OperationType { get; set; }

        public List<AddAccountingPostingCommand> MakeAccountPostingCommands { get; set; }
        public AddAccountingPostingCommandList()
        {
            MakeAccountPostingCommands = new List<AddAccountingPostingCommand>();
        }
    }
}
