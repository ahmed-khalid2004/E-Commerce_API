using DomainLayer.Contracts;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Persistence.Caching
{
    public class UpstashRedisClient : IRedisClient
    {
        private readonly HttpClient _httpClient;

        public UpstashRedisClient(HttpClient httpClient, string restUrl, string restToken)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(restUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", restToken);
        }

        private async Task<JsonElement> ExecuteAsync(object[] command)
        {
            var json = JsonSerializer.Serialize(command);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("", content);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.GetProperty("result").Clone();
        }

        public async Task<string?> GetAsync(string key)
        {
            var result = await ExecuteAsync(new object[] { "GET", key });
            return result.ValueKind == JsonValueKind.Null ? null : result.GetString();
        }

        public async Task SetAsync(string key, string value, TimeSpan? ttl = null)
        {
            if (ttl.HasValue)
                await ExecuteAsync(new object[] { "SET", key, value, "EX", (int)ttl.Value.TotalSeconds });
            else
                await ExecuteAsync(new object[] { "SET", key, value });
        }

        public async Task DeleteAsync(string key)
            => await ExecuteAsync(new object[] { "DEL", key });

        public async Task<List<string>> ScanKeysAsync(string pattern)
        {
            var keys = new List<string>();
            var cursor = "0";

            do
            {
                var result = await ExecuteAsync(new object[] { "SCAN", cursor, "MATCH", pattern, "COUNT", 100 });
                cursor = result[0].GetString()!;
                foreach (var k in result[1].EnumerateArray())
                    keys.Add(k.GetString()!);
            }
            while (cursor != "0");

            return keys;
        }
    }
}