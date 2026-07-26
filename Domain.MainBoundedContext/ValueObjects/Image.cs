using Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.MainBoundedContext.ValueObjects
{
    public class Image : ValueObject<Image>
    {
        public byte[] Buffer { get; private set; }

        public Image(byte[] buffer)
        {
            this.Buffer = buffer;
        }

        private Image()
        { }
    }
}
