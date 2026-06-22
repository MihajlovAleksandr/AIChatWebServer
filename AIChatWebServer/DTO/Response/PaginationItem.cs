using System.Text.Json.Serialization;

namespace AIChatWebServer.DTO.Response
{
    public sealed record PaginationItem<T>
    (
         [property: JsonPropertyName("data")] IEnumerable<T> Data,
         [property: JsonPropertyName("totalCount")] int TotalCount,
         [property: JsonPropertyName("currentPage")] int CurrentPage,
         [property: JsonPropertyName("totalPages")] int TotalPages,
         [property: JsonPropertyName("pageSize")] int PageSize
    );
}
