using System;
using System.Threading.Tasks;
using Altinn.Platform.Events.Functions.Models;
using AltinnIntegrator.Functions.Services.Interface;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AltinnIntegrator
{
    public  class EventsConfirmation
    {
        private readonly IAltinnApp _altinnApp;

        public EventsConfirmation(IAltinnApp altinnApp)
        {
            _altinnApp = altinnApp;
        }

        [FunctionName("EventsConfirmation")]
        public async Task Run([QueueTrigger("events-confirmation", Connection = "QueueStorage")]string item, ILogger log)
        {
            CloudEvent cloudEvent = System.Text.Json.JsonSerializer.Deserialize<CloudEvent>(item);

            await _altinnApp.AddCompleteConfirmation(cloudEvent.Source.AbsoluteUri);

            log.LogInformation($"C# Queue trigger function processed: {item}");
        }
    }
}
