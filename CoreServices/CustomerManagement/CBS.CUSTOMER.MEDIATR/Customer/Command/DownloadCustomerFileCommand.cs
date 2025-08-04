
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using Microsoft.AspNetCore.Http;
using CBS.CUSTOMER.MEDIATR.Customer.Command;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to update customer xsl and xlsx file .
    /// </summary>
    public class DownloadCustomerFileCommand : IRequest<ServiceResponse<Stream>>
    {

        public string? BranchCode { get; set; }


    }

}
