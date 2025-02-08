using RaftLibrary;

namespace RaftClient
{
    public class NodeDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string[] urls;

        public NodeDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            urls = Environment.GetEnvironmentVariable("NODE_URLS").Split(',');
        }

        public async Task<NodeData> GetDataFromApi1()
        {
            // Console.WriteLine("url" + urls[0]);
            return await _httpClient.GetFromJsonAsync<NodeData>(urls[0] + "/nodeData");
        }

        public async Task<RequestAppendEntriesData> SendEntryToApi1(RequestAppendEntriesData data)
        {
            // var response = await _httpClient.PostAsJsonAsync(urls[0] + "/request/appendEntries", data).Result.Content.ReadFromJsonAsync<RequestAppendEntriesData>();
            var response = await _httpClient.PostAsJsonAsync(urls[0] + "/request/appendEntries", data);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("response content " + responseContent);
            return await response.Content.ReadFromJsonAsync<RequestAppendEntriesData>();
        }

        public async Task<NodeData> GetDataFromApi2()
        {
            return await _httpClient.GetFromJsonAsync<NodeData>(urls[1] + "/nodeData");

        }

        public async Task<NodeData> GetDataFromApi3()
        {
            return await _httpClient.GetFromJsonAsync<NodeData>(urls[2] + "/nodeData");

        }
    }
}
