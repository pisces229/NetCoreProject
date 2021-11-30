using NetCoreProject.Domain.Model;
using NetCoreProject.BusinessLayer.Model.Test;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCoreProject.BusinessLayer.ILogic
{
    public interface ITestLogic
    {
        Task Run();
        Task<CommonApiResultModel<CommonQueryPageModel<List<TestLogicQueryGridOutputModel>>>> QueryGrid(CommonQueryPageModel<TestLogicQueryGridInputModel> model);
        Task<CommonApiResultModel<IEnumerable<TestLogicOutputModel>>> QueryWhere(TestLogicInputModel model);
        Task<CommonApiResultModel<TestLogicOutputModel>> QueryByRow(string model);
        Task<CommonApiResultModel<string>> Insert(TestLogicInputModel model);
        Task<CommonApiResultModel<string>> InsertRange(IEnumerable<TestLogicInputModel> model);
        Task<CommonApiResultModel<string>> Update(TestLogicInputModel model);
        Task<CommonApiResultModel<string>> UpdateRange(IEnumerable<TestLogicInputModel> model);
        Task<CommonApiResultModel<string>> Delete(string model);
        Task<CommonApiResultModel<string>> DeleteRange(IEnumerable<string> model);
        Task<CommonApiResultModel<string>> Upload(TestLogicUploadInputModel model);
        Task<CommonApiResultModel<string>> Uploads(IEnumerable<TestLogicUploadInputModel> model);
    }
}
