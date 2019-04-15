using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

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
            UpdateWinSWConfig(id, jarFolder, jarFile);
        }

        static void CopyWinSWToDeployFolder(string id, string winswSource, string destFolder)
        {
            var winswExeFile = Path.Combine(winswSource, "WinSW.NET4.exe");
            var winswConfigFile = Path.Combine(winswSource, "sample-allOptions.xml");

            File.Copy(winswExeFile, Path.Combine(destFolder, $"{id}.exe"), true);
            File.Copy(winswConfigFile, Path.Combine(destFolder, $"{id}.xml"), true);
        }

        static void UpdateWinSWConfig(string id, string path, string jarFile)
        {
            var configFile = Path.Combine(path, $"{id}.xml");
            var rootXElement = XElement.Load(configFile);

            SetXElementValue(rootXElement, "id", id);
            SetXElementValue(rootXElement, "name", id);
            SetXElementValue(rootXElement, "description", id);
            SetXElementValue(rootXElement, "executable", "java");
            SetXElementValue(rootXElement, "arguments", $"-jar {jarFile}");

            rootXElement.Save(configFile);
        }

        static void SetXElementValue(XElement rootXElement, XName name, string value)
        {
            var xElement = rootXElement.Element(name);

            if (xElement != null)
            {
                xElement.Value = value;
            }
            else
            {
                rootXElement.Add(new XElement(name, value));
            }
        }
    }
}
