using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace SpringBootWithWinSW
{
    class Program
    {
        static void Main(string[] args)
        {
            var argMap = ParseArguments(args);

            DoDeploy(argMap);
        }

        static IDictionary<string, string> ParseArguments(string[] args)
        {
            var map = new Dictionary<string, string>();

            for (var i = 0; i < args.Length; i += 2)
            {
                map.Add(args[i].Substring(1), args[i + 1]);
            }

            return map;
        }

        static void DoDeploy(IDictionary<string, string> args)
        {
            var id = args["id"];
            var jarFile = args["file"];
            var jarFolder = Path.GetDirectoryName(jarFile);
            var winswSource = ConfigurationManager.AppSettings["winswSource"];

            CopyWinSWToDeployFolder(id, winswSource, jarFolder);
        }

        static void CopyWinSWToDeployFolder(string id, string winswSource, string destFolder)
        {
            var winswExeFile = Path.Combine(winswSource, "WinSW.NET4.exe");
            var winswConfigFile = Path.Combine(winswSource, "sample-allOptions.xml");

            File.Copy(winswExeFile, Path.Combine(destFolder, $"{id}.exe"));
            File.Copy(winswConfigFile, Path.Combine(destFolder, $"{id}.xml"));
        }
    }
}
