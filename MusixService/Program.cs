using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceProcess;

namespace MusixService
{
    class Program
    {
        private static Config mConfig = null;

        public static Config Config {
            get
            {
                if (mConfig == null)
                {
                    string fileName = Assembly.GetAssembly(typeof(Program)).Location + ".xml";

                    Debug.WriteLine("Load configuration from file: " + fileName);

                    mConfig = new Config();
                    mConfig.ParseFile(fileName);
                }
                return mConfig;
            }
        }

        static object[] services = new object[] {
           new MusixJsonRpcService()
        };

        static void Main(string[] args)
        {
            //var a = new FanartTv.Music.Artist(;
            //a.List.AImagesrtistbackground[0].Url

            //Entities.Scraper s = new Entities.Scraper();
            //s.Run();
            //return;


            var service = new MusixWindowsService();

            if (Environment.UserInteractive)
            {
                service.CallOnStart(args);
                Console.WriteLine("Press any key to stop program");
                Console.Read();
                service.CallOnStop();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}
