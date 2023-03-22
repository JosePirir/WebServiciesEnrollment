using System.Runtime.Serialization;

namespace WebServiciesEnrollment.Models
{
    [DataContract]
    public class CandidateRecordResponse
    {       
        [DataMember]
        public int Codigo {get; set; }
        [DataMember]
        public string Respuesta {get; set; }
        [DataMember]
        public string NoExpediente {get;set;}    
    }
}