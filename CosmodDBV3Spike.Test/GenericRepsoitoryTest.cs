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

            fixture.InputTodo = new Todo
            {
                UserName = "Tsuyoshi Ushio",
                TaskName = "Tidy up my bedroom",
                Type = "household",
                IsSparkJoy = true
            };
            fixture.ExpectedTodo = fixture.InputTodo.Clone();
            fixture.ExpectedTodo.Id = "foo";
            fixture.ExpectedPartitionKey = "/TaskName";
            fixture.SetUp();

            var repository = new GenericRepository<Todo>(fixture.Container, fixture.ExpectedPartitionKey);

            var actualTodo = await repository.CreateItemAsync(fixture.InputTodo);
            Assert.Equal(fixture.ExpectedTodo, actualTodo);
        }

        private class TodoFixture
        {
            private Mock<ICosmosContainerWrapper> _containerMock;
            public ICosmosContainerWrapper Container => this._containerMock.Object;
            public Todo ExpectedTodo { get; set; }
            public Todo InputTodo { get; set; }
            public string ExpectedPartitionKey { get; set; }

            public void SetUp()
            {
                this._containerMock = new Mock<ICosmosContainerWrapper>();
                var expectedCosmosItemResponseMock = new Mock<CosmosItemResponse<Todo>>();
                expectedCosmosItemResponseMock.Setup(p => p.Resource).Returns(ExpectedTodo);
                expectedCosmosItemResponseMock.Setup(p => p.StatusCode).Returns(HttpStatusCode.OK);

                _containerMock.Setup(p =>
                        p.CreateItemAsync<Todo>(ExpectedPartitionKey, InputTodo, null, default(CancellationToken)))
                    .Returns(Task.FromResult(expectedCosmosItemResponseMock.Object));

            }
        }

        public class Todo
        {
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
            public string UserName { get; set; }
            public string TaskName { get; set; }
            public string Type { get; set; }
            public Boolean IsSparkJoy { get; set; }

            public Todo Clone()
            {
                return new Todo()
                {
                    Id = this.Id,
                    UserName = this.UserName,
                    TaskName = this.TaskName,
                    Type = this.Type,
                    IsSparkJoy = this.IsSparkJoy
                };
            }
        }
    }
}
