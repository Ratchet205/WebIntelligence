using System;
using System.Collections.Generic;
using System.IO;
using WebHttpsServer.server;
using Newtonsoft.Json.Linq;

namespace WebHttpsServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the path to the JSON-Config/s file as an argument.");
                return;
            }

            try
            {
                // Read and parse the JSON configuration file
                string json_config = File.ReadAllText(args[0]);
                var config = JObject.Parse(json_config);

                // Accessing the SetUp section correctly
                var setUpConfig = config["Config"]?["SetUp"];
                var aliasesConfig = config["Config"]?["Aliases"];

                string staticFolder = setUpConfig?["StaticFolder"]?.ToString() ?? "public";
                string[] urls = setUpConfig?["Prefixes"]?.ToObject<string[]>() ?? ["http://localhost:8080/"];
                Dictionary<string, string> aliases = aliasesConfig?.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>();

                // Setting up the server with the extracted configuration
                WebServer.SetUp(staticFolder, urls);

                // Adding aliases from the configuration
                foreach (var alias in aliases)
                {
                    WebServer.AddAlias(alias.Key, alias.Value);
                }

                // Starting the server
                WebServer.Start();

                Console.WriteLine("Server started. Press any key to stop...");
                WebServer.KeepAliveUntilKey('x');
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
