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

    public class HttpConnection
    {
        public TcpClient _socket;
        public HttpServer _srv;

        private Stream _socketInputStream;
        private Stream _socketOutputStream;
        
        
        public HttpConnection(TcpClient socket, HttpServer srv)
        {
            this._socket = socket;
            this._srv = srv;
            this._socketInputStream = new BufferedStream(_socket.GetStream());
            this._socketOutputStream = new BufferedStream(_socket.GetStream());
        }
        
        public Stream SocketInputStream
        {
            get { return _socketInputStream; }
        }

        public Stream SocketOutputStream
        {
            get { return _socketOutputStream; }
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
                var context = new HttpContext(this);
                context.Process();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
            }
            _socketOutputStream.Flush();
            _socketOutputStream = null;
            _socketInputStream = null;
            _socket.Close();
        }
        
        public void HandleRequest(HttpContext context)
        {
            _srv.HandleRequest(context);
        }
    }
}
