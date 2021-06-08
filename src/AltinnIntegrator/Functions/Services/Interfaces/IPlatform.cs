using System;
using System.IO;
using System.Threading.Tasks;

namespace AltinnIntegrator.Services.Interface
{
    /// <summary>
    /// Interface for data handling
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        /// Gets the data as is.
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the app.</param>
        /// <param name="app">Application identifier which is unique within an organisation.</param>
        /// <param name="instanceOwnerPartyId">The instance owner id</param>
        /// <param name="instanceGuid">The instanceid</param>
        /// <param name="dataId">the data id</param>
        Task<Stream> GetBinaryData(string org, string app, int instanceOwnerPartyId, Guid instanceGuid, Guid dataId);
    }
}
