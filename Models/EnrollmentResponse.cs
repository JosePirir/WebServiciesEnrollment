using System.Runtime.Serialization;

namespace WebServiciesEnrollment.Models
{
    [DataContract]
    public class EnrollmentResponse
    {
        [DataMember]
        public int Codigo { get; set; }
        [DataMember]
        public string Respuesta {get;set;}
        [DataMember]/*anotacion datamember*/
        public string Carne {get;set;}
    }
}