namespace System_Analysis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls(urls: "http://192.168.1.106:5001");
                    //webBuilder.UseUrls(urls: "http://192.168.4.1");
                });
    }
}
