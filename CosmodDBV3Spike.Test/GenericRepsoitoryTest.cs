using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Azure.Cosmos;
using System.Threading;
using CosmosDBV3Spike;
using Newtonsoft.Json;

namespace CosmosDBV3Spike.Test
{
    public class GenericRepsoitoryTest
    {
        // This test doesn't work because of theh lack of the testability.
        //[Fact]
        //public async Task CreateContainerAsync()
        //{
        //    var cosmosDatabaseMock = new Mock<CosmosDatabase>();
        //    var cosmosContainersMock = new Mock<CosmosContainers>();

        //    var containerResponseMock = new Mock<CosmosContainerResponse>();
        //    var containerMock = new Mock<CosmosContainer>();
        //    var container = containerMock.Object;
        //    containerResponseMock.Setup(p => p.Container).Returns(container);

        //    cosmosContainersMock.Setup(p => p.CreateContainerIfNotExistsAsync("Todo", "/UserName", null, null, default(CancellationToken)))
        //        .ReturnsAsync(containerResponseMock.Object);

        //    cosmosDatabaseMock.Setup(p => p.Containers).Returns(cosmosContainersMock.Object);
        //    var repository = new GenericRepository<Todo>(cosmosDatabaseMock.Object);
        //    Assert.Equal(container, repository.container);
        //    cosmosContainersMock.Verify(p => p.CreateContainerIfNotExistsAsync("Todo", "/UserName", null, null, default(CancellationToken)));

        //}

        [Fact]
        public async Task CreateItemAsyncTest()
        {
            var fixture = new TodoFixture();
            fixture.SetUp();

            var repository = new GenericRepository<Todo>(fixture.Container, fixture.ExpectedPartitionKey);
            repository.CreateItemAsyncFunc = fixture.CreateItemAsyncFunc;
            var actualTodo = await repository.CreateItemAsync(fixture.InputTodo.UserName, fixture.InputTodo);
            Assert.Equal(fixture.ExpectedTodo, actualTodo);
        }

        [Fact]
        public async Task CreateItemAsyncWithException()
        {
            var fixture = new TodoFixture();
            fixture.SetupWithException();
            var repository = new GenericRepository<Todo>(fixture.Container, fixture.ExpectedPartitionKey);
            repository.CreateItemAsyncFunc = fixture.CreateItemAsyncFunc;
            var ex =
                await Assert.ThrowsAsync<ArgumentException>(async () => await repository.CreateItemAsync(fixture.InputTodo.UserName, fixture.InputTodo));
            Assert.Equal("Can not create the item. responseCode: InternalServerError", ex.Message);
        }

        private class TodoFixture
        {
            private Mock<CosmosContainerResponse> _containerResponseMock;
            public CosmosContainerResponse Container => this._containerResponseMock.Object;
            public Todo ExpectedTodo { get; set; }
            public Todo InputTodo { get; set; }
            public string ExpectedPartitionKey { get; set; }

            // Can't mock the method. Instead, inject Func.
            public Func<object, Todo, Task<CosmosItemResponse<Todo>>> CreateItemAsyncFunc { get; set; }

            public void SetUp()
            {
                this.BasicSetup();
                this._containerResponseMock = new Mock<CosmosContainerResponse>();
               

                var expectedCosmosItemResponseMock = new Mock<CosmosItemResponse<Todo>>();
                expectedCosmosItemResponseMock.Setup(p => p.Resource).Returns(ExpectedTodo);
                expectedCosmosItemResponseMock.Setup(p => p.StatusCode).Returns(HttpStatusCode.OK);

                CreateItemAsyncFunc = (partitionKey, item) => { return Task.FromResult(expectedCosmosItemResponseMock.Object); };
            }

            public void SetupWithException()
            {
                this.BasicSetup();
                this._containerResponseMock = new Mock<CosmosContainerResponse>();
                var expectedCosmosItemResponseMock = new Mock<CosmosItemResponse<Todo>>();
                expectedCosmosItemResponseMock.Setup(p => p.Resource).Returns(ExpectedTodo);
                expectedCosmosItemResponseMock.Setup(p => p.StatusCode).Returns(HttpStatusCode.InternalServerError);

                CreateItemAsyncFunc = (partitionKey, item) => { return Task.FromResult(expectedCosmosItemResponseMock.Object); };

            }

            public void BasicSetup()
            {
                this.InputTodo = new Todo
                {
                    UserName = "Tsuyoshi Ushio",
                    TaskName = "Tidy up my bedroom",
                    Type = "household",
                    IsSparkJoy = true
                };
                this.ExpectedTodo = this.InputTodo.Clone();
                this.ExpectedTodo.Id = "foo";
                this.ExpectedPartitionKey = "/TaskName";
            }
        }


    }
}
