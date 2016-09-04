using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MusixService.Http
{
    public class HttpRequest
    {
        /// <summary>
        /// Maximal POST data size.
        /// </summary>
        private static int MAX_POST_SIZE = 20 * 1024 * 1024; // 10MB

        /// <summary>
        /// Read buffer size.
        /// </summary>
        private const int BUF_SIZE = 4096;


        /// <summary>
        /// Construct request object.
        /// </summary>
        public HttpRequest()
        {
            Headers = new NameValueCollection();
            User = new HttpUser();
            InputStream = new MemoryStream();
        }


        /// <summary>
        /// HTTP method like "HEAD", "GET" or "POST".
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// HTTP resource like /test.png.
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// HTTP version like "HTTP/1.0" or "HTTP/1.1"
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// HTTP request headers.
        /// </summary>
        public NameValueCollection Headers { get; set; }

        /// <summary>
        /// Username and password from basic authentication
        /// </summary>
        public HttpUser User { get; set; }

        /// <summary>
        /// POST data.
        /// </summary>
        public Stream InputStream { get; set; }


        /// <summary>
        /// Returns true for a HEAD request, otherwise false.
        /// </summary>
        public bool IsHEAD { get { return Method == null ? false : Method.Equals("HEAD"); } }

        /// <summary>
        /// Returns true for a GET request, otherwise false.
        /// </summary>
        public bool IsGET { get { return Method == null ? false : Method.Equals("GET"); } }

        /// <summary>
        /// Returns true for a POST request, otherwise false.
        /// </summary>
        public bool IsPOST {  get { return Method == null ? false : Method.Equals("POST"); } }




        public void Read(Stream inputStream)
        {
            ReadRequest(inputStream);
            ReadHeaders(inputStream);
            ReadBody(inputStream);
        }

        private void ReadRequest(Stream inputStream)
        {
            string request = StreamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }

            Method = tokens[0].ToUpper();
            Resource = tokens[1];
            Version = tokens[2];

            Console.WriteLine("starting: " + request);
        }


        private void ReadHeaders(Stream inputStream)
        {
            Console.WriteLine("readHeaders()");
            string line;
            while ((line = StreamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");

                    if (Headers["Authorization"] != null)
                    {
                        User.ParseAuthorization(Headers["Authorization"]);
                    }

                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                string name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                Headers.Add(name, value);
            }

            
        }

        
        private string StreamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') {
                    break;
                }
                if (next_char == '\r') {
                    continue;
                }
                if (next_char == -1) {
                    Thread.Sleep(1);
                    continue;
                };
                data += Convert.ToChar(next_char);
            }
            return data;
        }


        public void ReadBody(Stream inputStream)
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (Headers["Content-Length"] != null)
            {
                content_len = Convert.ToInt32(Headers["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    Console.WriteLine("starting Read, to_read={0}", to_read);

                    int numread = inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            InputStream = ms;
            Console.WriteLine("get post data end");
        }

    }
}
