using System.Net;

namespace WebServiciesEnrollment.Models
{
    public class AppLog
    {
        public AppLog()
        { /*constructor vacio*/ /*asignar la ip del servidor*/
            IPAddress[] address = Dns.GetHostEntry(HostName).AddressList;
            for(int i = 0; i< address.Length; i++)
            {
                this.Ip = address[i].ToString(); /*ip igual al vector que sería address en la posicion de I*/
            }
        }
        public string Name { get; set; } = "WebServiciesEnrollment"; //valor default
        public string HostName { get; set; } = Dns.GetHostName(); //recibir el nombre del host
        public string ApiKey { get; set; } = "N/A";
        public string Uri { get; set; } = "/EnrollmentService.asmx";
        public int ResponseCode { get; set; }
        public long ResponseTime { get; set; }
        public string Ip { get; set; } /*ip del contenedor que va a ejecutar el sp*/ /**/
        public int Level { get; set; }
        public string Message { get; set; }
        public string DateTime { get; set; }
        public string Version { get; set; } = "v1";
    }
}