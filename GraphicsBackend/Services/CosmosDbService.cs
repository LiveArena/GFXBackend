using GraphicsBackend.Models;
using Microsoft.Azure.Cosmos;
using System.Configuration;

namespace GraphicsBackend.Services
{
    public class CosmosDbService<T>: ICosmosDbService<T> where T : class
    {
        private readonly CosmosClient _cosmosClient;
        string _databaseName = "projects";
        public CosmosDbService(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        private Container GetContainer(string containerName)
        {
            return _cosmosClient.GetContainer(_databaseName, containerName);
        }

        public async Task AddAsync(T item, string id, string containerName)
        {
            var container = _cosmosClient.GetContainer(_databaseName, containerName);
            await container.CreateItemAsync(item, new PartitionKey(id));
        }

        public async Task<T> GetAsync(string id, string containerName)
        {
            var container = _cosmosClient.GetContainer(_databaseName, containerName);
            try
            {
                
                ItemResponse<T> response = await container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(string queryString, string containerName)
        {
            var container = _cosmosClient.GetContainer(_databaseName, containerName);
            var query = container.GetItemQueryIterator<T>(new QueryDefinition(queryString));
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }
        public async Task DeleteAsync(string id, string containerName)
        {
            var container = _cosmosClient.GetContainer(_databaseName, containerName);
            await container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }

        public async Task UpdateAsync(string id, T item,string containerName)
        {
            var container = _cosmosClient.GetContainer(_databaseName, containerName);
            await container.UpsertItemAsync(item, new PartitionKey(id));
        }

        
    }

}
