using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankMGT.MediatR.Validations;
using CBS.Customer.MEDIATR;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;



namespace CBS.CUSTOMER.API.Controllers
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
    public class EmployeeController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the CustomerDashboardController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

   
        /// <summary>
        /// Create a Employee
        /// </summary>
        [HttpPost("Employee")]
        [Produces("application/json", "application/xml", Type = typeof(CreateEmployee))]
        public async Task<IActionResult> AddCustomer(AddEmployeeCommand addEmployeeCommand)
        {

            var result = await _mediator.Send(addEmployeeCommand);
            return ReturnFormattedResponse(result);
        }  
        
        
        /// <summary>
        /// Create a Employee Department
        /// </summary>
        [HttpPost("Employee/Department")]
        [Produces("application/json", "application/xml", Type = typeof(CreateDepartment))]
        public async Task<IActionResult> AddCustomerDepartment(AddDepartmentCommand addDepartmentCommand)
        {

            var result = await _mediator.Send(addDepartmentCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Create a Employee Leave
        /// </summary>
        [HttpPost("Employee/EmployeeLeave")]
        [Produces("application/json", "application/xml", Type = typeof(CreateEmployeeLeave))]
        public async Task<IActionResult> AddEmployeeLeave(AddEmployeeLeaveCommand addEmployeeLeaveCommand)
        {

            var result = await _mediator.Send(addEmployeeLeaveCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Create a Employee Training
        /// </summary>
        [HttpPost("Employee/EmployeeTraining")]
        [Produces("application/json", "application/xml", Type = typeof(CreateEmployeeTraining))]
        public async Task<IActionResult> AddEmployeeTraining(AddEmployeeTrainingCommand addEmployeeLeaveCommand)
        {

            var result = await _mediator.Send(addEmployeeLeaveCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Create a Employee Job Title
        /// </summary>
        [HttpPost("Employee/JobTitle")]
        [Produces("application/json", "application/xml", Type = typeof(CreateJobTitle))]
        public async Task<IActionResult> AddJobTitle(AddJobTitleCommand addEmployeeLeaveCommand)
        {

            var result = await _mediator.Send(addEmployeeLeaveCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Create a Employee Leave Type
        /// </summary>
        [HttpPost("Employee/LeaveType")]
        [Produces("application/json", "application/xml", Type = typeof(CreateLeaveType))]
        public async Task<IActionResult> AddLeaveType(AddLeaveTypeCommand addLeaveTypeCommand)
        {

            var result = await _mediator.Send(addLeaveTypeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update Employee by EmployeeId
        /// </summary>
        [HttpPut("Employee/{EmployeeId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateEmployee))]
        public async Task<IActionResult> UpdateEmployee(string EmployeeId, UpdateEmployeeCommand updateEmployeeCommand)
        {
          
            updateEmployeeCommand.EmployeeId = EmployeeId;
            var result = await _mediator.Send(updateEmployeeCommand);
            return ReturnFormattedResponse(result);
        }
        
        
        /// <summary>
        /// Update Employee by EmployeeId
        /// </summary>
        [HttpPut("Employee/EmployeeLeave/{EmployeeLeaveId}")]
        [Produces("application/json", "application/xml", Type = typeof(UpdateEmployeeLeave))]
        public async Task<IActionResult> UpdateCustomer(string EmployeeLeaveId, UpdateEmployeeLeaveCommand updateEmployeeLeaveCommand)
        {

            updateEmployeeLeaveCommand.EmployeeLeaveId = EmployeeLeaveId;
            var result = await _mediator.Send(updateEmployeeLeaveCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Get all Employees
        /// </summary>
        [HttpGet("Employees")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetAllEmployees>))]
        public async Task<IActionResult> GetAllEmployees()
        {
            var getAllEmployeeQuery = new GetAllEmployeeQuery();
            var result = await _mediator.Send(getAllEmployeeQuery);
            return Ok(result);
        } 
        
        
        /// <summary>
        /// Get all Employee Leaves
        /// </summary>
        [HttpGet("Employee/EmployeeLeaves")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetEmployeeLeave>))]
        public async Task<IActionResult> GetAllEmployeeLeaves()
        {
            var getAllEmployeeLeave = new GetAllEmployeeLeaveQuery();
            var result = await _mediator.Send(getAllEmployeeLeave);
            return Ok(result);
        }


        /// <summary>
        /// Get all Employee Departments
        /// </summary>
        [HttpGet("Employee/Departments")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetDepartment>))]
        public async Task<IActionResult> GetAllDepartment()
        {
            var getAllDepartmentQuery = new GetAllDepartmentQuery();
            var result = await _mediator.Send(getAllDepartmentQuery);
            return Ok(result);
        }


        /// <summary>
        /// Get all Employee JobTitles
        /// </summary>
        [HttpGet("Employee/JobTitles")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetJobTitle>))]
        public async Task<IActionResult> GetAllJobTitle()
        {
            var getAllJobTitle = new GetAllJobTitleQuery();
            var result = await _mediator.Send(getAllJobTitle);
            return Ok(result);
        }



        /// <summary>
        /// Get all Employee LeaveTypes
        /// </summary>
        [HttpGet("Employee/LeaveTypes")]
        [Produces("application/json", "application/xml", Type = typeof(List<GetLeaveType>))]
        public async Task<IActionResult> GetAllLeaveType()
        {
            var getAllLeaveType = new GetAllLeaveTypeQuery();
            var result = await _mediator.Send(getAllLeaveType);
            return Ok(result);
        }


        /// <summary>
        /// Get Employees by EmployeeId
        /// </summary>
        [HttpGet("Employee/{EmployeeId}", Name = "GetEmployee")]
        [Produces("application/json", "application/xml", Type = typeof(GetEmployee))]
        public async Task<IActionResult> GetEmployee(string EmployeeId)
        {
            var getEmployeeQuery = new GetEmployeeQuery { EmployeeId = EmployeeId };
            var result = await _mediator.Send(getEmployeeQuery);
            return ReturnFormattedResponse(result);
        }  
        
        
        
        /// <summary>
        /// Get EmployeeLeave by EmployeeLeaveId
        /// </summary>
        [HttpGet("Employee/EmployeeLeave/{EmployeeLeaveId}", Name = "GetEmployeeLeave")]
        [Produces("application/json", "application/xml", Type = typeof(GetEmployeeLeave))]
        public async Task<IActionResult> GetEmployeeLeave(string EmployeeLeaveId)
        {
            var getEmployeeLeaveQuery = new GetEmployeeLeaveQuery { EmployeeLeaveId = EmployeeLeaveId };
            var result = await _mediator.Send(getEmployeeLeaveQuery);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Delete Employee by CustomerId
        /// </summary>
        [HttpDelete("Employee/{CustomerId}")]
        public async Task<IActionResult> DeleteCustomer(string Id)
        {
            var deleteEmployeeCommand = new DeleteEmployeeCommand { Id = Id };
            var result = await _mediator.Send(deleteEmployeeCommand);
            return ReturnFormattedResponse(result);
        }
    }
}


