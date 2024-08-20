using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeyRed.Mime;

namespace WebHttpsServer.server
{
    public static class HttpServer
    {
        private static bool set_up = false;
        private static string? StaticFolder;
        private static Dictionary<string, string> predefinedURLs = new(); ///<visiblepath, actualpath>
        private static HttpListener listener = new();
        private static CancellationTokenSource? cts;

        private static async Task ServerMain(CancellationToken token)
        {
            Console.WriteLine("Server started and awaiting requests...");
            while (!token.IsCancellationRequested)
            {
                try
                {
                    HttpListenerContext context = await listener.GetContextAsync().ConfigureAwait(false);
                    if (context == null) continue;

                    string clientIp = context.Request.RemoteEndPoint?.Address.ToString() ?? "Unknown IP";
                    string requestedPath = context.Request.RawUrl;
                    DateTime requestTime = DateTime.Now;

                    Console.WriteLine($"[{requestTime}] Request from {clientIp} for path {requestedPath}");

                    _ = Task.Run(async () =>
                    {
                        var res = context.Response;
                        var req = context.Request;

                        try
                        {
                            byte[] buffer;
                            string resFilePath = predefinedURLs.TryGetValue(req.RawUrl, out var path)
                                ? StaticFolder + path
                                : req.RawUrl == "/"
                                    ? StaticFolder + "/index.html"
                                    : StaticFolder + req.RawUrl;

                            if (File.Exists(resFilePath))
                            {
                                // Use HeyRed.Mime to determine the MIME type
                                string mimeType = MimeTypesMap.GetMimeType(resFilePath);

                                using (var fileStream = File.OpenRead(resFilePath))
                                {
                                    buffer = new byte[fileStream.Length];
                                    await fileStream.ReadAsync(buffer, 0, buffer.Length);
                                }

                                res.ContentType = mimeType;
                            }
                            else
                            {
                                buffer = Encoding.UTF8.GetBytes("File not found");
                                res.StatusCode = 404;
                                res.ContentType = "text/plain";
                            }

                            res.ContentLength64 = buffer.Length;
                            using (var output = res.OutputStream)
                            {
                                await output.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error processing request: {e.Message}");
                            res.StatusCode = 500; // Internal Server Error
                        }
                        finally
                        {
                            res.OutputStream.Close();
                        }
                    });
                }
                catch (HttpListenerException ex) when (ex.Message.Contains("The operation was canceled"))
                {
                    Console.WriteLine("Server stopped listening for new connections.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unhandled exception in ServerMain: {ex.Message}");
                }
            }
        }

        public static void AddUrl(params (string alias, string actual)[] urls)
        {
            foreach (var url in urls)
            {
                (string alias, string actual) = verifyUrls(url.Item1, url.Item2);
                predefinedURLs[alias] = StaticFolder + actual;
            }
        }

        public static void RemoveUrl(params string[] aliases)
        {
            foreach (var alias in aliases)
            {
                predefinedURLs.Remove(alias);
            }
        }

        private static (string, string) verifyUrls(string alias, string actual)
        {
            if (!alias.StartsWith('/')) { alias = "/" + alias; }
            if (!actual.StartsWith('/')) { actual = "/" + actual; }

            return (alias, actual);
        }

        private static (string, string) verifyUrls((string, string) urls)
        {
            if (!urls.Item1.StartsWith('/')) { urls.Item1 = "/" + urls.Item1; }
            if (!urls.Item2.StartsWith('/')) { urls.Item2 = "/" + urls.Item2; }

            return urls;
        }

        public static void SetUp(string staticFolder, params string[]? prefixes)
        {
            if (!staticFolder.Equals("/"))
            {
                if (staticFolder.EndsWith('/')) { staticFolder = staticFolder.TrimEnd('/'); }
            }

            StaticFolder = staticFolder;

            if (prefixes != null)
            {
                foreach (string prefix in prefixes)
                {
                    listener.Prefixes.Add(prefix);
                }
                set_up = true;
            }
        }

        public static void Start()
        {
            if (!set_up) { throw new Exception("Server isn't set up yet. Call HttpServer.SetUp(...) first."); }

            listener.Start();

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

        public static void Stop()
        {
            if (!set_up) { throw new Exception("Server isn't running."); }

            // Request cancellation and stop the listener
            cts?.Cancel();
            listener.Stop();
            set_up = false;
        }
    }
}
