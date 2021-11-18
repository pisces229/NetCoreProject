using Microsoft.AspNetCore.Http;

namespace NetCoreProject.BusinessLayer.Model.Test
{
    public class TestLogicUploadInputModel
    {
        public IFormFile UPLOAD_FILE { get; set; }
        public string UPLOAD_NAME { get; set; }
        public string UPLOAD_TYPE { get; set; }
    }
}
