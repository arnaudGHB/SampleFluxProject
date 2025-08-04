// Source: Votre modèle standard et intouchable.
using System;
using System.Collections.Generic;

namespace CBS.UserServiceManagement.Helper
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        // Constructeurs privés pour forcer l'utilisation des méthodes factory
        private ServiceResponse(T data) { Data = data; }
        private ServiceResponse() { }
        private ServiceResponse(int statusCode, List<string> errors) { StatusCode = statusCode; Errors = errors; }
        private ServiceResponse(int statusCode, T data) { StatusCode = statusCode; Data = data; }
        private ServiceResponse(int statusCode, string errorMessage) { StatusCode = statusCode; Errors = new List<string> { errorMessage }; }
        private ServiceResponse(int statusCode, T data, string message) { StatusCode = statusCode; Data = data; Message = message; }
        private ServiceResponse(int statusCode, T data, string message, string description, string status) { StatusCode = statusCode; Data = data; Message = message; Description = description; Status = status; }
        private ServiceResponse(int statusCode, string message, string description, string status) { StatusCode = statusCode; Message = message; Description = description; Status = status; }

        public bool Success => Errors == null || Errors.Count == 0;

        public static ServiceResponse<T> ReturnException(Exception ex)
        {
            return new ServiceResponse<T>(500, $"An unexpected fault happened. Error: {ex.Message}", "Failed with Internal Server Error", "FAILED");
        }
        public static ServiceResponse<T> ReturnFailed(int statusCode, List<string> errors)
        {
            return new ServiceResponse<T>(statusCode, errors);
        }
        public static ServiceResponse<T> ReturnFailed(int statusCode, string errorMessage)
        {
            return new ServiceResponse<T>(statusCode, errorMessage);
        }
        public static ServiceResponse<T> ReturnSuccess()
        {
            return new ServiceResponse<T>(default(T));
        }
        public static ServiceResponse<T> ReturnSuccess(string message)
        {
            return new ServiceResponse<T>(200, message, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> ReturnResultWith200(T data)
        {
            return new ServiceResponse<T>(200, data, string.Empty, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> ReturnResultWith200(T data, string message)
        {
            return new ServiceResponse<T>(200, data, message, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> ReturnResultWith201(T data, string message)
        {
            return new ServiceResponse<T>(201, data, message, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> ReturnResultWith204(T data)
        {
            return new ServiceResponse<T>(204, data, string.Empty, "Transaction was successful", "SUCCESS");
        }
        public static ServiceResponse<T> Return500()
        {
            return new ServiceResponse<T>(500, "An unexpected fault happened. Try again later.");
        }
        public static ServiceResponse<T> Return500(Exception ex, string message = null)
        {
            return new ServiceResponse<T>(500, $"An unexpected fault happened. {message} Error: {ex.Message}", "Failed with Internal Server Error", "FAILED");
        }
        public static ServiceResponse<T> Return500(string message)
        {
            return new ServiceResponse<T>(500, $"{message}", "Failed with Internal Server Error", "FAILED");
        }
        public static ServiceResponse<T> Return409(string message)
        {
            return new ServiceResponse<T>(409, message, "Failed, Record already exists. Operation was cancelled", "FAILED");
        }
        public static ServiceResponse<T> Return409()
        {
            return new ServiceResponse<T>(409, "Record already exists", "Failed, Record already exists. Operation was cancelled", "FAILED");
        }
        public static ServiceResponse<T> Return422(string message)
        {
            return new ServiceResponse<T>(422, new List<string> { "Unprocessable Entity", message, "Failed. Process terminated.", "FAILED" });
        }
        public static ServiceResponse<T> Return404()
        {
            return new ServiceResponse<T>(404, "Record not found", "Failed. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return404(string message)
        {
            return new ServiceResponse<T>(404, message, "Failed. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return401(string message)
        {
            return new ServiceResponse<T>(401, message, "Failed.User not authourized Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return403(string message)
        {
            return new ServiceResponse<T>(403, message, "Operation is forbidden. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return403(T data)
        {
            return new ServiceResponse<T>(403, "Operation is forbidden.", "Failed. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return403()
        {
            return new ServiceResponse<T>(403, "Operation is forbidden.", "Failed. Process terminated.", "FAILED");
        }
        public static ServiceResponse<T> Return403(T data, string message)
        {
            return new ServiceResponse<T>(403, data, message, "Operation is forbidden.", "FAILED");
        }
    }

    public class ResponseObject
    {
        public object Data { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; } = 200;
        public string StatusDescription { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

        public ResponseObject(object data = null, int statusCode = 200, string description = null, string message = null, string status = null, List<string> errors = null)
        {
            Data = data;
            StatusCode = statusCode;
            StatusDescription = description;
            Status = status;
            Message = message;
            Errors = errors;
        }
    }
}