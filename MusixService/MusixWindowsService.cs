/////////////////////////////////////////////////////////////////////////////
// $Id $
// Copyright (C) 2015 Matthias Lübben <ml81@gmx.de>
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either
// version 2 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
// Purpose:      JSON-RPC service for android app Play Kodi
// Created:      07.04.2015 (dd.mm.yyyy)
/////////////////////////////////////////////////////////////////////////////

using System;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using AustinHarris.JsonRpc;
using System.Diagnostics;
using MusixService.Http;

namespace MusixService
{
    public class MusixWindowsService : ServiceBase
    {
        private readonly Regex _rxJsonRpcPattern = new Regex("^/jsonrpc$");
        private readonly Regex _rxStreamPattern = new Regex("^/stream/([0-9]+)$");
        private readonly Regex _rxArtPattern = new Regex("^/art/([0-9]+)$");

        private HttpServer _httpServer = null;





        public MusixWindowsService()
        {
            // Nothing to do
        }

        
        public void CallOnStart(string[] args)
        {
            this.OnStart(args);
        }

        public void CallOnStop()
        {
            this.OnStop();
        }


        protected override void OnStart(string[] args)
        {
            _httpServer = new HttpServer(new HttpServer.HandleRequestDelegate(HandleRequest));
            _httpServer.Localaddr = IPAddress.Any;
            _httpServer.Port = Program.Config.Hostport;
            _httpServer.Start();

            if (Environment.UserInteractive )
            {
                Console.WriteLine(string.Format("Musix service is listening on {0}:{1}", _httpServer.Localaddr.ToString(), _httpServer.Port));
            }
        }

        protected override void OnStop()
        {
            if (_httpServer != null)
            {
                _httpServer.Stop();
                _httpServer = null;
            }
        }

        private void HandleRequest(HttpContext context)
        {
            RequestRouter(context);
        }


        private void RequestRouter(HttpContext ctx)
        {
            string uri = ctx.Request.Resource;
            Match m;

            Console.WriteLine("Request: " + uri);

            var username = ctx.Request.User.Username;
            var password = ctx.Request.User.Password;

            bool authorized = false;
            foreach (var user in Program.Config.Users)
            {
                if (user.Username.ToLower() == username.ToLower() && user.Password == password)
                {
                    authorized = true;
                    break;   
                }
            }
            if (!authorized)
            {
                Console.WriteLine("Unauthorized " + username + ":" + password);
                Unauthorized(ctx);
                return;
            }
            
            if ((m = _rxJsonRpcPattern.Match(uri)).Success)
            {
                JsonRpc(ctx);
            } 
            else if ((m = _rxStreamPattern.Match(uri)).Success)
            {
                int id = Convert.ToInt32(m.Groups[1].Value);
                Stream(ctx, id);
            }
            else if ((m = _rxArtPattern.Match(uri)).Success)
            {
                int id = Convert.ToInt32(m.Groups[1].Value);
                Art(ctx, id);
            }
            else
            {
                Static(ctx);
                //NotFound(ctx);
            }
        }


        private void Static(HttpContext ctx)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string webDirectory = System.IO.Path.Combine(appDirectory, "Client");
            string path = webDirectory;
            
            foreach (string directory in ctx.Request.Resource.Split('/'))
            {
                if (String.IsNullOrEmpty(directory))
                {
                    continue;
                }

                if (directory.Contains(".."))
                {
                    NotFound(ctx);
                    return;
                }
                
                path = System.IO.Path.Combine(path, directory);
            }

            if (Directory.Exists(path))
            {
                string indexFile = System.IO.Path.Combine(path, "index.html");
                if (File.Exists(indexFile))
                {
                    path = indexFile;
                    if (Directory.Exists(path))
                    {
                        Forbidden(ctx);
                        return;
                    }
                }
                else
                {
                    Forbidden(ctx);
                    return;
                }
            }
           
            if (!File.Exists(path))
            {
                NotFound(ctx);
                return;
            }

            byte[] buf = File.ReadAllBytes(path);

            string contentType = MimeTypes.GetMimeType(System.IO.Path.GetExtension(path));

            ctx.Response.ContentType = contentType;
            ctx.Response.ContentLength = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
        }


        private void JsonRpc(HttpContext ctx)
        {
            var myContext = new MusixJsonRpcContext()
            {
                User = new MyUser
                {
                    Name = ctx.Request.User.Username
                }
            };

            
            string jsonRpcRequest = new StreamReader(ctx.Request.InputStream).ReadToEnd();
            string jsonRpcResponse = JsonRpcProcessor.Process(jsonRpcRequest, myContext).Result;

            byte[] buf = Encoding.UTF8.GetBytes(jsonRpcResponse);
            ctx.Response.ContentLength = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
//            ctx.Finish();
        }


        private void Stream(HttpContext ctx, int id)
        {
            Database db = new Database();
            db.ConnectionString = Program.Config.KodiConnectionString;

            string songPath = db.GetSongPath(id);
            if (songPath == null)
            {
                NotFound(ctx);
                return;
            }

            string url = "http://" + Program.Config.KodiHostname + ":" + Program.Config.KodiHostport + "/vfs/" + Uri.EscapeDataString(songPath);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);

            request.UseDefaultCredentials = true;
            request.Credentials = new NetworkCredential(Program.Config.KodiUsername, Program.Config.KodiPassword);
            
            string range = ctx.Request.Headers.Get("Range");
            if (range != null)
            {
                string pattern = "([0-9]+)-([0-9]+)?";
                Match m = Regex.Match(range, pattern);

                //string from = m.Captures[1].Value;
                //string to = m.Captures[2].Value;

                string from = m.Groups[1].Value;
                string to = m.Groups[2].Value;

                if (string.IsNullOrEmpty(to))
                {
                    request.AddRange(Convert.ToInt32(from));
                }
                else
                {
                    request.AddRange(Convert.ToInt32(from), Convert.ToInt32(to));
                }
            }

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            Stream stream = response.GetResponseStream();
            
            ctx.Response.StatusCode = (int) response.StatusCode;
            ctx.Response.StatusDescription = response.StatusDescription;
            ctx.Response.ContentType = response.ContentType;
            ctx.Response.ContentLength = response.ContentLength;

            // Stream data
            byte[] buffer = new byte[4096];
            int count;
            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ctx.Response.OutputStream.Write(buffer, 0, count);
            }
//            ctx.Finish();
        }


        private void Art(HttpContext ctx, int id)
        {
            Database db = new Database();
            db.ConnectionString = Program.Config.KodiConnectionString;

            Art art = db.GetArtById(id);
            if (art == null)
            {
                NotFound(ctx);
                return;
            }

            byte[] buf = null;
            try {
                if (art.strUrl.StartsWith("http://"))
                {
                    using (WebClient client = new WebClient())
                    {
                        buf = client.DownloadData(art.strUrl);
                    }
                }
                else if (art.strUrl.StartsWith("smb://"))
                {
                    buf = File.ReadAllBytes("\\\\" + art.strUrl.Substring(6).Replace('/', '\\'));
                }
                else if (art.strUrl.StartsWith("image://"))
                {
                    buf = DownloadFromKodi(art.strUrl);
                }
                else if (File.Exists(art.strUrl))
                {
                    buf = File.ReadAllBytes(art.strUrl);
                }
                else
                {
                    buf = DownloadFromKodi(art.strUrl);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                buf = null;
            }

            if (buf == null)
            {
                NotFound(ctx);
                return;
            }

            //string rstr = "Art (" + id + ")";
            //byte[] buf = Encoding.UTF8.GetBytes(rstr);
            ctx.Response.ContentType = "image/jpeg";
            ctx.Response.ContentLength = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
//            ctx.Finish();
        }



        


        private byte[] DownloadFromKodi(string path)
        {
            using (WebClient client = new WebClient())
            {
                client.UseDefaultCredentials = true;
                client.Credentials = new NetworkCredential(Program.Config.KodiUsername, Program.Config.KodiPassword);
                string url = "http://" + Program.Config.KodiHostname + ":" + Program.Config.KodiHostport + "/vfs/" + Uri.EscapeDataString(path);
                return client.DownloadData(url);
            }
        }
        

        private void Unauthorized(HttpContext ctx)
        {
            ctx.Response.StatusCode = 401;
            ctx.Response.StatusDescription = "Unauthorized";

            ctx.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Musix\"";

            string rstr = "Unauthorized";
            byte[] buf = Encoding.UTF8.GetBytes(rstr);
            //ctx.Response.ContentLength = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
//            ctx.Finish();
        }


        private void NotFound(HttpContext ctx)
        {
            ctx.Response.StatusCode = 404;
            ctx.Response.StatusDescription = "Not found";

            string rstr = "Not found";
            byte[] buf = Encoding.UTF8.GetBytes(rstr);
            //ctx.Response.ContentLength = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
//            ctx.Finish();
        }


        private void Forbidden(HttpContext ctx)
        {
            ctx.Response.StatusCode = 403;
            ctx.Response.StatusDescription = "Forbidden";

            string rstr = "Forbidden";
            byte[] buf = Encoding.UTF8.GetBytes(rstr);
            //ctx.Response.ContentLength = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
            //            ctx.Finish();
        }

    }
}


















/////////////////////////////////////////////////////////////////////////////
// $Id $
// Copyright (C) 2015 Matthias Lübben <ml81@gmx.de>
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either
// version 2 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
// Purpose:      JSON-RPC service for android app Play Kodi
// Created:      07.04.2015 (dd.mm.yyyy)
/////////////////////////////////////////////////////////////////////////////

//using System;
//using System.ServiceProcess;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Net;
//using System.Threading;
//using Newtonsoft.Json;
//using System.IO;
//using AustinHarris.JsonRpc;
//using System.Diagnostics;

//namespace MusixService
//{
//    public class MusixWindowsService : ServiceBase
//    {
//        private readonly Regex _rxJsonRpcPattern = new Regex("^/jsonrpc$");
//        private readonly Regex _rxStreamPattern = new Regex("^/stream/([0-9]+)$");
//        private readonly Regex _rxArtPattern = new Regex("^/art/([0-9]+)$");

//        private HttpListener _httpListener;

//        public MusixWindowsService()
//        {
//            if (!HttpListener.IsSupported)
//            {
//                throw new NotSupportedException("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
//            }

//            string url = "http://+:9080/";

//            if (Environment.UserInteractive)
//            {
//                AddAddress(url, Environment.UserDomainName, Environment.UserName);
//            }

//            _httpListener = new HttpListener();
//            _httpListener.Prefixes.Add(url);
//            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
//        }


//        private void AddAddress(string address, string domain, string user)
//        {
//            string args = string.Format(@"http add urlacl url={0}", address) + " user=\"" + domain + "\\" + user + "\"";

//            ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
//            psi.Verb = "runas";
//            psi.CreateNoWindow = true;
//            psi.WindowStyle = ProcessWindowStyle.Hidden;
//            psi.UseShellExecute = true;

//            Process.Start(psi).WaitForExit();
//        }

//        public void CallOnStart(string[] args)
//        {
//            this.OnStart(args);
//        }

//        public void CallOnStop()
//        {
//            this.OnStop();
//        }

//        protected override void OnStart(string[] args)
//        {
//            if (_httpListener == null)
//            {
//                return;  // Nothing to do
//            }

//            _httpListener.Start();
//            Run();
//        }

//        protected override void OnStop()
//        {
//            if (_httpListener == null)
//            {
//                return;  // Nothing to do
//            }

//            _httpListener.Stop();
//            _httpListener.Close();
//        }

//        private void Run()
//        {
//            ThreadPool.QueueUserWorkItem((o) =>
//            {
//                try
//                {
//                    while (_httpListener.IsListening)
//                    {
//                        ThreadPool.QueueUserWorkItem((c) =>
//                        {
//                            var ctx = c as HttpListenerContext;
//                            try
//                            {
//                                RequestRouter(ctx);
//                            }
//                            catch { } // suppress any exceptions
//                            finally
//                            {
//                                // always close the stream
//                                ctx.Response.OutputStream.Close();
//                            }
//                        }, _httpListener.GetContext());
//                    }
//                }
//                catch { } // suppress any exceptions
//            });
//        }


//        private void RequestRouter(HttpListenerContext ctx)
//        {
//            string uri = ctx.Request.Url.AbsolutePath;
//            Match m;

//            Console.WriteLine("Request: " + uri);

//            HttpListenerBasicIdentity identity = (HttpListenerBasicIdentity)ctx.User.Identity;
//            var username = identity.Name;
//            var password = identity.Password;

//            bool authorized = false;
//            foreach (var user in Program.Config.Users)
//            {
//                if (user.Username.ToLower() == username.ToLower() && user.Password == password)
//                {
//                    authorized = true;
//                    break;
//                }
//            }
//            if (!authorized)
//            {
//                Console.WriteLine("Unauthorized " + username + ":" + password);
//                Unauthorized(ctx);
//                return;
//            }

//            if ((m = _rxJsonRpcPattern.Match(uri)).Success)
//            {
//                JsonRpc(ctx);
//            }
//            else if ((m = _rxStreamPattern.Match(uri)).Success)
//            {
//                int id = Convert.ToInt32(m.Groups[1].Value);
//                Stream(ctx, id);
//            }
//            else if ((m = _rxArtPattern.Match(uri)).Success)
//            {
//                int id = Convert.ToInt32(m.Groups[1].Value);
//                Art(ctx, id);
//            }
//            else
//            {
//                NotFound(ctx);
//            }
//        }


//        private void JsonRpc(HttpListenerContext ctx)
//        {
//            if (ctx.User.Identity.IsAuthenticated)
//            {
//                string jsonRpcRequest = new StreamReader(ctx.Request.InputStream).ReadToEnd();
//                string jsonRpcResponse = JsonRpcProcessor.Process(jsonRpcRequest, ctx).Result;

//                byte[] buf = Encoding.UTF8.GetBytes(jsonRpcResponse);
//                ctx.Response.ContentLength64 = buf.Length;
//                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
//            }
//            else
//            {
//                Unauthorized(ctx);
//            }
//        }


//        private void Stream(HttpListenerContext ctx, int id)
//        {
//            Database db = new Database();
//            db.ConnectionString = Program.Config.KodiConnectionString;

//            string songPath = db.GetSongPath(id);
//            if (songPath == null)
//            {
//                NotFound(ctx);
//                return;
//            }







//            using (WebClientEx client = new WebClientEx())
//            {
//                client.UseDefaultCredentials = true;
//                client.Credentials = new NetworkCredential(Program.Config.KodiUsername, Program.Config.KodiPassword);

//                string range = ctx.Request.Headers.Get("Range");
//                if (range != null)
//                {
//                    client.Headers["Range"] = range;
//                }

//                string url = "http://" + Program.Config.KodiHostname + ":" + Program.Config.KodiHostport + "/vfs/" + Uri.EscapeDataString(songPath);
//                Stream stream = client.OpenRead(url);

//                ctx.Response.StatusCode = (int)client.StatusCode;
//                ctx.Response.StatusDescription = client.StatusDescription;
//                ctx.Response.ContentType = client.ResponseHeaders["Content-Type"];
//                ctx.Response.ContentLength64 = Convert.ToInt32(client.ResponseHeaders["Content-Length"]);

//                // Stream data
//                byte[] buffer = new byte[4096];
//                int count;
//                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
//                {
//                    ctx.Response.OutputStream.Write(buffer, 0, count);
//                }
//            }


//            //string rstr = "Stream (" + id + ")";
//            //byte[] buf = Encoding.UTF8.GetBytes(rstr);
//            //ctx.Response.ContentLength64 = buf.Length;
//            //ctx.Response.OutputStream.Write(buf, 0, buf.Length);
//        }


//        private void Art(HttpListenerContext ctx, int id)
//        {
//            Database db = new Database();
//            db.ConnectionString = Program.Config.KodiConnectionString;

//            Art art = db.GetArtById(id);
//            if (art == null)
//            {
//                NotFound(ctx);
//                return;
//            }

//            byte[] buf = null;
//            try
//            {
//                if (art.strUrl.StartsWith("http://"))
//                {
//                    using (WebClient client = new WebClient())
//                    {
//                        buf = client.DownloadData(art.strUrl);
//                    }
//                }
//                else if (art.strUrl.StartsWith("smb://"))
//                {
//                    buf = File.ReadAllBytes("\\\\" + art.strUrl.Substring(6).Replace('/', '\\'));
//                }
//                else if (art.strUrl.StartsWith("image://"))
//                {
//                    buf = DownloadFromKodi(art.strUrl);
//                }
//                else if (File.Exists(art.strUrl))
//                {
//                    buf = File.ReadAllBytes(art.strUrl);
//                }
//                else
//                {
//                    buf = DownloadFromKodi(art.strUrl);
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.Message);
//                buf = null;
//            }

//            if (buf == null)
//            {
//                NotFound(ctx);
//                return;
//            }






//            //string rstr = "Art (" + id + ")";
//            //byte[] buf = Encoding.UTF8.GetBytes(rstr);
//            ctx.Response.ContentType = "image/jpeg";
//            ctx.Response.ContentLength64 = buf.Length;
//            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
//        }






//        private byte[] DownloadFromKodi(string path)
//        {
//            using (WebClient client = new WebClient())
//            {
//                client.UseDefaultCredentials = true;
//                client.Credentials = new NetworkCredential(Program.Config.KodiUsername, Program.Config.KodiPassword);
//                string url = "http://" + Program.Config.KodiHostname + ":" + Program.Config.KodiHostport + "/vfs/" + Uri.EscapeDataString(path);
//                return client.DownloadData(url);
//            }
//        }



//        private void Unauthorized(HttpListenerContext ctx)
//        {
//            ctx.Response.StatusCode = 401;
//            ctx.Response.StatusDescription = "Unauthorized";

//            string rstr = "Unauthorized";
//            byte[] buf = Encoding.UTF8.GetBytes(rstr);
//            ctx.Response.ContentLength64 = buf.Length;
//            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
//        }


//        private void NotFound(HttpListenerContext ctx)
//        {
//            ctx.Response.StatusCode = 404;
//            ctx.Response.StatusDescription = "Not found";

//            string rstr = "Not found";
//            byte[] buf = Encoding.UTF8.GetBytes(rstr);
//            ctx.Response.ContentLength64 = buf.Length;
//            ctx.Response.OutputStream.Write(buf, 0, buf.Length);


//            //string rstr = "<HTML><BODY>My web page.</BODY></HTML>";
//            //byte[] buf = Encoding.UTF8.GetBytes(rstr);
//            //ctx.Response.ContentLength64 = buf.Length;
//            //ctx.Response.OutputStream.Write(buf, 0, buf.Length);
//        }

//    }

//    class WebClientEx : WebClient
//    {
//        private WebResponse m_Resp = null;

//        protected override WebResponse GetWebResponse(WebRequest Req, IAsyncResult ar)
//        {
//            return m_Resp = base.GetWebResponse(Req, ar);
//        }

//        public HttpStatusCode StatusCode
//        {
//            get
//            {
//                if (m_Resp != null && m_Resp is HttpWebResponse)
//                    return (m_Resp as HttpWebResponse).StatusCode;
//                else
//                    return HttpStatusCode.OK;
//            }
//        }

//        public string StatusDescription
//        {
//            get
//            {
//                if (m_Resp != null && m_Resp is HttpWebResponse)
//                    return (m_Resp as HttpWebResponse).StatusDescription;
//                else
//                    return "OK";
//            }
//        }

//    }

//}
