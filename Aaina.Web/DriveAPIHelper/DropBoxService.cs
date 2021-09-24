using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aaina.Common;
using Aaina.Dto;
using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Aaina.Web
{
    public class DropBoxService : IDropBoxService
    {
        private readonly DropboxClient DBClient;
        private readonly IHostingEnvironment env;
        public DropBoxService(IHostingEnvironment env)
        {
            DBClient = GenerateDropboxClient();
            this.env = env;
        }
        private DropboxClient GenerateDropboxClient()
        {
            try
            {
                DropboxClientConfig CC = new DropboxClientConfig(SiteKeys.Dropbox_App, 1);
                HttpClient HTC = new HttpClient();
                HTC.Timeout = TimeSpan.FromMinutes(10); // set timeout for each ghttp request to Dropbox API.  
                CC.HttpClient = HTC;
                return new DropboxClient(SiteKeys.Dropbox_AccessTocken, CC);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DropboxDiles> GetFileFolders(string path)
        {
            List<DropboxDiles> response = new List<DropboxDiles>();
            try
            {


                var folder = DBClient.Files.ListFolderAsync(path);
                var result = folder.Result;
                foreach (var item in result.Entries)
                {
                    response.Add(new DropboxDiles()
                    {
                        IsFile = item.IsFile,
                        IsFolder = item.IsFolder,
                        Name = item.Name,
                        ParentSharedFolderId = item.ParentSharedFolderId,
                        PathDisplay = item.PathDisplay,
                        PathLower = item.PathLower
                    });
                }

            }
            catch (Exception ex)
            {

            }
            return response;

        }
        public bool CreateFolder(string path)
        {
            try
            {

                var folderArg = new CreateFolderArg(path);
                var folder = DBClient.Files.CreateFolderAsync(folderArg);
                var result = folder.Result;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public bool FolderExists(string path)
        {
            try
            {

                var folders = DBClient.Files.ListFolderAsync(path);
                var result = folders.Result;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool Delete(string path)
        {
            try
            {

                var folders = DBClient.Files.DeleteAsync(path);
                var result = folders.Result;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task PermanentlyDeleteAsync(string path)
        {
            try
            {

                await DBClient.Files.PermanentlyDeleteAsync(path);


            }
            catch (Exception ex)
            {

            }
        }

        public bool Upload(string UploadfolderPath, string UploadfileName, string SourceFilePath)
        {
            try
            {
                using (var stream = new MemoryStream(File.ReadAllBytes(SourceFilePath)))
                {
                    var response = DBClient.Files.UploadAsync(UploadfolderPath + "/" + UploadfileName, WriteMode.Overwrite.Instance, body: stream);
                    var rest = response.Result;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool Upload(string uploadfolderPath, string fileName, byte[] buffer)
        {
            try
            {
                using (var stream = new MemoryStream(buffer))
                {
                    var response = DBClient.Files.UploadAsync(uploadfolderPath + "/" + fileName, WriteMode.Overwrite.Instance, body: stream);
                    var rest = response.Result;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool Upload(IFormFile file, string parenthPath)
        {
            try
            {
                if (file != null)
                {
                    var response = DBClient.Files.UploadAsync(parenthPath + "/" + file.FileName, WriteMode.Overwrite.Instance, body: file.OpenReadStream());
                    var rest = response.Result;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public Stream Download(string DropboxFolderPath, string DropboxFileName)
        {
            try
            {
                var response = DBClient.Files.DownloadAsync(DropboxFolderPath + "/" + DropboxFileName);
                return response.Result.GetContentAsStreamAsync().Result; //Added to wait for the result from Async method  


            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public Stream Download(string fileath)
        {
            try
            {
                var response = DBClient.Files.DownloadAsync(fileath);
                return response.Result.GetContentAsStreamAsync().Result; //Added to wait for the result from Async method  


            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public string Share(string fileath)
        {
            try
            {
                var response = DBClient.Sharing.CreateSharedLinkAsync(fileath, true);
                return response.Result.Url; //Added to wait for the result from Async method 
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public string PrivateShare(string fileath)
        {
            try
            {
                RequestedVisibility visibility = new RequestedVisibility().AsPassword;
                LinkAudience audience = new LinkAudience().AsMembers;
                DateTime date = DateTime.Now.AddMinutes(5);
                var sharedLinkSettings = new SharedLinkSettings(visibility, "123456", date, audience);
                var overAllSettings = new CreateSharedLinkWithSettingsArg(fileath, sharedLinkSettings);
                var shared = DBClient.Sharing.CreateSharedLinkWithSettingsAsync(overAllSettings);
                var res = shared.Result;
                return res.Url; //Added to wait for the result from Async method 
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // skandira@hotmail.com
    }
}
