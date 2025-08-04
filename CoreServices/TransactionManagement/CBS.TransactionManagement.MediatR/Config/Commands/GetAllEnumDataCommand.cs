using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Helper.Model;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Config.
    /// </summary>
    public class GetAllEnumDataCommand : IRequest<ServiceResponse<EnumData>>
    {
    }

}
