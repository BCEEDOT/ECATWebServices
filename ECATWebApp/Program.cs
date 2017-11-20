using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Ecat.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                //TODO: Choose environment for deployment
                .UseEnvironment("Development")
                //.UseEnvironment("AWSTesting")
                //.UseEnvironment("AUPublicDev")
                //.UseEnvironment("AUGateway")
                //.UseEnvironment("Production")
                .Build();

            host.Run();
        }
    }
}
