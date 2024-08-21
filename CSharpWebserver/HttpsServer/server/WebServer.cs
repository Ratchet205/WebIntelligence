using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using HeyRed.Mime;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHttpsServer.server
{
    public static class WebServer
    {
        private static bool set_up = false;
        private static string? StaticFolder;
        private static Dictionary<string, string> AliasedURLs = new();
        private static List<string> prefixes = [];

        private static CancellationTokenSource? cts;

        private static async Task ServerMain(CancellationToken token)
        {
            HttpListener listener = new HttpListener();
            Console.WriteLine("Created HttpListener..");
            foreach (var prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
            Console.WriteLine("Prefixes Added..");

            try { listener.Start(); } catch (UnauthorizedAccessException e) { Console.WriteLine(e.Message +"\n\n\n\nRun this Program as Administrator if using Privileged Port (80, 443, etc.) as Prefix"); Console.ReadKey(true); }
            Console.WriteLine("HttpListener started..");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    _ = Task.Run(() =>
                    {
                        HttpListenerContext context = listener.GetContext();
                        string ContextIP = context.Request.RemoteEndPoint?.Address.ToString() ?? "Unknown IP";
                        string reqPath = context.Request.RawUrl == null ? "/" : context.Request.RawUrl;
                        DateTime reqTime = DateTime.Now;

                        Console.WriteLine($"[{reqTime}] Request from {ContextIP} for path {reqPath}");


                        var res = context.Response;
                        var req = context.Request;
                        try
                        {
                            byte[] buffer;
                            string resPath = AliasedURLs.TryGetValue(reqPath, out var path)
                                ? path
                                : req.RawUrl == "/"
                                    ? StaticFolder + "/index.html"
                                    : StaticFolder + req.RawUrl;
                            if(File.Exists(resPath))
                            {
                                string mimeType = MimeTypesMap.GetMimeType(resPath);
                                using(var fileStream = File.OpenRead(resPath))
                                {
                                    buffer = new byte[fileStream.Length];
                                    fileStream.Read(buffer, 0, buffer.Length);
                                }
                                res.ContentType = mimeType;
                            } else
                            {
                                buffer = Encoding.UTF8.GetBytes("File not Found");
                                res.StatusCode = 404;
                                res.ContentType = "text/plain";
                            }

                            res.ContentLength64 = buffer.Length;
                            using(var output = res.OutputStream)
                            {
                                output.Write(buffer, 0, buffer.Length);
                            }
                        } catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing request: {ex.Message}");
                            res.StatusCode=500;
                        } finally
                        {
                            res.OutputStream.Close();
                        }
                    });
                }catch (HttpListenerException ex) when (ex.Message.Contains("The operation was canceled"))
                {
                    Console.WriteLine("Server stopped listening for new Connections.");
                    break;
                }catch (Exception ex)
                {
                    Console.WriteLine($"Unhandled exception in ServerMain: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Sets up the Servers Static Folder and the URLs it's available on.
        /// </summary>
        /// <param name="staticFolder">string Path to the Static Folder.</param>
        /// <param name="prefixes">string URL the Server serves/reacts to.</param>
        public static void SetUp(string staticFolder, params string[]? prefixes)
        {
            if (!staticFolder.Equals("/"))
            {
                if (staticFolder.EndsWith('/')) { staticFolder = staticFolder.TrimEnd('/'); }
            }

            StaticFolder = staticFolder;

            if (prefixes != null)
            {
                for(int i = 0; i < prefixes.Length; i++)
                {
                    WebServer.prefixes.Add(prefixes[i]);
                }
                set_up = true;
            }
        }
        /// <summary>
        /// Adds an alias to the AliasedURLs Dictionary, enabling the possiblity to make a certain Actual path available via an alias.
        /// </summary>
        /// <param name="alias">Shortened URL/Alias.</param>
        /// <param name="actual">The Actual Path to the to be served File.</param>
        /// <exception cref="Exception"></exception>
        public static void AddAlias(string alias, string actual)
        {
            if (!alias.StartsWith("/"))
            {
                throw new Exception("The alias has to start with \"/\"");
            }
            AliasedURLs.Add(alias, actual);
        }
        /// <summary>
        /// Removes one or multiple Aliases form the "AliasedURLs" Dictionary.
        /// </summary>
        /// <param name="aliases">The Dictionary Key/Alias of the URL which is to be removed.</param>
        public static void RemoveAlias(params string[] aliases)
        {
            foreach (string alias in aliases)
            {
                AliasedURLs.Remove(alias);
            }
        }
        /// <summary>
        /// Starts the Server if set up.
        /// </summary>
        /// <exception cref="Exception">Is thrown when "SetUp()" wasn't called prior.</exception>
        public static void Start()
        {
            if(!set_up) { throw new Exception("Server isn't set up. Call SetUp(...) first."); }

            cts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                try
                {
                    await ServerMain(cts.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in ServerMain: {ex.Message}");
                }
            });
        }
        /// <summary>
        /// Stops the Server if already running.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void Stop()
        {
            if(!set_up) { throw new Exception("Server isn't running."); }

            cts?.Cancel();
            set_up= false;
        }
    }
}
