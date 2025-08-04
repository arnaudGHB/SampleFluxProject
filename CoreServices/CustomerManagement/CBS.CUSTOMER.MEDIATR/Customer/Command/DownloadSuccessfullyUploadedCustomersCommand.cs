
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using Microsoft.AspNetCore.Http;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using CBS.CUSTOMER.DATA.Dto.Customers;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to download customer xsl and xlsx file .
    /// </summary>
    public class DownloadSuccessfullyUploadedCustomersCommand : IRequest<ServiceResponse<DownloadSuccessfullyUploadedCustomersDto>>
    {

    }

}
