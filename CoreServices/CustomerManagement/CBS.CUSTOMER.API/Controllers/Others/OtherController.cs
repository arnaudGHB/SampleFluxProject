using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.Customer.MEDIATR;
using CBS.Customer.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.DATA.Entity.Config;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;


namespace CBS.CUSTOMER.API.Controllers.Other
{
    /// <summary>
    /// Controller for managing Others and related operations
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class OtherController : BaseController
    {
        private readonly IMediator _mediator;
        private  IWebHostEnvironment _webHostEnvironment;

        /// <summary>
        /// Constructor for the OtherController
        /// This OtherController manages operations related to Others and their customers.
        /// It includes methods for retrieving, creating, and updating Others and Other customers.
        /// The controller is secured with authorization, and MediatR is used for handling queries and commands.
        /// Each method has appropriate summaries for clarity.
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public OtherController(IMediator mediator, IWebHostEnvironment webHostEnvironment)
        {
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;
        }





        /// <summary>
        /// Get customer By ID And Profile by CustomerId
        /// </summary>
        [HttpGet("Customer/Search/{CustomerId}", Name = "GetSystemCustomerByID")]
        [Produces("application/json", "application/xml", Type = typeof(GetByCustomerByIdWithProfileTypeDto))]
        public async Task<IActionResult> GetSystemCustomerByID(string CustomerId)
        {
            var getCustomerSecretByPhoneQuery = new GetCustomerByIDAndProfileCommand { Id = CustomerId };
            var result = await _mediator.Send(getCustomerSecretByPhoneQuery);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Get all System Customers
        /// </summary>
        [HttpGet("Customer/System/GetAll")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetAllSystemCustomer>))]
        public async Task<IActionResult> GetAllSystemCustomers()
        {
            var getAllSystemCustomerQuery = new GetAllSystemCustomerQuery();
            var result = await _mediator.Send(getAllSystemCustomerQuery);
            return Ok(result);
        }


        [HttpPost("uploadCustomerDocuments")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UploadCustomerDocuments(AddDocumentCommand addDocumentCommand)
        {
            var result = await _mediator.Send(addDocumentCommand);
            return ReturnFormattedResponse(result);
        }

      


        /// <summary>
        /// Activate Or Dis-Activate Any Customer in the system
        /// </summary>
        [HttpPut("Customer/ActivateOrDis-activate")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> ChangeCustomerPin(DisActivateCustomerCommand disActivateCustomerCommand)
        {
            var result = await _mediator.Send(disActivateCustomerCommand);
            return ReturnFormattedResponse(result);
        }
        
        
        /// <summary>
        /// Upload Excel File Customer File in the system
        /// </summary>
        [HttpPost("UploadCustomerFile")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerFileMemberData>))]
        public async Task<IActionResult> UploadCustomerFile([FromQuery]UploadCustomerFileCommand uploadCustomerFileCommand)
        {
            var result = await _mediator.Send(uploadCustomerFileCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload Excel File Customer Collector File in the system
        /// </summary>
        [HttpPost("UploadDailyCollectorFile")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerFileMemberData>))]
        public async Task<IActionResult> UploadCustomerCollectorFile([FromQuery] UploadCustomerCollectorFileCommand uploadCustomerFileCommand)
        {
            var result = await _mediator.Send(uploadCustomerFileCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete Excel File Customer File in the system
        /// </summary>
        [HttpPost("DeleteCustomerFile")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerFileMemberData>))]
        public async Task<IActionResult> DeleteCustomerFile([FromQuery] DeleteCustomerFileCommand uploadCustomerFileCommand)
        {
            var result = await _mediator.Send(uploadCustomerFileCommand);
            return ReturnFormattedResponse(result);
        } 
        
        /// <summary>
        /// Verify Excel File Customer File is completely upload in system in the system
        /// </summary>
        [HttpPost("VerifyCustomerFile")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerFileMemberData>))]
        public async Task<IActionResult> VerifyCustomerFile([FromQuery] VerifyCustomerFileCommand verifyCustomerFileCommand)
        {
            var result = await _mediator.Send(verifyCustomerFileCommand);
            return ReturnFormattedResponse(result);
        } 
        
        
        /// <summary>
        /// download file upload result File is completely upload in system in the system
        /// </summary>
        [HttpPost("DownloadUploadResultFile")]
        public async Task<IActionResult> VerifyCustomerFile([FromQuery] DownloadCustomerFileCommand downloadFile)
        {
            var result = await _mediator.Send(downloadFile);
            var stream=result.Data;
            if (stream == null)
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.

            return File(stream, "application/octet-stream", "File" + downloadFile.BranchCode + ".csv"); // returns a FileStreamResult

        } 
        
        
        /// <summary>
        /// download file upload result File is completely upload in system in the system
        /// </summary>
        [HttpPost("DownloadSuccessfullyUploadedCustomers")]
        public async Task<IActionResult> VerifyCustomerFile( )
        {
            DownloadSuccessfullyUploadedCustomersCommand downloadFile = new DownloadSuccessfullyUploadedCustomersCommand();
            var result = await _mediator.Send(downloadFile);

          var stream=result.Data;
            if (stream == null)       
                return NotFound(); // returns a NotFoundResult with Status404NotFound response.
            return File(stream.Stream, "application/octet-stream",  stream.FileName ); // returns a FileStreamResult

        }


        /// <summary>
        /// Search Customer By CustomerId, FirstName , LastName, CNI ,GroupType
        /// </summary>
        [HttpGet("Customer/System/Search")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetCustomerByIDOrCINOrNameDto>))]
        public async Task<IActionResult> ChangeCustomerPin(GetCustomerByIDOrCINOrNameCommand customerByIDOrCINOrNameCommand)
        {
            var result = await _mediator.Send(customerByIDOrCINOrNameCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Get All Subscription Aggregates Enum
        /// </summary>
        [HttpGet("SubscriptionAggregates/GetAll")]
        [Produces("application/json", "application/xml", Type = typeof(SubscriptionAggregatesDto))]
        public async Task<IActionResult> GetAllSubscriptionAggregates( )
        {
            var getAllSubscriptionAggregates = new GetAllSubscriptionAggregatesQuery { };
            var result = await _mediator.Send(getAllSubscriptionAggregates);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Get all GetDocuments
        /// </summary>
        [HttpGet("GetDocuments")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetCustomerDocument>))]
        public async Task<IActionResult> GetAllCustomerDocuments()
        {
            var getAllCustomerQuery = new GetAllCustomerDocumentsQuery { };
            var result = await _mediator.Send(getAllCustomerQuery);
            return Ok(result);
        } 
        
        /// <summary>
        /// Get all GetDocuments By CustomerId
        /// </summary>
        [HttpGet("GetDocumentsByCustomerId/{CustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetCustomerDocument>))]
        public async Task<IActionResult> GetCustomerDocumentByCustomerId(string CustomerId)
        {
            var getAllCustomerQuery = new GetCustomerDocumentByCustomerIdQuery {CustomerId=CustomerId };
            var result = await _mediator.Send(getAllCustomerQuery);
            return Ok(result);
        }

        /// <summary>
        /// Delete Document by CustomerId
        /// </summary>
        [HttpDelete("Document/{CustomerId}")]
        public async Task<IActionResult> DeleteCustomerDocument(string Id)
        {
            var deleteCustomerCommand = new DeleteCustomerDocumentCommand { Id = Id };
            var result = await _mediator.Send(deleteCustomerCommand);
            return ReturnFormattedResponse(result);
        }

    }
}

