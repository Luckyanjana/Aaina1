using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aaina.Dto;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Aaina.Web
{
    public class GoogleDriveService: IGoogleDriveService
    {
        private readonly IHostingEnvironment env;
        public static string[] Scopes = { DriveService.Scope.Drive };
        public GoogleDriveService(IHostingEnvironment _env)
        {
            this.env = _env;
        }
        public DriveService GetService()
        {
            UserCredential credential;
            var CSPath = this.env.WebRootPath;
            using (var stream = new FileStream(Path.Combine(CSPath, "credentials.json"), FileMode.Open, FileAccess.Read))
            {
                String FolderPath = this.env.WebRootPath;
                String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "aaiina Drive App",
            });
            return service;
        }

        public List<GoogleDriveFile> GetDriveFiles()
        {
            DriveService service = GetService();

            // Define parameters of request.
            FilesResource.ListRequest FileListRequest = service.Files.List();
            // for getting folders only.
            //FileListRequest.Q = "mimeType='application/vnd.google-apps.folder'";
            FileListRequest.Fields = "nextPageToken, files(*)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFile> FileList = new List<GoogleDriveFile>();


            // For getting only folders
            // files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder").ToList();


            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    GoogleDriveFile File = new GoogleDriveFile
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        Version = file.Version,
                        CreatedTime = file.CreatedTime,
                        Parents = file.Parents,
                        MimeType = file.MimeType
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }

        public string UplaodFile(IFormFile file, string folderId=null)
        {
            if (file != null)
            {
                //create service
                DriveService service = GetService();
                var webRoot = env.WebRootPath + "/DYF";
                if (!Directory.Exists($"{webRoot}/GoogleDriveFiles/"))
                {
                    Directory.CreateDirectory($"{webRoot}/GoogleDriveFiles/");
                }

                string path = $"{webRoot}/GoogleDriveFiles/{Path.GetFileName(file.FileName)}";

                using (var streams = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(streams);
                }

                var FileMetaData = new Google.Apis.Drive.v3.Data.File();
                FileMetaData.Name = Path.GetFileName(file.FileName);
                //FileMetaData.MimeType = MimeMapping.GetMimeMapping(path);
                if (!string.IsNullOrEmpty(folderId))
                {
                    FileMetaData.Parents = new List<string>
                    {
                        folderId
                    };
                }
                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(path, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }
                var response = request.ResponseBody;
                var fileInfo = new FileInfo(path);
                if (fileInfo != null)
                {
                    fileInfo.Delete();
                }
                return response.Id;
            }
            return null;
        }

        public MemoryStream DownloadAsStrem(string fileId)
        {
            DriveService service = GetService();
            var webRoot = env.WebRootPath + "/DYF";
            string FolderPath = $"{webRoot}/GoogleDriveFiles/";
            FilesResource.GetRequest request = service.Files.Get(fileId);
            string FileName = request.Execute().Name;
            string FilePath = Path.Combine(FolderPath, FileName);

            MemoryStream stream1 = new MemoryStream();


            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream1, FilePath);
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream1);

            var fileInfo = new FileInfo(FilePath);

            if (fileInfo != null)
            {
                fileInfo.Delete();
            }

            return stream1;
        }

        public string DownloadAsPath(string fileId)
        {
            DriveService service = GetService();
            var webRoot = env.WebRootPath + "/DYF";
            string FolderPath = $"{webRoot}/GoogleDriveFiles/";
            FilesResource.GetRequest request = service.Files.Get(fileId);
            string FileName = request.Execute().Name;
            string FilePath = Path.Combine(FolderPath, FileName);

            MemoryStream stream1 = new MemoryStream();

            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream1, FilePath);
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream1);
            return FilePath;
        }

        public bool DeleteFile(string id)
        {
            DriveService service = GetService();
            try
            {
                service.Files.Delete(id).Execute();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string CreateFolder(string folderName, string parentId=null)
        {
            DriveService service = GetService();
            var fileMetaData = new Google.Apis.Drive.v3.Data.File();
            fileMetaData.Name = folderName;
            fileMetaData.MimeType = "application/vnd.google-apps.folder";
            if (!string.IsNullOrEmpty(parentId))
            {
                fileMetaData.Parents = new List<string>
                    {
                        parentId
                    };
            }
            FilesResource.CreateRequest request;
            request = service.Files.Create(fileMetaData);
            request.Fields = "id";
            var file = request.Execute();
            return file.Id;
        }

        public bool IsFolderExist(string folderName)
        {
            bool IsExist = false;
            DriveService service = GetService();
            // Define parameters of request.
            FilesResource.ListRequest FileListRequest = service.Files.List();
            FileListRequest.Fields = "nextPageToken, files(*)";
            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            IsExist = files.Any(x => x.MimeType == "application/vnd.google-apps.folder" && x.Name == folderName);           
            return IsExist;
        }
        public List<GoogleDriveFile> GetDriveFolders()
        {
            DriveService service = GetService();
            List<GoogleDriveFile> FolderList = new List<GoogleDriveFile>();

            FilesResource.ListRequest request = service.Files.List();
            request.Q = "mimeType='application/vnd.google-apps.folder'";
            request.Fields = "files(id, name)";
            Google.Apis.Drive.v3.Data.FileList result = request.Execute();
            foreach (var file in result.Files)
            {
                GoogleDriveFile File = new GoogleDriveFile
                {
                    Id = file.Id,
                    Name = file.Name,
                    Size = file.Size,
                    Version = file.Version,
                    CreatedTime = file.CreatedTime
                };
                FolderList.Add(File);
            }
            return FolderList;
        }

        public bool MoveFiles(string fileId, string folderId)
        {
            DriveService service = GetService();

            // Retrieve the existing parents to remove
            FilesResource.GetRequest getRequest = service.Files.Get(fileId);
            getRequest.Fields = "parents";
            Google.Apis.Drive.v3.Data.File file = getRequest.Execute();
            string previousParents = String.Join(",", file.Parents);

            // Move the file to the new folder
            FilesResource.UpdateRequest updateRequest = service.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId);
            updateRequest.Fields = "id, parents";
            updateRequest.AddParents = folderId;
            updateRequest.RemoveParents = previousParents;

            file = updateRequest.Execute();
            return file != null;            
        }
        public bool CopyFiles(string fileId, string folderId)
        {
            DriveService service = GetService();

            // Retrieve the existing parents to remove
            FilesResource.GetRequest getRequest = service.Files.Get(fileId);
            getRequest.Fields = "parents";
            Google.Apis.Drive.v3.Data.File file = getRequest.Execute();

            // Copy the file to the new folder
            Google.Apis.Drive.v3.FilesResource.UpdateRequest updateRequest = service.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId);
            updateRequest.Fields = "id, parents";
            updateRequest.AddParents = folderId;            
            file = updateRequest.Execute();
            return file != null;
        }

        public void PermissionCreate(string fileId)
        {
            DriveService service = GetService();
            PermissionsResource.CreateRequest getRequest = service.Permissions.Create(new Permission
            {
                Role = "reader",
                Type = "anyone"
            }, fileId);
            getRequest.Fields = "parents";
            var file = getRequest.Execute();            
        }

        public string GetSharedwebViewLink(string fileId)
        {
            DriveService service = GetService();

            //PermissionsResource.CreateRequest getRequestPermission = service.Permissions.Create(new Permission
            //{
            //    Role = "reader",
            //    Type = "anyone"
            //}, fileId);
            //getRequestPermission.Fields = "parents";
            //var file = getRequestPermission.Execute();

            FilesResource.GetRequest getRequest = service.Files.Get(fileId);
            return getRequest.Execute().WebViewLink;
        }

        private static void SaveStream(MemoryStream stream, string FilePath)
        {
            using (System.IO.FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }

    }
}
