using CBS.Communication;
using CBS.Communication.Helper.Helper;
using MediatR;

namespace CBS.Communication.MediatR.Email.Commands;

public class SendEmailSpecificationCommand : IRequest<ServiceResponse<bool>>
{
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public string? CCAddress { get; set; }
    public string? Body { get; set; }
    public string? Subject { get; set; }
    public List<Helper.Helper.FileInfo>? Attachments { get; set; }
}
