using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ASP.NETCoreWebAPiResumeDownload
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Switch.Microsoft.AspNetCore.Mvc.EnableRangeProcessing", true);
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
