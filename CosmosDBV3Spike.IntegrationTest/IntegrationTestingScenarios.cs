using System;
using System.Net;
using System.IO;
using System.Linq;
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
                Id = "1", // Is it necessary? If not, it throws error {"Errors":["The input name 'null' is invalid. Ensure to provide a unique non-empty string less than '255' characters."]}
                TaskName = "Clean up my bedroom",
                UserName = "Tsuyoshi Ushio",
                Type = "Household",
                IsSparkJoy = true
            };
            var todo02 = new Todo
            {
                Id = "2",
                TaskName = "Paperwork",
                UserName = "Tsuyoshi Ushio",
                Type = "Work",
                IsSparkJoy = false
            };
            var todo03 = new Todo
            {
                Id = "3",
                TaskName = "Programming",
                UserName = "Osvaldo",
                Type = "Work",
                IsSparkJoy = true
            };
            await repository.CreateItemAsync(todo01.UserName, todo01);
            await repository.CreateItemAsync(todo02.UserName, todo02);
            await repository.CreateItemAsync(todo03.UserName, todo03);

            // Query records

            var results = await repository.QueryWithUserNameAsync("Tsuyoshi Ushio");
            Assert.Equal(2, results.Count());
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
            services.AddSingleton<IDatabaseContext>(new DatabaseContext(database));
            // I need specify the PartitionKey here, however, I need to add it inside the TodoRepository constructor. It is weird.
            var cosmosContainer = await database.Containers.CreateContainerIfNotExistsAsync("Todo", "/UserName");
            
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
