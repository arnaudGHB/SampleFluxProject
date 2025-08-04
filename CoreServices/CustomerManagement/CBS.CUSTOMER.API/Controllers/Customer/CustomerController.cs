using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankMGT.MediatR.Validations;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using CBS.CUSTOMER.MEDIATR.Customer.Queries;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CBS.CUSTOMER.API.Controllers.Customer
{
    /// <summary>
    /// Controller for managing customers and related operations
    /// This CustomerDashboardController manages operations related to customers and their dependencies. 
    /// It includes methods for retrieving, creating, updating, and deleting customers, customer dependents, and customer requirements. 
    /// The controller is secured with authorization, and MediatR is used for handling queries and commands. 
    /// Each method has appropriate summaries for clarity.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CustomerController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the CustomerDashboardController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get customer date of birth by CustomerId
        /// </summary>
        [HttpGet("Customer/DateOfBirth/{id}", Name = "GetCustomerDateOfBirthDto")]
        [Produces("application/json", "application/xml", Type = typeof(GetCustomerDateOfBirthDto))]
        public async Task<IActionResult> GetCustomerDateOfBirthQuery(string id)
        {
            var getCustomerQuery = new GetCustomerDateOfBirthQuery { Id = id };
            var result = await _mediator.Send(getCustomerQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves a list of members based on the specified CustomerType.
        /// </summary>
        /// <param name="customerType">The type of customers to retrieve (e.g., "Regular", "VIP").</param>
        /// <returns>A list of members matching the specified CustomerType.</returns>
        [HttpGet("Customer/MembersByType/{customerType}", Name = "GetMembersByCustomerType")]
        [Produces("application/json", "application/xml", Type = typeof(ServiceResponse<List<CustomerDto>>))]
        public async Task<IActionResult> GetMembersByCustomerTypeQuery(string? customerType)
        {
            var getMembersQuery = new GetMembersByCustomerTypeQuery { CustomerType=customerType };
            var result = await _mediator.Send(getMembersQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get customer by CustomerId
        /// </summary>
        [HttpGet("Customer/{id}", Name = "CustomerDto")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerDto))]
        public async Task<IActionResult> GetCustomer(string id)
        {
            var getCustomerQuery = new GetCustomerQuery { Id = id };
            var result = await _mediator.Send(getCustomerQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        [HttpGet("Customers")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetAllCustomers>))]
        public async Task<IActionResult> GetCustomers()
        {
            var getAllCustomerQuery = new GetAllCustomerQuery { };
            var result = await _mediator.Send(getAllCustomerQuery);
            return Ok(result);
        }
        
        /// <summary>
        /// Get all customers By BranchId
        /// </summary>
        [HttpGet("Customers/{BranchId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetAllCustomers>))]
        public async Task<IActionResult> GetCustomers(string BranchId)
        {
            var getAllCustomerByBranchIdQuery = new GetAllCustomerByBranchIdQuery {BranchId=BranchId };
            var result = await _mediator.Send(getAllCustomerByBranchIdQuery);
            return Ok(result);
        }
        /// <summary>
        /// Retrieves all customers for a given branch ID where the Matricule is not null or empty.
        /// </summary>
        [HttpGet("customers-of-branch-filetered-by-matricule/{branchid}")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerLightDto>))]
        public async Task<IActionResult> GetCustomersWithMatriculeByBranchQuery(string branchid)
        {
            var getAllCustomerByBranchIdQuery = new GetCustomersWithMatriculeByBranchQuery { BranchId=branchid };
            var result = await _mediator.Send(getAllCustomerByBranchIdQuery);
            return Ok(result);
        }
        //
        /// <summary>
        /// Get customer by telephone number
        /// </summary>
        [HttpGet("Customers/TelephoneNumber/{msisdn}")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerDto>))]
        public async Task<IActionResult> GetCustomerByMsisdnQuery(string msisdn)
        {
            var getAllCustomerByBranchIdQuery = new GetCustomerByMsisdnQuery { TelephoneNumber = msisdn };
            var result = await _mediator.Send(getAllCustomerByBranchIdQuery);
            return Ok(result);
        }
        

        /// <summary>
        /// Endpoint to search and retrieve paginated customers based on search criteria.
        /// </summary>
        /// <param name="query">The search query containing search criteria and pagination details.</param>
        /// <returns>A paginated list of customers.</returns>
        [HttpGet("Customers/SearchByAnyCriterialQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomersList>))]
        public async Task<IActionResult> SearchByAnyCriterialQuery([FromQuery] CustomerResource customerResource)
        {
            var searchByAnyCriterialQuery = new SearchByAnyCriterialQuery { CustomerResource = customerResource };
            var result = await _mediator.Send(searchByAnyCriterialQuery);
            if (result.Data != null)
            {
                var paginationMetadata = new
                {
                    totalCount = result.Data.TotalCount,
                    pageSize = result.Data.PageSize,
                    skip = result.Data.Skip,
                    totalPages = result.Data.TotalPages
                };
                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            }
            // Assign pagination metadata to the PaginationMetadata property
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Retrieves customer details based on a list of Matricules or CustomerIds.
        /// Supports filtering by either Matricules or CustomerIds, determined by the IsMatricule flag.
        /// </summary>
        /// <param name="customersByMatriculesQuery">
        /// Query object containing the list of Matricules or CustomerIds and the IsMatricule flag.
        /// </param>
        /// <returns>
        /// A formatted response containing a list of customers matching the specified criteria.
        /// The response format can be JSON or XML, as specified by the client.
        /// </returns>
        [HttpGet("Customers/GetCustomersByMatriculesQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerLightDto>))]
        public async Task<IActionResult> GetCustomersByMatriculesQuery([FromQuery] GetCustomersByMatriculesQuery customersByMatriculesQuery)
        {
            // Send the query to the mediator for processing
            var result = await _mediator.Send(customersByMatriculesQuery);

            // Format and return the response based on the result
            return ReturnFormattedResponse(result);
        }



        //CMoneyMembersPagginationQuery
        /// <summary>
        /// Get all CardSignatureSpecimen
        /// </summary>
        [HttpGet("CardSignatureSpecimens")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetCardSignatureSpecimen>))]
        public async Task<IActionResult> GetAllCardSignatureSpecimens()
        {
            var getAllCustomerQuery = new GetAllCardSignatureSpecimenQuery { };
            var result = await _mediator.Send(getAllCustomerQuery);
            return Ok(result);
        }


        /// <summary>
        /// Get all MembershipNextOfKings
        /// </summary>
        [HttpGet("MembershipNextOfKings")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetMembershipNextOfKings>))]
        public async Task<IActionResult> GetAllMembershipNextOfKings()
        {
            var getAllCustomerQuery = new GetAllMembershipNextOfKingsQuery { };
            var result = await _mediator.Send(getAllCustomerQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a customer
        /// </summary>
        [HttpPost("Customer")]
        [Produces("application/json", "application/xml", Type = typeof(CreateCustomer))]
        public async Task<IActionResult> AddCustomer(AddCustomerCommand addCustomerCommand)
        {

            // Validate the command using the validator
            var validator = new AddCustomerCommandValidator();
            var validationResult = validator.Validate(addCustomerCommand);

           // Check if the validation passed
            if (!validationResult.IsValid)
            {
                // Validation failed, log the errors and return a Bad Request with error messages
                return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));
            }

            var result = await _mediator.Send(addCustomerCommand);
            return ReturnFormattedResponse(result);
        } 
        
        
        /// <summary>
        /// Create a salary customer
        /// </summary>
        [HttpPost("SalaryCustomer")]
        [Produces("application/json", "application/xml", Type = typeof(CreateCustomer))]
        public async Task<IActionResult> AddCustomer(AddNewSalaryCustomerCommand addNewSalaryCustomerCommand)
        {
            var result = await _mediator.Send(addNewSalaryCustomerCommand);
            return ReturnFormattedResponse(result);
        }



        /// <summary>
        /// Get all Members by Parameters [ByDate, ByBranch, All]
        /// </summary>
        [HttpPost("Customer/GetAllByParameters")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerListingDto>))]
        public async Task<IActionResult> GetAllCustomerListingsQuery(GetAllCustomerListingsQuery getAllCustomerListingsQuery)
        {
            var result = await _mediator.Send(getAllCustomerListingsQuery);
            return ReturnFormattedResponse(result);
        }

        

        /// <summary>
        /// Create a Card Signature Specimen
        /// </summary>
        [HttpPost("CardSignatureSpecimen")]
        [Produces("application/json", "application/xml", Type = typeof(CreateCardSignatureSpecimen))]
        public async Task<IActionResult> AddCustomer(AddCardSignatureSpecimenCommand addCardSignatureSpecimenCommand)
        {
            var result = await _mediator.Send(addCardSignatureSpecimenCommand);
            return ReturnFormattedResponse(result);
        }



        /// <summary>
        /// Activate Or Deactivate OnLineMobile Packages
        /// </summary>
        [HttpPut("ActivateDeactivateOnLineMobilePackages")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateCustomer))]
        public async Task<IActionResult> AddCustomer(ActivateDeactivateOnLineMobilePackagesCommand deactivateOnLineMobilePackagesCommand)
        {
            var result = await _mediator.Send(deactivateOnLineMobilePackagesCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a Membership Next Of Kings
        /// </summary>
        [HttpPost("MembershipNextOfKings")]
        [Produces("application/json", "application/xml", Type = typeof(CreateMembershipNextOfKings))]
        public async Task<IActionResult> AddCustomer(AddMembershipNextOfKingsCommand addMembershipNextOfKingsCommand)
        {
            var result = await _mediator.Send(addMembershipNextOfKingsCommand);
            return ReturnFormattedResponse(result);
        }

      

        ///// <summary>
        ///// Create a Customer Category
        ///// </summary>
        //[HttpPost("CustomerCategory")]
        //[Produces("application/json", "application/xml", Type = typeof(CreateCustomerCategory))]
        //public async Task<IActionResult> AddCustomer(AddCustomerCategoryCommand addMembershipNextOfKingsMembersCommand)
        //{
        //    var result = await _mediator.Send(addMembershipNextOfKingsMembersCommand);
        //    return ReturnFormattedResponse(result);
        //}


        /// <summary>
        /// Create a Membership Customer
        /// </summary>
        [HttpPost("MembershipCustomer")]
        [Produces("application/json", "application/xml", Type = typeof(CreateMembershipCustomer))]
        public async Task<IActionResult> AddCustomer(AddMembershipCustomerCommand addMembershipCustomerCommand)
        {
            var result = await _mediator.Send(addMembershipCustomerCommand);
            return ReturnFormattedResponse(result);
        }

     
        /// <summary>
        /// Update customer by CustomerId
        /// </summary>
        [HttpPut("Customer/{CustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateCustomer))]
        public async Task<IActionResult> UpdateCustomer(string CustomerId, UpdateCustomerCommand updateCustomerCommand)
        {
            // Validate the command using the validator
            var validator = new UpdateCustomerCommandValidator();
            var validationResult = validator.Validate(updateCustomerCommand);

            // Check if the validation passed
            if (!validationResult.IsValid)
            {
                // Validation failed, log the errors and return a Bad Request with error messages
                return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));
            }
            updateCustomerCommand.CustomerId = CustomerId;
            var result = await _mediator.Send(updateCustomerCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Update customer matricule by CustomerId
        /// </summary>
        [HttpPut("Customer/Matricule/{CustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateCustomer))]
        public async Task<IActionResult> UpdateCustomerMatricule(string CustomerId, UpdateCustomerMatriculeCommand updateCustomerCommand)
        {
          
            updateCustomerCommand.CustomerId = CustomerId;
            var result = await _mediator.Send(updateCustomerCommand);
            return ReturnFormattedResponse(result);
        }
        
        
        /// <summary>
        /// Update customer DateOfBirth by CustomerId
        /// </summary>
        [HttpPut("Customer/DateOfBirth/{CustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateCustomerDateBirthDto))]
        public async Task<IActionResult> UpdateCustomer(string CustomerId, UpdateCustomerDateOfBirthCommand updateCustomerCommand)
        {
          
            updateCustomerCommand.CustomerId = CustomerId;
            var result = await _mediator.Send(updateCustomerCommand);
            return ReturnFormattedResponse(result);
        }
        
        /// <summary>
        /// Update customer Secret by CustomerId
        /// </summary>
        [HttpPut("Customer/CustomerSecret/{CustomerId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateCustomer))]
        public async Task<IActionResult> UpdateCustomerSecret(string Id, UpdateCustomerSecretCommand updateCustomerSecretCommand)
        {
            updateCustomerSecretCommand.CustomerId = Id;
            var result = await _mediator.Send(updateCustomerSecretCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete customer by CustomerId
        /// </summary>
        [HttpDelete("Customer/{CustomerId}")]
        public async Task<IActionResult> DeleteCustomer(string Id)
        {
            var deleteCustomerCommand = new DeleteCustomerCommand { Id = Id };
            var result = await _mediator.Send(deleteCustomerCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Change customer PIN
        /// </summary>
        [HttpPut("Customer/Change/Pin")]
        [Produces("application/json", "application/xml", Type = typeof(ChangePinCommand))]
        public async Task<IActionResult> ChangeCustomerPin(ChangePinCommand changePinCommand)
        {
            var result = await _mediator.Send(changePinCommand);
            return ReturnFormattedResponse(result);
        }  
        
        
        /// <summary>
        /// Change Customer Membership Status
        /// </summary>
        [HttpPut("Customer/Change/Membership/Status")]
        [Produces("application/json", "application/xml", Type = typeof(CreateCustomer))]
        public async Task<IActionResult> ChangeMembershipStatus(ChangeMembershipStatusCommand changePinCommand)
        {
            var result = await _mediator.Send(changePinCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get customer secret by CustomerId
        /// </summary>
        [HttpGet("Customer/secret/{CustomerId}", Name = "GetCustomerSecret")]
        [Produces("application/json", "application/xml", Type = typeof(GetCustomerSecret))]
        public async Task<IActionResult> GetCustomerSecret(string CustomerId)
        {
            var getCustomerSecretByPhoneQuery = new GetCustomerSecretByPhoneQuery { CustomerId = CustomerId };
            var result = await _mediator.Send(getCustomerSecretByPhoneQuery);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Reset customer PIN
        /// </summary>
        [HttpPut("Customer/Pin/Reset/{phone}")]
        [Produces("application/json", "application/xml", Type = typeof(CreateCustomer))]
        public async Task<IActionResult> ResetCustomerPin(string phone, ResetCustomerPinCommand resetCustomerPinCommand)
        {
            resetCustomerPinCommand.Phone=phone;
            var result = await _mediator.Send(resetCustomerPinCommand);
            return ReturnFormattedResponse(result);
        }



        /// <summary>
        /// Get all Question
        /// </summary>
        [HttpGet("Questions")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetQuestion>))]
        public async Task<IActionResult> GetQuestions()
        {
            var getAllQuestionsQuery = new GetAllQuestionsQuery { };
            var result = await _mediator.Send(getAllQuestionsQuery);
            return Ok(result);
        } 
        
        
        /// <summary>
        /// Get all Categories
        /// </summary>
        [HttpGet("Categories")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetAllCustomerCategory>))]
        public async Task<IActionResult> GetAllCustomerCategory()
        {
            var getAllCustomerCategoryQuery = new GetAllCustomerCategoryQuery { };
            var result = await _mediator.Send(getAllCustomerCategoryQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create Question
        /// </summary>
        [HttpPost("CreateQuestion")]
        [Produces("application/json", "application/xml", Type = typeof(CreateQuestion))]
        public async Task<IActionResult> AddQuestion(AddQuestionCommand resetCustomerPinCommand)
        {
            var result = await _mediator.Send(resetCustomerPinCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete Questions by QuestionId
        /// </summary>
        [HttpDelete("Questions/{QuestionId}")]
        public async Task<IActionResult> DeleteQuestion(string QuestionId)
        {
            var deleteQuestionCommand = new DeleteQuestionCommand { QuestionId = QuestionId };
            var result = await _mediator.Send(deleteQuestionCommand);
            return ReturnFormattedResponse(result);
        }
    }
}


