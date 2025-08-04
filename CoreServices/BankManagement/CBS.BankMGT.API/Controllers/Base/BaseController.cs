
using CBS.BankMGT.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CBS.BankMGT.API
{
    public class BaseController : ControllerBase
    {
        // Method to format and return API responses.
        // Parameters:
        // - result: ServiceResponse containing data and response details.
        // Returns:
        // - IActionResult representing the formatted API response.
        public IActionResult ReturnFormattedResponse<T>(ServiceResponse<T> result)
        {
            // Create a response object using the provided ServiceResponse data.
            ResponseObject responseObject = new ResponseObject(
                data: result.Data,
                statusCode: result.StatusCode,
                description: result.Description,
                message: result.Message,
                status: result.Status,
                errors: result.Errors
            );

            // Check if the operation was successful.
            if (result.Status == "SUCCESS")
            {
                // Return a 200 OK response with the formatted response object.
                return Ok(responseObject);
            }

            // Return an appropriate status code with the formatted response object.
            return StatusCode(responseObject.StatusCode, responseObject);
        }
    }
}