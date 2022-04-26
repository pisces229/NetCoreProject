using System;

namespace NetCoreProject.Domain.Model
{
    public class CommonTokenModel
    {
        public DateTime Expiration { get; set; }
        public string Username { get; set; }
    }
}
