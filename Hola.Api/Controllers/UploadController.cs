﻿using Hola.Core.Model;
using Hola.GoogleCloudStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace Hola.Api.Controllers
{
    public class UploadController : ControllerBase
    {
        public readonly IUploadFileGoogleCloudStorage _GoogleCloudStorage;

        public UploadController(IUploadFileGoogleCloudStorage googleCloudStorage)
        {
            _GoogleCloudStorage = googleCloudStorage;
        }

        /// <summary>
        /// Upload file image to Server, Author : Nguyễn Viết Minh Tiến
        /// </summary>
        /// <param name="inputFiles"></param>
        /// <returns></returns>
        [HttpPost("UploadImage")]
        public async Task<JsonResponseModel> UploadFile(IFormFile? inputFiles)
        {
            
            var filename = inputFiles;
            var filePath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(filePath))
            {
                // The formFile is the method parameter which type is IFormFile
                // Saves the files to the local file system using a file name generated by the app.
                await inputFiles.CopyToAsync(stream);
            }
            var resultUrl =   _GoogleCloudStorage.UploadFile(filePath,"5512421445", inputFiles.FileName, "credentials.json", "image", "image/jpeg");
            return JsonResponseModel.Success(resultUrl);
        }
    }
}
