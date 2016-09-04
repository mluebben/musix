using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MusixService.Http
{
    class HttpChunkedOutputStream : Stream
    {
        

        private bool _disposed = false;

        public HttpChunkedOutputStream(Stream stream)
        {
            BaseStream = stream;
        }


        public Stream BaseStream { get; private set; }


        public override bool CanRead { get { return false; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        public override long Length { get { throw new NotSupportedException(); } }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("HttpChunkedOutputStream");  // stream already closed
            }
            if (count == 0)
            {
                return;  // empty buffer
            }

            WriteChunk(buffer, offset, count);
        }

        private void WriteEndOfStream()
        {
            byte[] data = new byte[0];
            WriteChunk(data, 0, 0);
        }

        private void WriteChunk(byte[] data, int offset, int count)
        {
            WriteChunkSize(count);
            WriteChunkData(data, offset, count);
        }

        private void WriteChunkSize(int chunkSize)
        {
            byte[] data = Encoding.UTF8.GetBytes(string.Format("{0:X}", chunkSize));
            byte[] crlf = { 13, 10 };
            BaseStream.Write(data, 0, data.Length);
            BaseStream.Write(crlf, 0, crlf.Length);
        }

        private void WriteChunkData(byte[] data, int offset, int count)
        {
            byte[] crlf = { 13, 10 };
            if (count > 0) {
                BaseStream.Write(data, offset, count);
            }
            BaseStream.Write(crlf, 0, crlf.Length);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                WriteEndOfStream();
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
