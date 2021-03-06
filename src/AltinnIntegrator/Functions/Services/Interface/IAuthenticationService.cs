using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AltinnIntegrator.Functions.Services.Interfaces
{
    public interface IAuthenticationService
    {

        /// <summary>
        /// Methods that return Altinn token. If not cached it will log the solution
        /// in to MaskinPorten and then exchange the Maskinporten token to an Altinn token.
        /// </summary>
        /// <returns></returns>
        Task<string> GetAltinnToken();
    }
}
