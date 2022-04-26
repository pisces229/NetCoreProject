using NetCoreProject.DataLayer.Model.Test;
using NetCoreProject.DataLayer.IManager;
using NetCoreProject.Domain.DatabaseContext;
using NetCoreProject.Domain.Enum;
using NetCoreProject.Domain.Entity;
using NetCoreProject.Domain.IService;
using NetCoreProject.Domain.Model;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetCoreProject.Domain.Util;

namespace NetCoreProject.DataLayer.Manager
{
    public class TestManager : ITestManager
    {
        private readonly ILogger<TestManager> _logger;
        private readonly DefaultDbContext _defaultDbContext;
        private readonly IDapperService<DefaultDbContext> _defaultDapperService;
        private readonly SqlUtil _sqlUtil;
        public TestManager(ILogger<TestManager> logger,
            DefaultDbContext defaultDbContext,
            IDapperService<DefaultDbContext> defaultDapperService,
            SqlUtil sqlUtil)
        {
            _logger = logger;
            _defaultDbContext = defaultDbContext;
            _defaultDapperService = defaultDapperService;
            _sqlUtil = sqlUtil;
        }
        #region Run
        public async Task Run()
        {
            await RunCatch();
            await RunLinq();
            await RunSqlQuery();
            await RunSqlQueryByPage();
            await RunDynamic();
            await RunLinqQuery();
        }
        private async Task RunCatch()
        {
            await Task.FromResult("");
        }
        private async Task RunLinq()
        {
            await _defaultDbContext.TestDetail.Where(w => w.ROW == 1).ToListAsync();
        }
        private async Task RunSqlQuery()
        {
            {
                var sql = "SELECT * FROM TESTDETAIL (NOLOCK) WHERE BORN_DATE >= @BORN_DATE AND NAME IN @NAME ORDER BY BORN_DATE";
                var parameters = new DynamicParameters();
                parameters.Add("BORN_DATE", "2000/01/01");
                parameters.Add("NAME", new[] { "C", "D", "E" });
                //parameters.Add("NAME", new List<string>(){ "C", "D", "E" });
                var entity = await _defaultDapperService.SqlQuery<TestDetail>(sql, parameters);
                _logger.LogInformation(JsonSerializer.Serialize(entity));
            }
            {
                var sql = new StringBuilder();
                var dynamicParameters = new DynamicParameters();
                sql.Append(@"
                    SELECT *
                    FROM TESTDETAIL (NOLOCK)
                ");
                var condition = new StringBuilder();
                _sqlUtil.WhereAndCondition(condition, dynamicParameters,
                    "BORN_DATE", OperatorEnum.GREATER_THAN_EQUAL, "2000/01/01");
                _sqlUtil.WhereAndCondition(condition, dynamicParameters,
                    "NAME", OperatorEnum.IN, new[] { "C", "D", "E" });
                //SqlStringUtil.WhereAndCondition(condition, dynamicParameters,
                //    "NAME", SqlStringUtil.OPER.IN, new List<string>() { "C", "D", "E" });
                sql.Append(condition.ToString());
                sql.Append(@"
                    ORDER BY BORN_DATE
                ");
                var entity = await _defaultDapperService.SqlQuery<TestDetail>(sql.ToString(), dynamicParameters);
                _logger.LogInformation(JsonSerializer.Serialize(entity));
            }
        }
        private async Task RunSqlQueryByPage()
        {
            {
                var sql = new StringBuilder();
                var dynamicParameters = new DynamicParameters();
                sql.Append(@"
                    SELECT *
                    FROM TESTDETAIL (NOLOCK)
                ");
                var condition = new StringBuilder();
                _sqlUtil.WhereAndCondition(condition, dynamicParameters,
                    "BORN_DATE", OperatorEnum.GREATER_THAN_EQUAL, "2000/01/01");
                _sqlUtil.WhereAndCondition(condition, dynamicParameters,
                    "NAME", OperatorEnum.IN, new[] { "C", "D", "E" });
                //SqlStringUtil.WhereAndCondition(condition, dynamicParameters,
                //    "NAME", SqlStringUtil.OPER.IN, new List<string>() { "C", "D", "E" });
                sql.Append(condition.ToString());
                var orderby = "ORDER BY BORN_DATE";
                var queryCount = _sqlUtil.CreateSelectCount(sql.ToString());
                var querySql = sql.ToString() + orderby;
                var entityPageModel = new CommonPageModel()
                {
                    PageNo = 1,
                    PageSize = 1,
                    TotalCount = 0
                };
                var dtoPageModel = new CommonPageModel()
                {
                    PageNo = 2,
                    PageSize = 1,
                    TotalCount = 0
                };
                var entity = await _defaultDapperService.SqlQueryByPage<TestDetail>(queryCount, querySql, dynamicParameters, entityPageModel);
                _logger.LogInformation(JsonSerializer.Serialize(entity));
            }
        }
        private async Task RunDynamic()
        {
            {
                var testSelect = new List<string>()
                {
                    nameof(Test.ROW), nameof(Test.NAME)
                };
                var condition = new StringBuilder();
                var dynamicParameters = new DynamicParameters();
                _sqlUtil.WhereAndCondition(condition, dynamicParameters,
                    nameof(Test.ROW), OperatorEnum.EQUAL, "1");
                _sqlUtil.WhereAndCondition(condition, dynamicParameters,
                    nameof(Test.NAME), OperatorEnum.EQUAL, "123");
                var sql = _sqlUtil.CreateSelect<Test>(testSelect, condition.ToString(), "ROW");
                await _defaultDapperService.SqlQuery<Test>(sql, dynamicParameters);
            }
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add(nameof(Test.NAME), "A");
                dynamicParameters.Add(nameof(Test.MAKE_DATE), DateTime.Now);
                //dynamicParameters.Add(nameof(Test.SALE_AMT), 0);
                dynamicParameters.Add(nameof(Test.SALE_DATE), DateTime.Now);
                dynamicParameters.Add(nameof(Test.TAX), 10);
                dynamicParameters.Add(nameof(Test.REMARK), "HELLO");
                var sql = _sqlUtil.CreateInsert<Test>(dynamicParameters);
                await _defaultDapperService.Execute(sql, dynamicParameters);
            }
            {
                var condition = new StringBuilder();
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add(nameof(Test.TAX), 11);
                dynamicParameters.Add(nameof(Test.REMARK), "HELLO");
                _sqlUtil.WhereAndCondition(condition, dynamicParameters,
                    nameof(Test.ROW), OperatorEnum.EQUAL, "1");
                //_sqlUtil.WhereAndCondition(condition, dynamicParameters,
                //    nameof(Test.NAME), OperatorEnum.EQUAL, "123");
                var sql = _sqlUtil.CreateUpdate<Test>(dynamicParameters, condition.ToString());
                await _defaultDapperService.Execute(sql, dynamicParameters);
            }
            {
                var condition = new StringBuilder();
                var dynamicParameters = new DynamicParameters();
                _sqlUtil.WhereAndCondition(condition, dynamicParameters,
                    nameof(Test.ROW), OperatorEnum.EQUAL, "1");
                //_sqlUtil.WhereAndCondition(condition, dynamicParameters,
                //    nameof(Test.NAME), OperatorEnum.EQUAL, "123");
                var sql = _sqlUtil.CreateDelete<Test>(condition.ToString());
                await _defaultDapperService.Execute(sql, dynamicParameters);
            }
        }
        private async Task RunLinqQuery()
        {
            // not work
            // https://docs.microsoft.com/zh-tw/ef/core/querying/client-eval

            // inner join
            var innerJoinResult = (
                from m in _defaultDbContext.TestMaster
                join d in _defaultDbContext.TestDetail
                    on new { A = m.ID, B = "A1" } equals new { A = d.MASTER_ID, B = d.NAME }
                select new
                {
                    MASTER_ID = m.ID,
                    MASTER_NAME = m.NAME,
                    DETAIL_ID = d.ID,
                    DETAIL_NAME = d.NAME
                }
            ).ToList();
            // not work
            //(
            //    from m in _defaultDbContext.TestMaster
            //    join d in _defaultDbContext.TestDetail
            //        on new { A = m.ID, B = "A1" } equals new { A = d.MASTER_ID, B = d.NAME }
            //        into subGrp
            //    select new
            //    {
            //        MASTER_ID = m.ID,
            //        MASTER_NAME = m.NAME,
            //        DETAIL = subGrp
            //    }
            //).ToList();

            // left join
            var leftJoinResult = (
                from m in _defaultDbContext.TestMaster
                join d in _defaultDbContext.TestDetail
                    on new { A = m.ID, B = "A1" } equals new { A = d.MASTER_ID, B = d.NAME }
                    into subGrp
                from s in subGrp.DefaultIfEmpty()
                select new
                {
                    MASTER_ID = m.ID,
                    MASTER_NAME = m.NAME,
                    DETAIL_ID = s.ID,
                    DETAIL_NAME = s.NAME
                }
            ).ToList();
            // not work
            //(
            //    from m in _defaultDbContext.TestMaster
            //    join d in _defaultDbContext.TestDetail
            //        on new { A = m.ID, B = "A1" } equals new { A = d.MASTER_ID, B = d.NAME } 
            //        into subGrp
            //    select new
            //    {
            //        MASTER_ID = m.ID,
            //        MASTER_NAME = m.NAME,
            //        DETAIL = subGrp
            //    }
            //).ToList();
            _defaultDbContext.TestMaster.Join(
                _defaultDbContext.TestDetail.AsEnumerable(),
                m => new { A = m.ID, B = "A1" },
                d => new { A = d.ID, B = d.NAME },
                (m, d) => new
                {
                    MASTER_ID = m.ID,
                    MASTER_NAME = m.NAME,
                    DETAIL_ID = d.ID,
                    DETAIL_NAME = d.NAME
                })
                .ToList();
            _defaultDbContext.TestMaster.GroupJoin(
                _defaultDbContext.TestDetail.AsEnumerable(),
                m => new { A = m.ID, B = "A1" },
                d => new { A = d.ID, B = d.NAME },
                (m, d) => new { master = m, detail = d })
               .SelectMany(
                    t => t.detail.DefaultIfEmpty(),
                    (m, d) => new
                    {
                        MASTER_ID = m.master.ID,
                        MASTER_NAME = m.master.NAME,
                        DETAIL_ID = d.ID,
                        DETAIL_NAME = d.NAME
                    })
               .ToList();
            //var result = _defaultDbContext.TestMaster
            //    .Include(i => i.TestDetail)
            //    .ToList();
            //result.ForEach(m =>
            //{
            //    _logger.LogInformation($"TestMaster.ROW:" + m.ROW);
            //    _logger.LogInformation($"TestMaster.ID:" + m.ID);
            //    _logger.LogInformation($"TestMaster.NAME:" + m.NAME);
            //    m.TestDetail.ToList().ForEach(d =>
            //    {
            //        _logger.LogInformation($"TestDetail.ROW:" + d.ROW);
            //        _logger.LogInformation($"TestDetail.MASTER_ID:" + d.MASTER_ID);
            //        _logger.LogInformation($"TestDetail.ID:" + d.ID);
            //        _logger.LogInformation($"TestDetail.NAME:" + d.NAME);
            //    });
            //});
            _defaultDbContext.ChangeTracker.QueryTrackingBehavior
                = QueryTrackingBehavior.TrackAll;
            var gradeDatas = _defaultDbContext.Grade
                .Include(c => c.Students)
                .ToList();
            var studentDatas = _defaultDbContext.Student
                .Include(c => c.Grade)
                    .ThenInclude(c => c.Students)
                .Include(c => c.Address)
                .ToList();
            var studentAddressDatas = _defaultDbContext.StudentAddress
                .Include(c => c.Student)
                    .ThenInclude(c => c.Grade)
                        .ThenInclude(c => c.Students)
                .ToList();
            await Task.FromResult("");
        }
        #endregion
        public async Task<CommonQueryPageResultModel<TestManagerQueryDto>> QueryGrid(TestManagerQueryGridModel model, CommonPageModel pageModel)
        {
            var result = new CommonQueryPageResultModel<TestManagerQueryDto>();
            {
                // Sql
                var sql = new StringBuilder();
                var dynamicParameters = new DynamicParameters();
                sql.Append(@"
                    SELECT 
                        ROW,
                        NAME,
                        MAKE_DATE,
                        SALE_AMT,
                        TAX,
                        REMARK
                    FROM TEST (NOLOCK)
                ");
                var condition = new StringBuilder();
                _sqlUtil.WhereAndCondition(condition, dynamicParameters,
                    "NAME", OperatorEnum.EQUAL, model.NAME);
                sql.Append(condition.ToString());
                var orderby = "ORDER BY ROW";
                var queryCount = _sqlUtil.CreateSelectCount(sql.ToString());
                var querySql = sql.ToString() + orderby;
                var commonQueryPageResult = await _defaultDapperService.SqlQueryByPage<TestManagerQueryDto>(queryCount, querySql, dynamicParameters, pageModel);
                result.Page = commonQueryPageResult.Page;
                result.Data = commonQueryPageResult.Data;
            }
            //{
            //    // Linq
            //    var queryable = _defaultDbContext.Test.AsQueryable();
            //    if (!string.IsNullOrEmpty(model.NAME))
            //    {
            //        queryable = queryable.Where(w => w.NAME == model.NAME);
            //    }
            //    pageModel.TotalCount = await queryable.CountAsync();
            //    var dto = await queryable.OrderBy(o => o.ROW)
            //        .Skip((pageModel.PageNo - 1) * pageModel.PageSize)
            //        .Take(pageModel.PageSize)
            //        .Select(s => new TestManagerQueryDto()
            //        {
            //            ROW = s.ROW,
            //            NAME = s.NAME,
            //            MAKE_DATE = s.MAKE_DATE,
            //            SALE_AMT = s.SALE_AMT,
            //            TAX = s.ROW,
            //            REMARK = s.REMARK
            //        }).ToListAsync();
            //    result.Page = pageModel;
            //    result.Data = dto;
            //}
            return result;
        }
        public async Task<IEnumerable<TestManagerQueryDto>> QueryWhere(TestManagerQueryModel model)
        {
            var query = _defaultDbContext.Test.AsQueryable();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model.NAME))
                {
                    query = query.Where(w => w.NAME == model.NAME);
                }
                if (model.MAKE_DATE.Year > 1911)
                {
                    query = query.Where(w => w.MAKE_DATE == model.MAKE_DATE);
                }
                if (model.SALE_AMT != null)
                {
                    query = query.Where(w => w.SALE_AMT == model.SALE_AMT);
                }
                if (model.SALE_DATE != null)
                {
                    query = query.Where(w => w.SALE_DATE == model.SALE_DATE);
                }
            }
            var result =  await query
                .Select(s => new TestManagerQueryDto()
                {
                    ROW = s.ROW,
                    NAME = s.NAME,
                    MAKE_DATE = s.MAKE_DATE,
                    SALE_AMT = s.SALE_AMT,
                    SALE_DATE = s.SALE_DATE,
                    TAX = s.TAX,
                    REMARK = s.REMARK
                }).ToListAsync();
            return result;
        }
        public async Task<Test> QueryByRow(int model)
            => await _defaultDbContext.Test.Where(w => w.ROW == model).Select(s => s).SingleOrDefaultAsync();
        public async Task<IEnumerable<Test>> QueryByRows(IEnumerable<int> model)
            => await _defaultDbContext.Test.Where(w => model.Contains(w.ROW)).Select(s => s).ToListAsync();
        public async Task Insert(Test model) 
            => await _defaultDbContext.Test.AddAsync(model);
        public async Task InsertRange(IEnumerable<Test> model)
            => await _defaultDbContext.Test.AddRangeAsync(model);
        public async Task Update(Test model)
            => await Task.Run(() => _defaultDbContext.Test.Update(model));
        public async Task UpdateRange(IEnumerable<Test> model)
            => await Task.Run(() => _defaultDbContext.Test.UpdateRange(model));
        public async Task Delete(Test model)
            => await Task.Run(() => _defaultDbContext.Test.Remove(model));
        public async Task DeleteRange(IEnumerable<Test> model)
            => await Task.Run(() => _defaultDbContext.Test.RemoveRange(model));
    }
}
