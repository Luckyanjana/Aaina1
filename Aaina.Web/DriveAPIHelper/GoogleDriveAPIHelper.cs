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
    public class GoogleDriveAPIHelper
    {

        public static string[] Scopes = { DriveService.Scope.Drive };

        //create Drive API service.
        public static DriveService GetService(IHostingEnvironment env)
        {
            //get Credentials from client_secret.json file 
            UserCredential credential;
            //Root Folder of project
            var CSPath = env.WebRootPath;
            using (var stream = new FileStream(Path.Combine(CSPath, "credentials.json"), FileMode.Open, FileAccess.Read))
            {
                String FolderPath = env.WebRootPath;
                String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }
            //create Drive API service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "aaiina Drive App",
            });
            return service;
        }

        //get all files from Google Drive.
        public static List<GoogleDriveFile> GetDriveFiles(IHostingEnvironment env)
        {
            DriveService service = GetService(env);

            // Define parameters of request.
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
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


        //file Upload to the Google Drive root folder.
        public static string UplaodFileOnDrive(IHostingEnvironment env, string folderId, IFormFile file)
        {
            if (file != null)
            {
                //create service
                DriveService service = GetService(env);
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
                // FileMetaData.MimeType = MimeMapping.GetMimeMapping(path);
                if (!string.IsNullOrEmpty(folderId))
                {
                    FileMetaData.Parents = new List<string>
                    {
                        folderId
                    };
                }
                FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }
                var response = request.ResponseBody;
                return response.Id;
            }
            return null;
        }

        public static void UplaodFileOnDriveWithShare(IHostingEnvironment env, string folderId, IFormFile file)
        {
            if (file != null)
            {
                //create service
                DriveService service = GetService(env);
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
                // FileMetaData.MimeType = MimeMapping.GetMimeMapping(path);
                if (!string.IsNullOrEmpty(folderId))
                {
                    FileMetaData.Parents = new List<string>
                    {
                        folderId
                    };
                    FileMetaData.Permissions.Add(new Permission()
                    {
                        Role = "reader",
                        Type = "anyone"
                    });
                }
                FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    var  response=request.Upload();
                    
                }
            }
        }

        //Download file from Google Drive by fileId.
        public static string DownloadGoogleFile(IHostingEnvironment env, string fileId)
        {
            DriveService service = GetService(env);
            var webRoot = env.WebRootPath + "/DYF";
            string FolderPath = $"{webRoot}/GoogleDriveFiles/";
            FilesResource.GetRequest request = service.Files.Get(fileId);
            string FileName = request.Execute().Name;
            string FilePath = Path.Combine(FolderPath, FileName);

            MemoryStream stream1 = new MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
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

        // file save to server path
        private static void SaveStream(MemoryStream stream, string FilePath)
        {
            using (System.IO.FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }

        //Delete file from the Google drive
        public static void DeleteFile(IHostingEnvironment env, GoogleDriveFile files)
        {
            Google.Apis.Drive.v3.DriveService service = GetService(env);
            try
            {
                // Initial validation.
                if (service == null)
                    throw new ArgumentNullException("service");

                if (files == null)
                    throw new ArgumentNullException(files.Id);

                // Make the request.
                service.Files.Delete(files.Id).Execute();
            }
            catch (Exception ex)
            {
                throw new Exception("Request Files.Delete failed.", ex);
            }
        }

        public static void DeleteFile(IHostingEnvironment env, string id)
        {
            Google.Apis.Drive.v3.DriveService service = GetService(env);
            try
            {
               

                // Make the request.
                service.Files.Delete(id).Execute();
            }
            catch (Exception ex)
            {
                throw new Exception("Request Files.Delete failed.", ex);
            }
        }

        // Create Folder in root
        public static string CreateFolder(IHostingEnvironment env, string FolderName)
        {
            Google.Apis.Drive.v3.DriveService service = GetService(env);

            var FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.Name = FolderName;
            FileMetaData.MimeType = "application/vnd.google-apps.folder";

            FilesResource.CreateRequest request;

            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();
            return file.Id;
        }

        // Create Folder in existing folder
        public static string CreateFolderInFolder(IHostingEnvironment env, string folderId, string FolderName)
        {

            Google.Apis.Drive.v3.DriveService service = GetService(env);

            var FileMetaData = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(FolderName),
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>
                    {
                        folderId
                    }
            };


            FilesResource.CreateRequest request;

            request = service.Files.Create(FileMetaData);
            request.Fields = "id";
            var file = request.Execute();
            return file.Id;

        }


        // check Folder name exist or note in root
        public static bool CheckFolder(IHostingEnvironment env, string FolderName)
        {
            bool IsExist = false;

            Google.Apis.Drive.v3.DriveService service = GetService(env);

            // Define parameters of request.
            Google.Apis.Drive.v3.FilesResource.ListRequest FileListRequest = service.Files.List();
            FileListRequest.Fields = "nextPageToken, files(*)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFile> FileList = new List<GoogleDriveFile>();


            //For getting only folders
            files = files.Where(x => x.MimeType == "application/vnd.google-apps.folder" && x.Name == FolderName).ToList();

            if (files.Count > 0)
            {
                IsExist = false;
            }
            return IsExist;
        }


        public static List<GoogleDriveFile> GetDriveFolders(IHostingEnvironment env)
        {
            Google.Apis.Drive.v3.DriveService service = GetService(env);
            List<GoogleDriveFile> FolderList = new List<GoogleDriveFile>();

            Google.Apis.Drive.v3.FilesResource.ListRequest request = service.Files.List();
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

        public static string MoveFiles(IHostingEnvironment env, string fileId, string folderId)
        {
            Google.Apis.Drive.v3.DriveService service = GetService(env);

            // Retrieve the existing parents to remove
            Google.Apis.Drive.v3.FilesResource.GetRequest getRequest = service.Files.Get(fileId);
            getRequest.Fields = "parents";
            Google.Apis.Drive.v3.Data.File file = getRequest.Execute();
            string previousParents = String.Join(",", file.Parents);

            // Move the file to the new folder
            Google.Apis.Drive.v3.FilesResource.UpdateRequest updateRequest = service.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId);
            updateRequest.Fields = "id, parents";
            updateRequest.AddParents = folderId;
            updateRequest.RemoveParents = previousParents;

            file = updateRequest.Execute();
            if (file != null)
            {
                return "Success";
            }
            else
            {
                return "Fail";
            }
        }
        public static string CopyFiles(IHostingEnvironment env, string fileId, string folderId)
        {
            Google.Apis.Drive.v3.DriveService service = GetService(env);

            // Retrieve the existing parents to remove
            Google.Apis.Drive.v3.FilesResource.GetRequest getRequest = service.Files.Get(fileId);
            getRequest.Fields = "parents";
            Google.Apis.Drive.v3.Data.File file = getRequest.Execute();

            // Copy the file to the new folder
            Google.Apis.Drive.v3.FilesResource.UpdateRequest updateRequest = service.Files.Update(new Google.Apis.Drive.v3.Data.File(), fileId);
            updateRequest.Fields = "id, parents";
            updateRequest.AddParents = folderId;
            //updateRequest.RemoveParents = previousParents;
            file = updateRequest.Execute();
            if (file != null)
            {
                return "Success";
            }
            else
            {
                return "Fail";
            }
        }

        public static void PermissionCreate(IHostingEnvironment env, string fileId)
        {
            DriveService service = GetService(env);

            
            PermissionsResource.CreateRequest getRequest = service.Permissions.Create(new Permission
            {
                Role = "reader",
                Type = "anyone"
            }, fileId);
            getRequest.Fields = "parents";
            var file = getRequest.Execute();
        }

        public static string GetSharedwebViewLink(IHostingEnvironment env, string fileId)
        {
            DriveService service = GetService(env);
            FilesResource.GetRequest getRequest = service.Files.Get(fileId);
            return getRequest.Execute().WebViewLink;
        }

    }
}
