using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBV3Spike
{
    public interface ICosmosContainerWrapper
    {
        Task<CosmosItemResponse<T>> CreateItemAsync<T>( // NOTE: CosmosItemResponse can Mock it. 
            object partitionKey,
            T item,
            CosmosItemRequestOptions requestOptions = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public class CosmosContainerWrapper : ICosmosContainerWrapper
    {
        private CosmosContainer Container { get; set; }
        
        public CosmosContainerWrapper(CosmosContainer container)
        {
            this.Container = container;
        }

        public virtual Task<CosmosItemResponse<T>> CreateItemAsync<T>( // NOTE: CosmosItemResponse can Mock it. 
            object partitionKey,
            T item,
            CosmosItemRequestOptions requestOptions = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.Container.Items.CreateItemAsync(partitionKey, item, requestOptions, cancellationToken);          
        }


    }
}
