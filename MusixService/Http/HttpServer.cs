using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace MusixService.Http
{
    public class HttpServer
    {
        public delegate void HandleRequestDelegate(HttpContext context);

        /// <summary>
        /// Service is bound to this local network interface.
        /// </summary>
        private IPAddress _localaddr;

        /// <summary>
        /// Service is bound to this local network port.
        /// </summary>
        private int _port;

        /// <summary>
        /// Listener for incoming connections.
        /// </summary>
        private TcpListener _listener;

        /// <summary>
        /// Current service state (started or stopped).
        /// </summary>
        private bool _started;

        /// <summary>
        /// List of client connections.
        /// </summary>
        private List<HttpConnection> _connections;

        private HandleRequestDelegate _callback;
 


        /// <summary>
        /// Create a new HTTP service instance.
        /// </summary>
        /// <param name="localaddr"></param>
        /// <param name="port"></param>
        public HttpServer(HandleRequestDelegate callback)
        {
            _localaddr = IPAddress.Any;
            _port = 80;
            _listener = null;
            _started = false;
            _connections = new List<HttpConnection>();
            _callback = callback;
        }


        public IPAddress Localaddr
        {
            get { lock (this) { return _localaddr; } }
            set
            {
                lock (this)
                {
                    if (_started)
                    {
                        throw new Exception("Not possible while listening");
                    }
                    _localaddr = value;
                }
            }
        }


        public int Port
        {
            get { lock(this) { return _port; } }
            set
            {
                lock (this)
                {
                    if (_started)
                    {
                        throw new Exception("Not possible while listening");
                    }
                    _port = value;
                }
            }
        }


        public bool IsStarted { get { return _started; } }
        
        
        public void Start()
        {
            lock (this)
            {
                if (_started)
                {
                    return;  // service is already started
                }

                _started = true;
                _listener = new TcpListener(_localaddr, _port);
                _listener.Start();

                ThreadPool.QueueUserWorkItem(new WaitCallback(DoListening), _listener);
            }
        }


        public void Stop()
        {
            lock (this)
            {
                if (!_started)
                {
                    return;  // service is already stopped
                }

                _listener.Stop();
                _listener.Server.Close();
                _listener = null;
                _started = false;
            }
        }







        //private void Run()
        //{
            
            


        //    ThreadPool.QueueUserWorkItem((o) =>
        //    {
        //        var listener = o as TcpListener;
        //        try
        //        {
        //            while (listener == _listener && _started)
        //            {
        //                ThreadPool.QueueUserWorkItem((c) =>
        //                {
        //                    var client = c as TcpClient;
        //                    var connection = new HttpConnection(client, this);
        //                    connection.Process();
        //                    try
        //                    {
        //                        RequestRouter(ctx);
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        // suppress any exceptions

        //                        Console.WriteLine("Exception: " + e.Message);

        //                    }
        //                    finally
        //                    {
        //                        // always close the stream
        //                        ctx.Response.OutputStream.Close();
        //                    }
        //                }, listener.AcceptTcpClient());
        //            }
        //        }
        //        catch { } // suppress any exceptions
        //    }, _listener );
        //}


        private void DoListening(object state)
        {
            if (state is TcpListener)
                DoListening(state as TcpListener);
            else
                throw new ArgumentException("state is not of type TcpListener", "state");
        }

        private void DoListening(TcpListener listener)
        {
            try
            {
                while (listener == _listener && _started)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(DoConnection), listener.AcceptTcpClient());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private void DoConnection(object state)
        {
            if (state is TcpClient)
                DoConnection(state as TcpClient);
            else
                throw new ArgumentException("state is not of type TcpClient", "state");
        }

        private void DoConnection(TcpClient client)
        {
            try
            {
                HandleConnection(new HttpConnection(client, this));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private void HandleConnection(HttpConnection connection)
        {
            lock (_connections)
            {
                _connections.Add(connection);
            }

            try
            {
                connection.Process();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            lock (_connections)
            {
                _connections.Remove(connection);
            }
        }

        public void HandleRequest(HttpContext context)
        {
            if (_callback != null)
                _callback(context);   
        }
        

        public HttpConnection[] Connections
        {
            get
            {
                lock (this)
                {
                    return _connections.ToArray();
                }
            }
        }
        

        //public HttpContext GetContext()
        //{
        //    TcpClient s = _listener.AcceptTcpClient();
        //    HttpContext processor = new HttpContext(s, this);
        //    return processor;
        //}
    }
}










