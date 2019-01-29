using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBV3Spike
{
    public interface ITodoRepository
    {
        Task<Todo> CreateItemAsync(string partitionKey, Todo item);
        Task<IEnumerable<Todo>> QueryWithUserNameAsync(string userName);
    }

    public class TodoRepository : GenericRepository<Todo>, ITodoRepository
    {
        public TodoRepository(CosmosContainerResponse containerResponse) : this(containerResponse, "/UserName")
        {

        }
        public TodoRepository(CosmosContainerResponse containerResponse, string partitionKey) : base(containerResponse, partitionKey)
        {
        }

        public async Task<IEnumerable<Todo>> QueryWithUserNameAsync(string userName)
        {

            var query = new CosmosSqlQueryDefinition("SELECT * FROM Todo t WHERE t.UserName = @userName")
                .UseParameter("@userName", userName);
            var iterator = ContainerResponse.Container.Items.CreateItemQuery<Todo>(query, partitionKey:userName);
            var result = new List<Todo>();
            while (iterator.HasMoreResults)
            {
                result.AddRange((await iterator.FetchNextSetAsync()));
            }

            return result;
        }
    }
}
