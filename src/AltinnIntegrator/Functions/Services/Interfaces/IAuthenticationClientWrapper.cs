using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AltinnIntegrator.Services.Interface
{
    public interface IAuthenticationClientWrapper
    {
        Task<string> ConvertToken(string maskinportenToken);
    }
}
