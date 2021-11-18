using NetCoreProject.Domain.Model;
using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.BusinessLayer.Model.Test;
using NetCoreProject.Batch.Model.Test;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Text.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using NetCoreProject.Domain.DatabaseContext;

namespace NetCoreProject.Batch.Runner
{
    public class TestRunner : IRunner
    {
        private readonly ILogger<TestRunner> _logger;
        private readonly IMapper _mapper;
        private readonly DefaultDbContext _defaultDbContext;
        public TestRunner(ILogger<TestRunner> logger,
            IMapper mapper,
            DefaultDbContext defaultDbContext)
        {
            _logger = logger;
            _mapper = mapper;
            _defaultDbContext = defaultDbContext;
        }
        public async Task Execute()
        {
            await Task.FromResult("");
            _logger.LogInformation("Excute Start");

            _defaultDbContext.Test.Add(new Domain.Entity.Test()
            {
                NAME = "ABC",
                MAKE_DATE = DateTime.Now,
                REMARK = "1234567890"
            });
            await _defaultDbContext.SaveChangesAsync();

            _logger.LogInformation("Excute End");
        }
        private void TestMapper()
        {
            var mapperPerson1 = new TestRunnerMapperPerson1()
            {
                //Name = new MapperName1()
                //{
                //    FirstName = "Wu",
                //    LastName = "GG"
                //},
                Age = 22,
                Address = "QAZWSXEDC"
            };
            var mapperPerson2 = _mapper.Map<TestRunnerMapperPerson1, TestRunnerMapperPerson2>(mapperPerson1);
            var mapperPerson3 = _mapper.Map<TestRunnerMapperPerson1, TestRunnerMapperPerson3>(mapperPerson1);
            //target.Age = 20;
            //target.Address = "火星";
            //target.Name.FirstName = "Lu";
            //target.Name.LastName = "CC";
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
            };
            _logger.LogInformation(JsonSerializer.Serialize(mapperPerson1, jsonSerializerOptions));
            _logger.LogInformation(JsonSerializer.Serialize(mapperPerson2, jsonSerializerOptions));
            _logger.LogInformation(JsonSerializer.Serialize(mapperPerson3, jsonSerializerOptions));
        }
    }
}

