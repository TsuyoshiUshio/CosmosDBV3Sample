using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBV3Spike
{
    public interface IGenericRepository<T>
    {
        Task<T> CreateItemAsync(T item);
    }

    public class GenericRepository<T> : IGenericRepository<T>
    {
        protected CosmosContainerResponse ContainerResponse { get; set; }
        protected string PartitionKey { get; set; }

        public GenericRepository(CosmosContainerResponse containerResponse, string partitionKey)
        {
            this.ContainerResponse = containerResponse;
            this.PartitionKey = partitionKey;
            this.SetupDefaultFunction();
        }

        private void SetupDefaultFunction()
        {
            CreateItemAsyncFunc = (partitionKey, item) => this.ContainerResponse.Container.Items.CreateItemAsync(this.PartitionKey, item);
        }

        internal Func<object, T, Task<CosmosItemResponse<T>>> CreateItemAsyncFunc { get; set; }

        public async Task<T> CreateItemAsync(T item)
        {
            var response = await this.CreateItemAsyncFunc(this.PartitionKey, item);
            if (!response.StatusCode.IsSuccessStatusCode())
            {
                // TODO I'm not sure if throwing ArgumentException is good or not
                throw new ArgumentException($"Can not create the item. responseCode: {response.StatusCode}");
            }
            return response.Resource;
        }


    }
}
