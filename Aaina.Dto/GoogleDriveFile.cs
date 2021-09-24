using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class GoogleDriveFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public long? Size { get; set; }
        public long? Version { get; set; }
        public DateTime? CreatedTime { get; set; }
        public IList<string> Parents { get; set; }
        public string MimeType { get; set; }
    }

    public class DropboxDiles
    {
       
        public bool IsFile { get; set; }
       
      
        public bool IsFolder { get; set; }
       
      
        public string Name { get; set; }
      
        public string PathLower { get; set; }
     
        public string PathDisplay { get; set; }
       
        public string ParentSharedFolderId { get; set; }
    }
}
