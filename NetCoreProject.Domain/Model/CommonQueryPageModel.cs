
namespace NetCoreProject.Domain.Model
{
    public class CommonQueryPageModel<T> where T : class
    {
        public T Data { get; set; }
        public CommonPageModel Page { get; set; }
    }
}
