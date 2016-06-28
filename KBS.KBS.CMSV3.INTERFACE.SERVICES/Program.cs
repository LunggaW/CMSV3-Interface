using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace KBS.KBS.CMSV3.INTERFACE.SERVICES
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new ServiceSqlServer() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
