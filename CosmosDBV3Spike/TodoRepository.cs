using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBV3Spike
{
    public interface ITodoRepository
    {
        Task<Todo> CreateItemAsync(Todo item);
      //  Task<IEnumerable<Todo>> QueryWithUserName(string userName);
    }

    public class TodoRepository : GenericRepository<Todo>, ITodoRepository
    {
        public TodoRepository(CosmosContainerResponse containerResponse) : this(containerResponse, "/TaskName")
        {

        }
        public TodoRepository(CosmosContainerResponse containerResponse, string partitionKey) : base(containerResponse, partitionKey)
        {
        }

        //public Task<IEnumerable<Todo>> QueryWithUserName(string userName)
        //{
            
        //    var query = new CosmosSqlQueryDefinition("SELECT * FROM Todo t WHERE f.UserName = @userName")
        //        .UseParameter("@userName", userName);
        //   ContainerResponse.Container.Items.
        //}
    }
}
