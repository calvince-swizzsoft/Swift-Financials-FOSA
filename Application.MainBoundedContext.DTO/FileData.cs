using Application.Seedwork;
using System.Runtime.Serialization;

namespace Application.MainBoundedContext.DTO
{
    public class FileData : BindingModelBase<FileData>
    {
        public FileData()
        {
            AddAllAttributeValidators();
        }

        [DataMember]
        public string Filename { get; set; }

        [DataMember]
        public int Offset { get; set; }

        [DataMember]
        public byte[] Buffer { get; set; }
    }
}