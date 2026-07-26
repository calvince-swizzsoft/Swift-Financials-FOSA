using System;

namespace Application.MainBoundedContext.DTO
{
    public class MediaDTO
    {
        public Guid SKU { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileRemarks { get; set; }

        public string FileExtension { get; set; }
        
        public string ContentType { get; set; }

        public string ContentCoding { get; set; }

        public long ContentLength { get; set; }

        public byte[] Content { get; set; }
    }
}
