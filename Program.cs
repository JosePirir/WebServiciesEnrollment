using Microsoft.AspNetCore;
using WebServiciesEnrollment;
using Serilog;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.File("WebServiciesEnrollment.out", Serilog.Events.LogEventLevel.Debug,"{Message:lj}{NewLine}",encoding:Encoding.UTF8).CreateLogger();/*imprimir el mensaje en una sola line y por cada linea será una nueva linea en formato utf8*/
        CreateWebHostBuilder(args).Build().Run(); /*Construye el aplicativo y lo corre.*/
    }
    public static IWebHostBuilder CreateWebHostBuilder(string[] args)=>
        WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
}