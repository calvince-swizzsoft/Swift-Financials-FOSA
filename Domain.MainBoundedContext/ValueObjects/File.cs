using Domain.Seedwork;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class File : ValueObject<File>
    {
        public byte[] Buffer { get; private set; }

        public File(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        private File()
        {

        }
    }
}
