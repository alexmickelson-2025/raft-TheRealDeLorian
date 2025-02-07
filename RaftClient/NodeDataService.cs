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
            this.urls = Environment.GetEnvironmentVariable("NODE_URLS").Split(',');
        }

        public async Task<NodeData> GetDataFromApi1()
        {
            return await _httpClient.GetFromJsonAsync<NodeData>(urls[0] + "/nodeData");
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
