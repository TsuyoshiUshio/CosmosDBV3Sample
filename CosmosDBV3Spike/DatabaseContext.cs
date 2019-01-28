using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBV3Spike
{
    public interface IDatabaseContext
    {
        Task<CosmosContainerResponse> CreateContainerIfNotExistsAsync<T>(string id, string partitionKey);
    }

    public class DatabaseContext : IDatabaseContext
    {
        private CosmosDatabase Database { get; set; }

        public DatabaseContext(CosmosDatabase database)
        {
            this.Database = database;
        }

        public async Task<CosmosContainerResponse> CreateContainerIfNotExistsAsync<T>(string id, string partitionKey)
        {
            return await this.Database.Containers.CreateContainerIfNotExistsAsync(id, partitionKey);
        }

    }
}
