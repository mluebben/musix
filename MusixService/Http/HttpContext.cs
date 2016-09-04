using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MusixService.Http
{

    public class HttpContext
    {
        private HttpConnection _connection;
        private HttpRequest _request;
        private HttpResponse _response;
        
        
        public HttpContext(HttpConnection connection)
        {
            this._connection = connection;
            this._request = new HttpRequest();
            this._response = new HttpResponse();
//            this._request.Read(_connection.SocketInputStream);
        }

        public HttpConnection Connection
        {
            get { return _connection; }
        }

        
        public HttpRequest Request
        {
            get { return _request; }
        }


        public HttpResponse Response
        {
            get { return _response; }
        }


        //public void Finish()
        //{
        //    Response.Write(_socketOutputStream);
        //    _socketOutputStream.Flush();
        //    _socketInputStream = null;
        //    _socketOutputStream = null;
        //    _socket.Close();
        //}

        public void Process()
        {
            try
            {
                Request.Read(Connection.SocketInputStream);
                HandleRequest();
                Response.Write(Connection.SocketOutputStream);
                //Response.WriteResponse(outputStream);
                //Response.WriteHeaders(outputStream);
                //Response.WriteBody(outputStream);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
        }



        public void HandleRequest()
        {
            Connection.HandleRequest(this);
        }
    }
}
