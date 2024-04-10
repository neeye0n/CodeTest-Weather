using MediatR;
using Sorted.Weather.Core.Response;

namespace Sorted.Weather.Core.Rainfall.Queries
{
    public record GetRainfallReadingsQuery(string StationId, string Count = "10") : IRequest<GetRainfallReadingResponse>;
}
