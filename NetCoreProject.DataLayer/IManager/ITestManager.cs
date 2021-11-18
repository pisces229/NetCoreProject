using NetCoreProject.DataLayer.Model.Test;
using NetCoreProject.Domain.Entity;
using NetCoreProject.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreProject.DataLayer.IManager
{
    public interface ITestManager
    {
        Task Run();
        Task<CommonQueryPageResultModel<TestManagerQueryDto>> QueryGrid(TestManagerQueryGridModel model, CommonPageModel pageModel);
        Task<IEnumerable<TestManagerQueryDto>> QueryWhere(TestManagerQueryModel model);
        Task<Test> QueryByRow(int model);
        Task<IEnumerable<Test>> QueryByRows(IEnumerable<int> model);
        Task Insert(Test model);
        Task InsertRange(IEnumerable<Test> model);
        Task Update(Test model);
        Task UpdateRange(IEnumerable<Test> model);
        Task Delete(Test model);
        Task DeleteRange(IEnumerable<Test> model);
    }
}
