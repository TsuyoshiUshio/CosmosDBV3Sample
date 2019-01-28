using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDBV3Spike
{
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
