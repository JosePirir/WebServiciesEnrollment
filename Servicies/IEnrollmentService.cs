using System.ServiceModel;
using WebServiciesEnrollment.Models;

namespace WebServiciesEnrollment.Servicies
{
    [ServiceContract]
    public interface IEnrollmentService
    {
        [OperationContract]
        string Test(string s);
        [OperationContract]
        EnrollmentResponse EnrollmentProcess(EnrollmentRequest request);
    }
}
/**/