using System.Text.Json.Serialization;

namespace Shared
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductSortingOption
    {
        NameAsc = 1,
        NameDesc = 2,
        PriceAsc = 3,
        PriceDesc = 4,
    }
}