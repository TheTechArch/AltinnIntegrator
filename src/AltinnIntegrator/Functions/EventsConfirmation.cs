using System;
using Altinn.Platform.Events.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AltinnIntegrator
{
    public static class EventsConfirmation
    {
        [FunctionName("EventsConfirmation")]
        public static void Run([QueueTrigger("events-confirmation", Connection = "QueueStorage")]string item, ILogger log)
        {
            CloudEvent cloudEvent = System.Text.Json.JsonSerializer.Deserialize<CloudEvent>(item);

            log.LogInformation($"C# Queue trigger function processed: {item}");
        }
    }
}
