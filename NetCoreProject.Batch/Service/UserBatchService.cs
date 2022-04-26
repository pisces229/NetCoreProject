using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using System;

namespace NetCoreProject.Batch.Service
{
    public class UserBatchService : IUserService
    {
        private CommonUserModel _commonUserModel;
        public UserBatchService()
        {
            _commonUserModel = new CommonUserModel()
            {
                Proid = Environment.GetEnvironmentVariable("NETOCRESAMPLEPROJECT_BATCH_PROID"),
                Userid = Environment.GetEnvironmentVariable("NETOCRESAMPLEPROJECT_BATCH_PROID"),
                Username = Environment.GetEnvironmentVariable("NETOCRESAMPLEPROJECT_BATCH_PROID")
            };
        }
        public string Proid() => _commonUserModel.Proid;
        public string Userid() => _commonUserModel.Userid;
        public string Username() => _commonUserModel.Username;

    }
}
