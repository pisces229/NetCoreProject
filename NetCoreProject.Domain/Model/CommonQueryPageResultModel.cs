using System.Collections.Generic;

namespace NetCoreProject.Domain.Model
{
    public class CommonQueryPageResultModel<T> where T : class
    {
        public CommonPageModel Page { get; set; }
        public List<T> Data { get; set; }
    }
}