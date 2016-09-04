using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace MusixService.Http
{
    public class HttpResponse
    {
        //private HttpOutputStream _outputStream = null;

        public HttpResponse()
        {
            Version = "HTTP/1.0";
            StatusCode = 200;
            StatusDescription = "OK";
            Headers = new NameValueCollection();
            Headers["Content-Type"] = "text/html";
            Headers["Connection"] = "close";

            OutputStream = new MemoryStream();
        }

        public string Version { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public NameValueCollection Headers { get; set; }

        public MemoryStream OutputStream { get; set; }

        //public Stream OutputStream
        //{
        //    get
        //    {
        //        if (_outputStream == null)
        //        {
        //            _outputStream = CreateOutputStream();
        //        }
        //        return _outputStream;

        //    }
        //}

        //protected HttpOutputStream CreateOutputStream()
        //{
        //    return new HttpOutputStream(this);
        //}



        public string ContentType
        {
            get { return Headers["Content-Type"]; }
            set { Headers["Content-Type"] = value; }
        }

        public long ContentLength {
            get { return Convert.ToInt64(Headers["Content-Length"] ?? "-1"); }
            set { Headers["Content-Length"] = Convert.ToString(value); }
        }

        


        public void Write(Stream outputStream)
        {
            WriteResponse(outputStream);
            WriteHeaders(outputStream);
            WriteBody(outputStream);
        }


        private void WriteResponse(Stream outputStream)
        {
            Console.WriteLine("WriteResponse: start");
            if (string.IsNullOrWhiteSpace(Version))
            {
                throw new Exception("Version is null or contains only whitespace.");
            }
            if (StatusCode < 100 || StatusCode >= 600)
            {
                throw new Exception("Status code must be between 100 and 599");
            }
            if (string.IsNullOrWhiteSpace(StatusDescription))
            {
                throw new Exception("Status description is null or contains only whitespace.");
            }

            StreamWriter outputWriter = new StreamWriter(outputStream);
            outputWriter.WriteLine(Version + " " + StatusCode + " " + StatusDescription);
            outputWriter.Flush();
            Console.WriteLine("WriteResponse: end");
        }


        private void WriteHeaders(Stream outputStream)
        {
            Console.WriteLine("WriteHeaders: start");

            if (Headers["Content-Length"] == null)
            {
                Headers["Content-Length"] = Convert.ToString(OutputStream.Length);
            }

            StreamWriter outputWriter = new StreamWriter(outputStream);
            
            // Write response headers
            foreach (string key in Headers)
            {
                foreach (string value in Headers.GetValues(key))
                {
                    outputWriter.WriteLine("{0}: {1}", key, value);
                    Console.WriteLine(string.Format("{0}: {1}", key, value));
                }
            }

            // This terminates the HTTP headers - everything after this is HTTP body.
            outputWriter.WriteLine();
            outputWriter.Flush();

            Console.WriteLine("WriteHeaders: end");
        }


        private void WriteBody(Stream outputStream)
        {
            Console.WriteLine("WriteBody: start");
            OutputStream.Seek(0, SeekOrigin.Begin);
            OutputStream.WriteTo(outputStream);
            outputStream.Flush();
            Console.WriteLine("WriteBody: end");
        }




        //public void WriteFailure(Stream outputStream)
        //{
        //    StreamWriter outputWriter = new StreamWriter(outputStream);

        //    // this is an http 404 failure response
        //    outputWriter.WriteLine("HTTP/1.0 404 File not found");
        //    // these are the HTTP headers
        //    outputWriter.WriteLine("Connection: close");
        //    // ..add your own headers here

        //    outputWriter.WriteLine(""); // this terminates the HTTP headers.
        //    outputWriter.Flush();
        //}
    }
}
