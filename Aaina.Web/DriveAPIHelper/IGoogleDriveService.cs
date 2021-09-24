using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aaina.Dto;
using Google.Apis.Drive.v3;
using Microsoft.AspNetCore.Http;

namespace Aaina.Web
{
    public interface IGoogleDriveService
    {
        DriveService GetService();

        List<GoogleDriveFile> GetDriveFiles();

        string UplaodFile(IFormFile file, string folderId=null);

        MemoryStream DownloadAsStrem(string fileId);

        string DownloadAsPath(string fileId);

        bool DeleteFile(string id);

        string CreateFolder(string folderName, string parentId = null);

        bool IsFolderExist(string folderName);

        List<GoogleDriveFile> GetDriveFolders();

        bool MoveFiles(string fileId, string folderId);

        bool CopyFiles(string fileId, string folderId);

        void PermissionCreate(string fileId);

        string GetSharedwebViewLink(string fileId);
    }
}
