using System.Runtime.Serialization;

namespace WebServiciesEnrollment.Models
{
    [DataContract]
    public class EnrollmentRequest
    {
        [DataMember]
        public string NoExpediente { get; set; }
        [DataMember]
        public string Ciclo {get; set;}
        [DataMember]
        public int MesInicioPago {get; set; }
        [DataMember]
        public string CarreraId {get; set; }
/*        [DataMember]
        public string CargoInscripcion {get; set; }
        [DataMember]
        public string CargoCarne {get; set; }
        [DataMember]
        public string CargoMensualidad {get; set; }*/

    }
}