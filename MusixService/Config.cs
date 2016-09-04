using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace MusixService
{
    class Config
    {
        private string mKodiConnectionString = "Server=192.168.178.25;Uid=ml;Pwd=ml;Database=mymusic56;";
        private string mKodiHostname = "192.168.178.25";
        private int mKodiHostport = 8080;
        private string mKodiUsername = "kodi";
        private string mKodiPassword = "kodi";

        private List<User> mUsers = new List<User>();

        private int mHostport = 9080;

        public Config()
        {
            mUsers.AddRange(new User[]
            {
                new User()
                {
                    Username = "ml",
                    Password = "ml"
                }
            });
        }


        public string KodiConnectionString
        {
            get { return mKodiConnectionString; }
        }

        public string KodiHostname
        {
            get { return mKodiHostname; }
        }

        public int KodiHostport
        {
            get { return mKodiHostport; }
        }

        public string KodiUsername
        {
            get { return mKodiUsername; }
        }

        public string KodiPassword
        {
            get { return mKodiPassword; }
        }

        public List<User> Users
        {
            get { return mUsers; }
        }

        public int Hostport
        {
            get { return mHostport; }
        }


        public void ParseFile(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    return;
                }

                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);
                mKodiConnectionString = ReadString(doc, "KodiConnectionString", mKodiConnectionString);
                mKodiHostname = ReadString(doc, "KodiHostname", mKodiHostname);
                mKodiHostport = ReadInt(doc, "KodiHostport", mKodiHostport);
                mKodiUsername = ReadString(doc, "KodiUsername", mKodiUsername);
                mKodiPassword = ReadString(doc, "KodiPassword", mKodiPassword);
                mHostport = ReadInt(doc, "Hostport", mHostport);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }


        private string ReadString(XmlDocument xml, string name, string defaultValue)
        {
            string xpath = "//" + name;

            XmlNodeList nodeList = xml.DocumentElement.SelectNodes(xpath);
            if (nodeList.Count == 0)
            {
                return defaultValue;
            }

            XmlNode node = nodeList[0];

            string value = node.InnerText;
            return value;
        }


        private long ReadLong(XmlDocument xml, string name, long defaultValue)
        {
            string value = ReadString(xml, name, Convert.ToString(defaultValue));
            long n = Convert.ToInt64(value);
            return n;
        }


        private int ReadInt(XmlDocument xml, string name, long defaultValue)
        {
            string value = ReadString(xml, name, Convert.ToString(defaultValue));
            int n = Convert.ToInt32(value);
            return n;
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
