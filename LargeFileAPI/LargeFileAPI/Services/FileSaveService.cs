using LargeFileAPI.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;
using tusdotnet.Models.Configuration;

namespace LargeFileAPI.Services
{
    public class FileSaveService
    {
        private AppSettings AppSettings { get; }
        public FileSaveService(IOptions<AppSettings> appSettings)
        {
            AppSettings = appSettings.Value;
        }

        public async Task SaveFileUsingMemoryStream(IFormFile file)
        {
            using var ms = new MemoryStream();

            await file.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);

            File.WriteAllBytes(Path.Combine(AppSettings.FileSavePath, file.FileName), ms.GetBuffer());
        }

        public async Task SaveFileUsingStream(IFormFile file)
        {
            using var createdFile = File.Create(Path.Combine(AppSettings.FileSavePath, file.FileName));

            await file.CopyToAsync(createdFile);
        }

        public async Task<bool> SaveFileUsingTus(FileCompleteContext eventContext)
        {
            try
            {
                ITusFile file = await eventContext.GetFileAsync();

                var metadata = await file.GetMetadataAsync(eventContext.CancellationToken);
                var fileName = metadata["name"].GetString(System.Text.Encoding.UTF8);

                using (var createdFile = File.Create(Path.Combine(AppSettings.FileSavePath, fileName)))
                using (var stream = await file.GetContentAsync(eventContext.CancellationToken))
                {
                    await stream.CopyToAsync(createdFile);
                }

                var terminationStore = (ITusTerminationStore)eventContext.Store;
                await terminationStore.DeleteFileAsync(file.Id, eventContext.CancellationToken);
                return true;
            } catch (Exception e)
            {
                return false;
            }            
        }
    }
}
