using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    /// <summary>
    /// Implementation of System.IO.Stream based on a SqlFileStream
    /// Disposes the connection, transaction and SqlFileStream objects
    /// </summary>
    public class MvcResultSqlFileStream : Stream
    {
        public SqlFileStream SqlStream { get; set; }
        public SqlConnection Connection { get; set; }
        public SqlTransaction Transaction { get; set; }
        public override bool CanRead
        {
            get { return SqlStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return SqlStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return SqlStream.CanWrite; }
        }

        public override void Flush()
        {
            SqlStream.Flush();
        }

        public override long Length
        {
            get { return SqlStream.Length; }
        }

        public override long Position
        {
            get
            {
                return SqlStream.Position;
            }
            set
            {
                SqlStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return SqlStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return SqlStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            SqlStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            SqlStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SqlStream.Dispose();
                Transaction.Dispose();
                Connection.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
