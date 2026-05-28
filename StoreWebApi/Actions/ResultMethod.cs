using Microsoft.AspNetCore.Mvc;
using StoreService.ResultPattern;

namespace StoreWebApi.Actions
{
    public static class ResultMethod
    {
        public static IActionResult ToActionResult<T>(
            this ResultResponse<T> Result,
            ControllerBase controller)
        {
            if (Result.Success)
            {
                return controller.Ok(Result);
            }

            return Result.ErrorType switch
            {
                ErrorTypes.NotFound =>controller.NotFound(Result.Error),
                ErrorTypes.BadRequest =>controller.BadRequest(Result.Error),
                //ErrorTypes.Unauthorized =>controller.Unauthorized(result.ErrorMessage),
                //ErrorTypes.Forbidden =>controller.Forbid(),
                _ =>controller.BadRequest(Result.Error)
            };
        }
    }
}
