using System;

namespace KBuildSystem.App
{
    public static class MainClass
    {
        public static Server Server = null;
        public static void Main(string[] args)
        {
            Server = new Server();
            Server.InitalizeServer();
            Server.StartThreads();
            Console.ReadKey(true);
        }
    }
}
