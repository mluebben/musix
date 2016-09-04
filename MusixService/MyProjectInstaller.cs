using System;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;
using System.Reflection;

namespace MusixService
{
    [RunInstaller(true)]
    public class MyProjectInstaller : Installer
    {
        public MyProjectInstaller()
        {
            this.Installers.Add(GetServiceInstaller());
            this.Installers.Add(GetServiceProcessInstaller());
        }

        private ServiceInstaller GetServiceInstaller()
        {
            var installer = new ServiceInstaller();
            installer.StartType = ServiceStartMode.Manual;
            installer.ServiceName = "MusixService";
            installer.DisplayName = "Musix service";
            installer.Description = "Streams music to Musix clients.";
            return installer;
        }

        private ServiceProcessInstaller GetServiceProcessInstaller()
        {
            var installer = new ServiceProcessInstaller();
            installer.Account = ServiceAccount.NetworkService;
            return installer;
        }
    }
}
