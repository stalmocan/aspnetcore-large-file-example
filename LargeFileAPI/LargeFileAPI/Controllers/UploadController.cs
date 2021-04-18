using LargeFileAPI.Configuration;
using LargeFileAPI.Models;
using LargeFileAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LargeFileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private FileSaveService FileSaveService { get; }

        public UploadController(FileSaveService fileSaveService)
        {
            FileSaveService = fileSaveService;
        }

        [HttpPost]
        [Route("memorystream")]
        [RequestSizeLimit(long.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task UploadFileMemoryStream([FromForm]UploadForm form)
        {
            await FileSaveService.SaveFileUsingMemoryStream(form.File);
        }

        [HttpPost]
        [Route("stream")]
        [RequestSizeLimit(long.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task UploadFile([FromForm] UploadForm form)
        {
            await FileSaveService.SaveFileUsingStream(form.File);
        }
    }
}
