using CBS.FixedAssetsManagement.Helper;
using Microsoft.AspNetCore.Mvc;

namespace CBS.FixedAssetsManagement.API
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T ApiResponseData { get; set; }
    }

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

        public IActionResult ReturnFormattedResponseObject<T>(ServiceResponse<T> result)
        {
            // Create a response object using the provided ServiceResponse data.
            ApiResponse<T> responseObject = new ApiResponse<T>
            {
                ApiResponseData = result.Data,
                IsSuccess = result.Status == "SUCCESS",

                Message = result.Status == "SUCCESS" ? result.Message : GetMessage(result.Errors)
            };

            // Check if the operation was successful.
            if (result.Status == "SUCCESS")
            {
                // Return a 200 OK response with the formatted response object.
                return Ok(responseObject);
            }
            else
            {
                responseObject.Message = result.Message;
                return BadRequest(responseObject);
            }
        }

        private string GetMessage(List<string> errors)
        {
            string message = "";
            foreach (var error in errors)
                message = message + error + " ";
            return message;
        }

        //public DateTime ConvertStringToDateTime(string dateString)
        //{
        //    CultureInfo culture = new CultureInfo("en-US");

        //    DateTime parsedDate = DateTime.Parse(dateString, culture, DateTimeStyles.None);

        //    return parsedDate;

        //}
    }
}