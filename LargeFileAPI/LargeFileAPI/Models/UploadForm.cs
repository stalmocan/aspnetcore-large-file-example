using Microsoft.AspNetCore.Http;

namespace LargeFileAPI.Models
{
    public class UploadForm
    {
        public IFormFile File { get; set; }
    }
}
