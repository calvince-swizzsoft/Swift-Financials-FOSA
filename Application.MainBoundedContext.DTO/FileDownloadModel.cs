using System.IO;

namespace Application.MainBoundedContext.DTO
{
    public class FileDownloadModel
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string ContentCoding { get; set; }

        public long ContentLength { get; set; }

        public Stream Content { get; set; }
    }
}
