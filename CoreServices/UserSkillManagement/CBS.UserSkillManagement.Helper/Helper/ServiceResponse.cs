using System;
using System.Collections.Generic;

namespace CBS.UserSkillManagement.Helper
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ServiceResponse<T> SuccessResponse(T data, string message = null)
        {
            return new ServiceResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResponse<T> ErrorResponse(string message, List<string> errors = null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }

    public class PaginatedResponse<T> : ServiceResponse<List<T>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public static PaginatedResponse<T> Create(
            List<T> items,
            int pageNumber,
            int pageSize,
            int totalRecords,
            string message = null)
        {
            return new PaginatedResponse<T>
            {
                Success = true,
                Message = message,
                Data = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }
    }
}
