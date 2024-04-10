using OneOf;
using OneOf.Types;
using Sorted.Weather.Core.Models;
using Sorted.Weather.Core.Rainfall.Validation;

namespace Sorted.Weather.Core.Response
{
    [GenerateOneOf]
    public partial class GetRainfallReadingResponse : OneOfBase<RainfallApiResponse, NotFound, ValidationFailed, Error>;
}
