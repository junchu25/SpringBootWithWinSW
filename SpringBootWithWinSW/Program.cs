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
            map.Add("options", String.Empty);

            for (var i = 0; i < args.Length; ++i)
            {
                var argName = args[i].ToLower();

                if (argName == "-id")
                {
                    map.Add("id", args[++i]);
                }
                else if (argName == "-file")
                {
                    map.Add("file", args[++i]);
                }
                else
                {
                    map["options"] += argName;
                }
            }

            return map;
        }

        static void DoDeploy(IDictionary<string, string> args)
        {
            var id = args["id"];
            var jarFile = args["file"];
            var options = args["options"];
            var jarFolder = Path.GetDirectoryName(jarFile);
            var winswSource = ConfigurationManager.AppSettings["winswSource"];

            CopyWinSWToDeployFolder(id, winswSource, jarFolder);
            UpdateWinSWConfig(id, jarFolder, jarFile, options);
        }

        static void CopyWinSWToDeployFolder(string id, string winswSource, string destFolder)
        {
            var winswExeFile = Path.Combine(winswSource, "WinSW.NET4.exe");
            var winswConfigFile = Path.Combine(winswSource, "sample-allOptions.xml");

            File.Copy(winswExeFile, Path.Combine(destFolder, $"{id}.exe"), true);
            File.Copy(winswConfigFile, Path.Combine(destFolder, $"{id}.xml"), true);
        }

        static void UpdateWinSWConfig(string id, string path, string jarFile, string options)
        {
            var configFile = Path.Combine(path, $"{id}.xml");
            var rootXElement = XElement.Load(configFile);
            var javaOpts = Environment.GetEnvironmentVariable("JAVA_OPTS") ?? "-Xms128m -Xmx256m";

            SetXElementValue(rootXElement, "id", id);
            SetXElementValue(rootXElement, "name", id);
            SetXElementValue(rootXElement, "description", id);
            SetXElementValue(rootXElement, "executable", "java");
            SetXElementValue(rootXElement, "arguments", $"-jar {javaOpts} {jarFile} {options}");

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
