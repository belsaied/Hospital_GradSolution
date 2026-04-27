using Microsoft.AspNetCore.Mvc;

namespace Hospital_Grad.API.Factories
{
    public static class ApiResponseFactory
    {
        public static IActionResult GenerateApiValidationResponse(ActionContext actionContext)
        {
            var errors = actionContext.ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e =>
                        string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Invalid value provided. Please check the field and try again."
                            : e.ErrorMessage
                    )
                )
                .ToArray();

            var problem = new ProblemDetails
            {
                Title = "Validation Errors",
                Detail = "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { "Errors", errors } }
            };

            return new BadRequestObjectResult(problem);
        }
    }
}