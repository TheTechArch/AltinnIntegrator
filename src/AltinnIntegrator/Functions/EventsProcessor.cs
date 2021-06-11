using System;
using System.Text.Json;
using System.Threading.Tasks;
using Altinn.Platform.Events.Functions.Models;
using Altinn.Platform.Storage.Interface.Models;
using AltinnIntegrator.Functions.Services.Interface;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AltinnIntegrator.Functions
{
    public class EventsProcessor
    {
        private readonly IAltinnApp _altinnApp;

        public EventsProcessor(IAltinnApp altinnApp)
        {
            _altinnApp = altinnApp;
        }


        [FunctionName("EventsProcessor")]
        public async Task Run([QueueTrigger("events-inbound", Connection = "QueueStorage")] string item, ILogger log)
        {
            CloudEvent cloudEvent = JsonSerializer.Deserialize<CloudEvent>(item);
            if(ShouldProcessEvent(cloudEvent))
            {
               Instance instance = CreateInstanceFromSource(cloudEvent);
               instance = await _altinnApp.GetInstance(instance.AppId, instance.Id);
                foreach(DataElement data in instance.Data)
                {
                    ResourceLinks links = data.SelfLinks;
                }
            }
        }


        private Instance CreateInstanceFromSource(CloudEvent cloudEvent)
        {
            Instance instance = new Instance();
            string[] parts =  cloudEvent.Source.PathAndQuery.Split("/");
            instance.AppId = $"{parts[1]}/{parts[2]}";
            instance.Org = $"{parts[1]}";
            instance.InstanceOwner = new InstanceOwner() { PartyId = parts[4] };
            instance.Id = $"{parts[4]}/{parts[5]}";
            return instance;
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
