using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AltinnIntegrator.Functions.Services.Interfaces
{
    public interface IStorage
    {
        public Task SaveBlob(string name, string data);

        public Task<long> UploadFromStreamAsync(Stream stream, string fileName);
    }
}
