using System;
using System.Collections.Generic;

namespace SpringBootWithWinSW
{
    class Program
    {
        static void Main(string[] args)
        {
            var argMap = ParseArguments(args);
            

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
    }
}
