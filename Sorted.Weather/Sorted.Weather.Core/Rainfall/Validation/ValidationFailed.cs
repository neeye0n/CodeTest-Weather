using FluentValidation.Results;

namespace Sorted.Weather.Core.Rainfall.Validation
{
    public record ValidationFailed(IEnumerable<ValidationFailure> errors)
    {
        public ValidationFailed(ValidationFailure error) : this([error])
        {
        }
    }
}
