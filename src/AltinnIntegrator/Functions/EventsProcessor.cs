using System;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Platform.Events.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public class EventsProcessor
    {
        [FunctionName("EventsProcessor")]
        public async Task Run([QueueTrigger("events-inbound", Connection = "QueueStorage")] string item, ILogger log)
        {
            CloudEvent cloudEvent = JsonSerializer.Deserialize<CloudEvent>(item);
            if(ShouldProcessEvent(cloudEvent))
            {

            }
        }


        private bool ShouldProcessEvent(CloudEvent cloudEvent)
        {
            return true;
        }

        private async Task GetDataForInstance(string source)
        {

        }

    }
}
