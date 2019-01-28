using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBV3Spike
{
  
    public class GenericRepository<T> 
    {
        private ICosmosContainerWrapper Container { get; set; }
        private string PartitionKey { get; set; }

        public GenericRepository(ICosmosContainerWrapper container, string partitionKey)
        {
            this.Container = container;
            this.PartitionKey = partitionKey;
        }

        public async Task<T> CreateItemAsync(T item)
        {
            var response = await this.Container.CreateItemAsync(this.PartitionKey, item);
            if (!response.StatusCode.IsSuccessStatusCode())
            {
                // TODO I'm not sure if throwing ArgumentException is good or not
                throw new ArgumentException($"Can not create the item. responseCode: {response.StatusCode}");
            }
            return response.Resource;
        }


    }
}
