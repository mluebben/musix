using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace MusixService.Http
{
    public class HttpOutputStream : Stream
    {
        public HttpOutputStream(HttpContext context)
        {
            Context = context;

            //            _acceptEncoding = request.Headers["Accept-Encoding"];
            //            _acceptEncoding
            //            request.AcceptGzip;
            //            request.AcceptDeflate;

            //            if (request.)
            //            GZipStream
            //            DeflateStream


            //            request.AcceptTransferEncodingChunked 


            BufferedStream = new BufferedStream(TransferStream);
        }

        public HttpContext Context { get; private set; }

        public string ContentEncoding { get; private set; }
        public string ContentType { get; set; }
        public string ContentLength { get; set; }
        
        public string TransferEncoding { get; private set; }


        //protected Stream CreateContentStream()
        //{

        //}

        //protected Stream CreateTransferStream()
        //{

        //}

        public Stream BufferedStream { get; private set; }
        public Stream ContentStream { get; private set; }
        public Stream TransferStream { get; private set; }


        //public bool IsChunked { get; private set; }
        //public bool IsFixed { get; private set; }


        //public Stream BaseStream { get; private set; }


        public override bool CanRead { get { return false; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void Flush()
        {
            //BaseStream.Flush();
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
            
        }




        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
