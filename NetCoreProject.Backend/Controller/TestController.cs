using NetCoreProject.Domain.Model;
using NetCoreProject.Domain.Util;
using NetCoreProject.BusinessLayer.ILogic;
using NetCoreProject.BusinessLayer.Model.Test;
using NetCoreProject.Backend.Model.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using NetCoreProject.Backend.AuthorizationFilter;
using System.Text;

namespace NetCoreProject.Backend.Controller
{
    //[Authorize]
    //[ServiceFilter(typeof(DefaultAuthorizationFilter))]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly ITestLogic _testLogic;
        private readonly ConfigurationUtil _configurationUtil;
        public TestController(ILogger<TestController> logger,
            ITestLogic testLogic,
            ConfigurationUtil configurationUtil)
        {
            _logger = logger;
            _testLogic = testLogic;
            _configurationUtil = configurationUtil;
        }
        [HttpGet]
        public async Task<ActionResult> GetValueByValue([FromQuery]string value)
            => Ok(await Task.FromResult(value));
        [HttpPost]
        public async Task<ActionResult> PostValueByValue([FromBody]string value)
            => Ok(await Task.FromResult(value));
        [HttpGet]
        public async Task<ActionResult> GetValueByModel([FromQuery] TestValueInputModel model)
            => Ok(await Task.FromResult(model));
        [HttpPost]
        public async Task<ActionResult> PostValueByModel([FromBody] TestValueInputModel model)
            => Ok(await Task.FromResult(model));
        [HttpPost]
        public async Task<ActionResult> QueryGrid([FromBody] CommonQueryPageModel<TestLogicQueryGridInputModel> model)
            => Ok(await _testLogic.QueryGrid(model));
        [HttpGet]
        public async Task<ActionResult> QueryWhere([FromQuery]TestLogicInputModel model)
            => Ok(await _testLogic.QueryWhere(model));
        [HttpGet]
        public async Task<ActionResult> QueryByRow([FromQuery] string row)
            => Ok(await _testLogic.QueryByRow(row));
        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] TestLogicInputModel model)
            => Ok(await _testLogic.Insert(model));
        [HttpPost]
        public async Task<ActionResult> InsertRange([FromBody]IEnumerable<TestLogicInputModel> model)
            => Ok(await _testLogic.InsertRange(model));
        [HttpPost]
        public async Task<ActionResult> Update([FromBody] TestLogicInputModel model)
            => Ok(await _testLogic.Update(model));
        [HttpPost]
        public async Task<ActionResult> UpdateRange([FromBody]IEnumerable<TestLogicInputModel> model)
            => Ok(await _testLogic.UpdateRange(model));
        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] string model)
            => Ok(await _testLogic.Delete(model));
        [HttpPost]
        public async Task<ActionResult> DeleteRange([FromBody]IEnumerable<string> model)
            => Ok(await _testLogic.DeleteRange(model));
        //[HttpGet]
        [HttpPost]
        public async Task<ActionResult> Download()
        {
            var fileInfo = new FileInfo(Path.Combine(_configurationUtil.TempPath, Guid.Empty.ToString() + ".zip"));
            if (fileInfo.Exists)
            {
                //return PhysicalFile(fileInfo.FullName, "application/download", filename);
                Response.ContentType = "application/download";
                Response.Headers.Add("content-disposition", "attachment; filename="
                    + HttpUtility.HtmlEncode(fileInfo.Name));
                //await Response.SendFileAsync(fileInfo.FullName);
                var buffer = new byte[16 * 1024];
                using (var fileStream = fileInfo.OpenRead())
                {
                    var read = 0;
                    while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await Response.Body.WriteAsync(buffer, 0, read);
                    }
                }
                //fileInfo.Delete();
                return Ok();
            }
            else
            {
                return Ok("File Not Exist");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Upload([FromForm] TestLogicUploadInputModel model)
            => Ok(await _testLogic.Upload(model));
        [HttpPost]
        public async Task<ActionResult> Uploads([FromForm] IEnumerable<TestLogicUploadInputModel> model)
            => Ok(await _testLogic.Uploads(model));
    }
}