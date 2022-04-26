using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreProject.Domain.IService
{
    public interface ICaptchaCacheService
    {
        Task Add(string key, string value, TimeSpan expiry);
        Task<string> Get(string key);
        Task Remove(string key);
        void Dispose();
    }
}
