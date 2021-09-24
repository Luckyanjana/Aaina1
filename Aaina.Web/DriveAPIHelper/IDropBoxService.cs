using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using Microsoft.AspNetCore.Http;

namespace Aaina.Web
{
    public interface IDropBoxService
    {
        List<DropboxDiles> GetFileFolders(string path);
        bool CreateFolder(string path);

        bool FolderExists(string path);

        bool Delete(string path);

        Task PermanentlyDeleteAsync(string path);

        bool Upload(string UploadfolderPath, string UploadfileName, string SourceFilePath);

        bool Upload(string uploadfolderPath, string fileName, byte[] buffer);

        Stream Download(string DropboxFolderPath, string DropboxFileName);

        

        bool Upload(IFormFile file, string parenthPath);
        
        Stream Download(string fileath);
        string Share(string fileath);

        string PrivateShare(string fileath);
    }
}
