using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace DeployStatus
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            var staticFilesPath = GetStaticFilesPath();
            var fileSystem = new PhysicalFileSystem(staticFilesPath);

            var fileServerOptions = new FileServerOptions
            {
                EnableDefaultFiles = true,
                FileSystem = fileSystem
            };
            fileServerOptions.StaticFileOptions.FileSystem = fileSystem;
            fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            fileServerOptions.DefaultFilesOptions.DefaultFileNames = new[] { "index.html", "index.htm" };

            app.UseFileServer(fileServerOptions);
        }

        private static string GetStaticFilesPath()
        {
            if (Debugger.IsAttached)
            {
                var currentPath = Directory.GetCurrentDirectory();
                var newPath = Regex.Replace(currentPath, @"bin\\.*", "Web");
                if (Directory.Exists(newPath))
                    return newPath;
            }

            return @".\Web";
        }
    }
}