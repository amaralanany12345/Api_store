using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.ResultPattern
{
    public class ResultResponse<T>
    {
        public string Error { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
        public ErrorTypes ErrorType { get; set; }
        
        public static ResultResponse<T> Pass(T value) 
        {
            return new ResultResponse<T>
            {
                Success = true,
                Result=value,
            };
        }
        public static ResultResponse<T> Fail(string error,ErrorTypes errorType)
        {
            return new ResultResponse<T>
            {
                Success = false,
                ErrorType=errorType,
                Error=error,
            };
        }
    }
}
