using System.Threading.Tasks;

namespace NetCoreProject.Batch
{
    public interface IRunner
    {
        Task Execute();
    }
}
