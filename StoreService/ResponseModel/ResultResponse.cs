using Microsoft.AspNetCore.Http;
using StoreDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.ResponseModel
{
    public class ResultResponse<T>
    {
        public string Error { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
        public int StatusCode { get; set; }
        public ErrorTypes ErrorType { get; set; }

        public static ResultResponse<T> Pass(T value, int statusCode)
        {
            return new ResultResponse<T>
            {
                Success = true,
                Result = value,
                StatusCode = statusCode
            };
        }
        public static ResultResponse<T> Fail(string error, ErrorTypes errorType, int statusCode)
        {
            return new ResultResponse<T>
            {
                Success = false,
                ErrorType = errorType,
                Error = error,
                StatusCode = statusCode
            };
        }
    }
}
