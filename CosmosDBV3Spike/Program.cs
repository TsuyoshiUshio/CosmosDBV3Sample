using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CosmosDBV3Spike
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            new Program().ExecuteAsync().GetAwaiter().GetResult();
           
        }

        private async Task ExecuteAsync()
        {
            var client = new CosmosClient("ConnectionString");
            var database = await client.Databases.CreateDatabaseIfNotExistsAsync("SomeDB");
            var container = await database.Database.Containers.CreateContainerIfNotExistsAsync("Todo", "/Name");            
        }
    }
}
