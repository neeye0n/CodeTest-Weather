using FluentValidation;
using Sorted.Weather.Core.Rainfall.Queries;

namespace Sorted.Weather.Core.Rainfall.Validation
{
    public class GetRainfallReadingsValidator : AbstractValidator<GetRainfallReadingsQuery>
    {
        public GetRainfallReadingsValidator()
        {
            RuleFor(prop => prop.Count)
                .Must(BeValidInteger).WithMessage("Count must be a valid number.")
                .Must(BeInRange).WithMessage("Count must be between 1 and 100.");
        }

        private bool BeValidInteger(string count)
        {
            return int.TryParse(count, out _);
        }

        private bool BeInRange(string count)
        {
            if (int.TryParse(count, out int countValue))
            {
                return countValue >= 1 && countValue <= 100;
            }
            return false;
        }
    }
}