using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusixService.Http
{
    public class HttpUser
    {
        public HttpUser()
        {
            AuthorizationHeader = "";
            AuthorizationMethod = "";
            AuthorizationToken = "";
            Username = "";
            Password = "";
        }

        public string AuthorizationHeader { get; private set; }
        public string AuthorizationMethod { get; private set; }
        public string AuthorizationToken { get; private set; }

        public string Username { get; private set; }
        public string Password { get; private set; }


        public void ParseAuthorization(string authorization)
        {
            AuthorizationHeader = authorization;
            AuthorizationMethod = "";
            AuthorizationToken = "";
            Username = "";
            Password = "";

            if (authorization.Trim().StartsWith("Basic"))
            {
                ParseBasicAuthorization(authorization);
            }
        }

        private void ParseBasicAuthorization(string authorization)
        {
            Console.WriteLine("Basic authorization");
            try
            {
                AuthorizationMethod = "Basic";
                AuthorizationToken = authorization.Replace("Basic", "").Trim();
//                Console.WriteLine("AuthorizationToken: " + AuthorizationToken);
                string usernameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(AuthorizationToken));
//                Console.WriteLine("AuthInfo: " + usernameAndPassword);
                int pos = usernameAndPassword.IndexOf(':');
                if (pos >= 0)
                {
                    Username = usernameAndPassword.Substring(0, pos);
                    Password = usernameAndPassword.Substring(pos + 1);

//                    Console.WriteLine("Username: " + Username);
//                    Console.WriteLine("Password: " + Password);
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Base64 decoding failed");
                // Nothing to do
                // String is not well formatted
            }
        }
    }
}
