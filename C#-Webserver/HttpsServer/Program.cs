using System;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using WebHttpsServer.server;


namespace WebHttpsServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            HttpServer.SetUp("public", "http://localhost:80/");
            HttpServer.AddUrl(("/Lukas/stinkt", "/html/chat.html"));
            HttpServer.Start();
            Console.ReadKey();
            HttpServer.Stop();
        }
    }
}