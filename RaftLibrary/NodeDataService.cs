using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaftLibrary
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class NodeDataService
    {
        private readonly HttpClient _httpClient;

        public NodeDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<NodeData?> GetNodeDataAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:8080/nodeData");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<NodeData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching node data: {ex.Message}");
            }

            return null;
        }
    }
}
