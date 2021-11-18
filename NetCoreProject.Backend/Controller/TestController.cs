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

namespace NetCoreProject.Backend.Controller
{
    //[Authorize]
    [ServiceFilter(typeof(DefaultAuthorizationFilter))]
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
        {
            var result = await Task.FromResult(value);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> PostValueByValue([FromBody]string value)
        {
            var result = await Task.FromResult(value);
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult> GetValueByModel([FromQuery] TestValueInputModel model)
        {
            var result = await Task.FromResult(model);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> PostValueByModel([FromBody] TestValueInputModel model)
        {
            var result = await Task.FromResult(model);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> QueryGrid([FromBody] CommonQueryPageModel<TestLogicQueryGridInputModel> model)
        {
            var result = await _testLogic.QueryGrid(model);
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult> QueryWhere([FromQuery]TestLogicInputModel model)
        {
            var result = await _testLogic.QueryWhere(model);
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult> QueryByRow([FromQuery] string row)
        {
            var result = await _testLogic.QueryByRow(row);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> Insert([FromBody] TestLogicInputModel model)
        {
            var result = await _testLogic.Insert(model);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> InsertRange([FromBody]IEnumerable<TestLogicInputModel> model)
        {
            var result = await _testLogic.InsertRange(model);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> Update([FromBody] TestLogicInputModel model)
        {
            var result = await _testLogic.Update(model);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateRange([FromBody]IEnumerable<TestLogicInputModel> model)
        {
            var result = await _testLogic.UpdateRange(model);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> Delete([FromBody] string model)
        {
            var result = await _testLogic.Delete(model);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteRange([FromBody]IEnumerable<string> model)
        {
            var result = await _testLogic.DeleteRange(model);
            return Ok(result);
        }
        [HttpGet]
        public async Task<ActionResult> GetDownload()
        {
            await Task.FromResult("");
            var temppath = _configurationUtil.TempPath;
            var filename = Guid.Empty.ToString() + ".zip";

            //var physicalFileProvider = new PhysicalFileProvider(temppath);
            //var fileInfo = physicalFileProvider.GetFileInfo(filename);
            ////Response.Headers.Add("x-download-filename", "Download.docx");
            //return File(fileInfo.CreateReadStream(), "application/download");

            var fileInfo = new FileInfo(Path.Combine(temppath, filename));
            if (fileInfo.Exists)
            {
                //return PhysicalFile(fileInfo.FullName, "application/download", filename);
                Response.ContentType = "application/download";
                Response.Headers.Add("content-disposition", "attachment; filename="
                    + HttpUtility.HtmlEncode(filename));
                await Response.SendFileAsync(fileInfo.FullName);
                return Ok();
            }
            else
            {
                return Ok("File Not Exist");
            }
        }
        [HttpPost]
        public async Task<ActionResult> PostDownload()
        {
            await Task.FromResult("");
            var temppath = _configurationUtil.TempPath;
            var filename = Guid.Empty.ToString() + ".zip";

            //var physicalFileProvider = new PhysicalFileProvider(temppath);
            //var fileInfo = physicalFileProvider.GetFileInfo(filename);
            ////Response.Headers.Add("x-download-filename", "Download.docx");
            //return File(fileInfo.CreateReadStream(), "application/download");

            var fileInfo = new FileInfo(Path.Combine(temppath, filename));
            if (fileInfo.Exists)
            {
                //return PhysicalFile(fileInfo.FullName, "application/download", filename);
                Response.ContentType = "application/download";
                Response.Headers.Add("content-disposition", "attachment; filename="
                    + HttpUtility.HtmlEncode(filename));
                await Response.SendFileAsync(fileInfo.FullName);
                return Ok();
            }
            else
            {
                return Ok("File Not Exist");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Upload([FromForm] TestLogicUploadInputModel model)
        {
            var result = await _testLogic.Upload(model);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> Uploads([FromForm] IEnumerable<TestLogicUploadInputModel> model)
        {
            var result = await Task.FromResult(new CommonApiResultModel<string>()
            {
                Success = true,
                Message = "Complete"
            });
            return Ok(result);
        }

    }
}