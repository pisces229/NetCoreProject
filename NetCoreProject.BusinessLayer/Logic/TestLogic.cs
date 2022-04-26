using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.Entity;
using NetCoreProject.Domain.Model;
using NetCoreProject.Domain.Util;
using NetCoreProject.BusinessLayer.Model.Test;
using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.DataLayer.Model.Test;
using NetCoreProject.DataLayer.IManager;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetCoreProject.Domain.IService;

namespace NetCoreProject.BusinessLayer.Logic
{
    public class TestLogic : ITestLogic
    {
        private readonly ILogger<TestLogic> _logger;
        private readonly IDefaultDataProtector _defaultDataProtector;
        private readonly DefaultDbContext _defaultDbContext;
        private readonly IMapper _mapper;
        private readonly ITestManager _testManager;
        private readonly ConfigurationUtil _configurationUtil;
        public TestLogic(ILogger<TestLogic> logger,
            IDefaultDataProtector defaultDataProtector,
            DefaultDbContext defaultDbContext,
            IMapper mapper,
            ITestManager testManager,
            ConfigurationUtil configurationUtil)
        {
            _logger = logger;
            _defaultDataProtector = defaultDataProtector;
            _defaultDbContext = defaultDbContext;
            _mapper = mapper;
            _testManager = testManager;
            _configurationUtil = configurationUtil;
        }
        public async Task Run()
        {
            var sourceString = "1";
            _logger.LogInformation($"Source:[{ sourceString }]");
            var protectString = _defaultDataProtector.Protect(sourceString);
            _logger.LogInformation($"protectString:[{ protectString }]");
            var unprotectString = _defaultDataProtector.Unprotect(protectString);
            _logger.LogInformation($"unprotectString:[{ unprotectString }]");
            await _testManager.Run();
        }
        public async Task<CommonApiResultModel<CommonQueryPageModel<List<TestLogicQueryGridOutputModel>>>> QueryGrid(CommonQueryPageModel<TestLogicQueryGridInputModel> model)
        {
            var result = new CommonApiResultModel<CommonQueryPageModel<List<TestLogicQueryGridOutputModel>>>()
            {
                Success = true,
                Message = "Complete",
                Data = new CommonQueryPageModel<List<TestLogicQueryGridOutputModel>>()
            };
            var commonQueryPageResult = await _testManager.QueryGrid(_mapper.Map<TestLogicQueryGridInputModel, TestManagerQueryGridModel>(model.Data), model.Page);
            result.Data.Page = commonQueryPageResult.Page;
            result.Data.Data = _mapper.Map<List<TestManagerQueryDto>, List<TestLogicQueryGridOutputModel>>(commonQueryPageResult.Data);
            result.Data.Data.ToList().ForEach(f => f.ROW = _defaultDataProtector.Protect(f.ROW));
            return result;
        }
        public async Task<CommonApiResultModel<IEnumerable<TestLogicOutputModel>>> QueryWhere(TestLogicInputModel model)
        {
            var result = new CommonApiResultModel<IEnumerable<TestLogicOutputModel>>()
            {
                Success = true,
                Message = "Complete"
            };
            var dto = await _testManager.QueryWhere(_mapper.Map<TestLogicInputModel, TestManagerQueryModel>(model));
            result.Data = _mapper.Map<IEnumerable<TestManagerQueryDto>, IEnumerable<TestLogicOutputModel>>(dto);
            result.Data.ToList().ForEach(f => f.ROW = _defaultDataProtector.Protect(f.ROW));
            return result;
        }
        public async Task<CommonApiResultModel<TestLogicOutputModel>> QueryByRow(string model)
        {
            var result = new CommonApiResultModel<TestLogicOutputModel>()
            {
                Success = true,
                Message = "Complete"
            };
            var data = await _testManager.QueryByRow(Convert.ToInt32(_defaultDataProtector.Unprotect(model)));
            if (data != null)
            {
                result.Data = _mapper.Map<Test, TestLogicOutputModel>(data);
                result.Data.ROW = _defaultDataProtector.Protect(result.Data.ROW);
            }
            return result;
        }
        public async Task<CommonApiResultModel<string>> Insert(TestLogicInputModel model)
        {
            var result = new CommonApiResultModel<string>()
            {
                Success = true,
                Message = "Complete"
            };
            var data = new Test()
            {
                NAME = model.NAME,
                MAKE_DATE = model.MAKE_DATE,
                SALE_AMT = model.SALE_AMT,
                SALE_DATE = model.SALE_DATE,
                TAX = model.TAX,
                REMARK = model.REMARK,
                UPDATE_USER_ID = model.UPDATE_USER_ID,
                UPDATE_PROG_CD = model.UPDATE_PROG_CD,
                UPDATE_DATE_TIME = model.UPDATE_DATE_TIME
            };
            await _testManager.Insert(data);
            await _defaultDbContext.SaveChangesAsync();
            return result;
        }
        public async Task<CommonApiResultModel<string>> InsertRange(IEnumerable<TestLogicInputModel> model)
        {
            var result = new CommonApiResultModel<string>()
            {
                Success = true,
                Message = "Complete"
            };
            var data = model.Select(s => new Test() {
                NAME = s.NAME,
                MAKE_DATE = s.MAKE_DATE,
                SALE_AMT = s.SALE_AMT,
                SALE_DATE = s.SALE_DATE,
                TAX = s.TAX,
                REMARK = s.REMARK,
                UPDATE_USER_ID = s.UPDATE_USER_ID,
                UPDATE_PROG_CD = s.UPDATE_PROG_CD,
                UPDATE_DATE_TIME = s.UPDATE_DATE_TIME
            });
            await _testManager.InsertRange(data);
            await _defaultDbContext.SaveChangesAsync();
            return result;
        }
        public async Task<CommonApiResultModel<string>> Update(TestLogicInputModel model)
        {
            var result = new CommonApiResultModel<string>()
            {
                Success = true,
                Message = "Complete"
            };
            var data = await _testManager.QueryByRow(Convert.ToInt32(_defaultDataProtector.Unprotect(model.ROW)));
            if (data != null)
            {
                data.NAME = model.NAME;
                data.MAKE_DATE = model.MAKE_DATE;
                data.SALE_AMT = model.SALE_AMT;
                data.SALE_DATE = model.SALE_DATE;
                data.TAX = model.TAX;
                data.REMARK = model.REMARK;
                data.UPDATE_USER_ID = model.UPDATE_USER_ID;
                data.UPDATE_PROG_CD = model.UPDATE_PROG_CD;
                data.UPDATE_DATE_TIME = model.UPDATE_DATE_TIME;
                await _testManager.Update(data);
                await _defaultDbContext.SaveChangesAsync();
            }
            return result;
        }
        public async Task<CommonApiResultModel<string>> UpdateRange(IEnumerable<TestLogicInputModel> model)
        {
            var result = new CommonApiResultModel<string>()
            {
                Success = true,
                Message = "Complete"
            };
            var data = model.Select(s => new Test()
            {
                ROW = Convert.ToInt32(_defaultDataProtector.Unprotect(s.ROW)),
                NAME = s.NAME,
                MAKE_DATE = s.MAKE_DATE,
                SALE_AMT = s.SALE_AMT,
                SALE_DATE = s.SALE_DATE,
                TAX = s.TAX,
                REMARK = s.REMARK,
                UPDATE_USER_ID = s.UPDATE_USER_ID,
                UPDATE_PROG_CD = s.UPDATE_PROG_CD,
                UPDATE_DATE_TIME = s.UPDATE_DATE_TIME
            });
            await _testManager.UpdateRange(data);
            await _defaultDbContext.SaveChangesAsync();
            return result;
        }
        public async Task<CommonApiResultModel<string>> Delete(string model)
        {
            var result = new CommonApiResultModel<string>()
            {
                Success = true,
                Message = "Complete"
            };
            var data = await _testManager.QueryByRow(Convert.ToInt32(_defaultDataProtector.Unprotect(model)));
            if (data != null)
            {
                await _testManager.Delete(data);
                await _defaultDbContext.SaveChangesAsync();
            }
            return result;
        }
        public async Task<CommonApiResultModel<string>> DeleteRange(IEnumerable<string> model)
        {
            var result = new CommonApiResultModel<string>()
            {
                Success = true,
                Message = "Complete"
            };
            var data = await _testManager.QueryByRows(
                model.Select(s => Convert.ToInt32(_defaultDataProtector.Unprotect(s))));
            if (data.Any())
            {
                await _testManager.DeleteRange(data);
                await _defaultDbContext.SaveChangesAsync();
            }
            return result;
        }
        public async Task<CommonApiResultModel<string>> Upload(TestLogicUploadInputModel model)
        {
            var result = new CommonApiResultModel<string>()
            {
                Success = false,
                Message = "Complete"
            };
            if (model != null && model.UPLOAD_FILE != null && model.UPLOAD_FILE.Length > 0)
            {
                var fileName = model.UPLOAD_FILE.FileName;
                if (!string.IsNullOrEmpty(model.UPLOAD_NAME) && !string.IsNullOrEmpty(model.UPLOAD_TYPE))
                {
                    fileName = $"{model.UPLOAD_NAME}.{model.UPLOAD_TYPE}";
                }
                var savePath = Path.Combine(_configurationUtil.TempPath, fileName);
                using (var stream = File.Create(savePath))
                {
                    await model.UPLOAD_FILE.CopyToAsync(stream);
                }
                result.Success = true;
                result.Message = "Complete";
            }
            else
            {
                result.Message = "Please Select File";
            }
            return result;
        }
        public async Task<CommonApiResultModel<string>> Uploads(IEnumerable<TestLogicUploadInputModel> model)
        {
            var result = new CommonApiResultModel<string>()
            {
                Success = false,
                Message = "Complete"
            };
            if (model != null && model.Any())
            {
                await Task.Run(() =>
                {
                    model.Where(w => w.UPLOAD_FILE != null && w.UPLOAD_FILE.Length > 0)
                        .ToList()
                        .ForEach(f =>
                        {
                            var fileName = f.UPLOAD_FILE.FileName;
                            if (!string.IsNullOrEmpty(f.UPLOAD_NAME) && !string.IsNullOrEmpty(f.UPLOAD_TYPE))
                            {
                                fileName = $"{f.UPLOAD_NAME}.{f.UPLOAD_TYPE}";
                            }
                            var savePath = Path.Combine(_configurationUtil.TempPath, fileName);
                            using (var stream = File.Create(savePath))
                            {
                                f.UPLOAD_FILE.CopyTo(stream);
                            }
                        });
                });
                result.Success = true;
                result.Message = "Complete";
            }
            else
            {
                result.Message = "Please Select File";
            }
            return result;
        }
    }
}
