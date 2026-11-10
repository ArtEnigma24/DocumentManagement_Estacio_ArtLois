using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentManagementlibrary.Models
{
    public class DocumentModel
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; }
        public string StoredFileName { get; set; } 
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAtUtc { get; set; }
    }
}
