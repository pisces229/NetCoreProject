using NetCoreProject.Domain.Model;
using NetCoreProject.DataLayer.IManager;
using NetCoreProject.DataLayer.Model.Default;
using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.BusinessLayer.Model.Default;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace NetCoreProject.BusinessLayer.Logic
{
    public class DefaultLogic : IDefaultLogic
    {
        private readonly ILogger<DefaultLogic> _logger;
        private readonly IMapper _mapper;
        private readonly IDefaultManager _defaultManager;
        public DefaultLogic(ILogger<DefaultLogic> logger,
            IMapper mapper,
            IDefaultManager defaultManager)
        {
            _logger = logger;
            _mapper = mapper;
            _defaultManager = defaultManager;
        }
        public async Task<CommonApiResultModel<DefaultLogicRunOutputModel>> Run(DefaultLogicRunInputModel model)
        {
            var result = new CommonApiResultModel<DefaultLogicRunOutputModel>()
            {
                Success = true,
                Message = "Complete",
                Data = new DefaultLogicRunOutputModel()
            };
            var runResult = await _defaultManager.Run(_mapper.Map<DefaultLogicRunInputModel, DefaultManagerRunModel>(model));
            result.Data = _mapper.Map<DefaultManagerRunDto, DefaultLogicRunOutputModel>(runResult);
            return result;
        }
    }
}
