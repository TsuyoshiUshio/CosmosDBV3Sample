using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Xunit;
using Microsoft.Extensions.DependencyInjection;


namespace CosmosDBV3Spike.IntegrationTest
{
    public class IntegrationTestingScenarios
    {
        private  ServiceProvider Provider;
        private static IConfigurationRoot Configuration;

        public IntegrationTestingScenarios()
        {
            // Test Setup
            SetupServiceCollectionAsync().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            // teardown
        }

        [Fact]
        public async Task BasicCRUDScenario()
        {
            // Prerequisite: Start CosmosDB emulator
            // Setup the repository
            var repository = Provider.GetRequiredService<ITodoRepository>();
            // Create records
            var todo01 = new Todo
            {
                TaskName = "Clean up my bedroom",
                UserName = "Tsuyoshi Ushio",
                Type = "Household",
                IsSparkJoy = true
            };
            var todo02 = new Todo
            {
                TaskName = "Paperwork",
                UserName = "Tsuyoshi Ushio",
                Type = "Work",
                IsSparkJoy = false
            };
            var todo03 = new Todo
            {
                TaskName = "Programming",
                UserName = "Osvaldo",
                Type = "Work",
                IsSparkJoy = true
            };
            await repository.CreateItemAsync(todo01);
            await repository.CreateItemAsync(todo02);
            await repository.CreateItemAsync(todo03);

            // Query records

           
            // Update records
            // Delete records
        }

        static IntegrationTestingScenarios()
        {
            SetupConfiguration();
            

        }

        private async Task SetupServiceCollectionAsync()
        {
            var services = new ServiceCollection();
            var client = new CosmosClient(Configuration["CosmosConnectionString"]);
            var databaseResponse = await client.Databases.CreateDatabaseIfNotExistsAsync(Configuration["DatabaseName"]);
            // clear detabase for integration testing. 
            await databaseResponse.Database.DeleteAsync();
            databaseResponse = await client.Databases.CreateDatabaseIfNotExistsAsync(Configuration["DatabaseName"]);

            var database = databaseResponse.Database;
            
            services.AddSingleton<CosmosDatabase>(database);
            services.AddSingleton<IDatabaseContext>();
            // I need specify the PartitionKey here, however, I need to add it inside the TodoRepository constructor. It is weird.
            var cosmosContainer = await database.Containers.CreateContainerIfNotExistsAsync("Todo", "/TaskName");
            
            services.AddSingleton<ITodoRepository>(new TodoRepository(cosmosContainer));
            Provider = services.BuildServiceProvider();
        }

        private static void SetupConfiguration()
        {
            const string APP_SETTINGS = "appSettings.json";
            var builder = new ConfigurationBuilder();
            if (File.Exists(APP_SETTINGS))
            {
                builder.AddJsonFile(APP_SETTINGS);
            }
            else
            {
                Console.WriteLine("appSettings.json not found.");
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
    }
}
