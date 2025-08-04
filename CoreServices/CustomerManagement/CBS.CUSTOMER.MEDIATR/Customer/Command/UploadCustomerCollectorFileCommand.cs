
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using Microsoft.AspNetCore.Http;
using CBS.CUSTOMER.MEDIATR.Customer.Command;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to upload customer collector  xsl and xlsx file .
    /// </summary>
    public class UploadCustomerCollectorFileCommand : IRequest<ServiceResponse<List<CustomerFileMemberData>>>
    {

        public IFormFile? formFile { get; set; }


    }

}
