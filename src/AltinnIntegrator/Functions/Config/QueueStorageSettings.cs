namespace AltinnIntegrator.Functions.Config
{
    /// <summary>
    /// Configuration object used to hold settings for the queue storage.
    /// </summary>
    public class QueueStorageSettings
    {
        /// <summary>
        /// ConnectionString for the storage account
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the queue to push incomming events to.
        /// </summary>
        public string InboundQueueName { get; set; }

        /// <summary>
        /// Name of the queue to push confirmation
        /// </summary>
        public string ConfirmationQueueName { get; set; }

        /// <summary>
        /// Indicated if events should be pushed to queue.
        /// </summary>
        public bool EnablePushToQueue { get; set; }
    }
}
