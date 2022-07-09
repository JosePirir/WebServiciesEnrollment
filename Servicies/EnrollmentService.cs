using WebServiciesEnrollment.Models;
using System.Data.SqlClient;
using System.Data;
using System.Text.Json;
using Serilog;

namespace WebServiciesEnrollment.Servicies
{
    public class EnrollmentService : IEnrollmentService
    {   
        private SqlConnection connection = new SqlConnection("Server=LAPTOP-46B42692\\SQLEXPRESS;database=kalum_test;User Id=sa;Password=12345;");
        /*Agregar doble \\ para que no de error en el nombre del servidor */

        private AppLog AppLog = new AppLog();/*crear log y almacenarlo*/
        public EnrollmentService()
        {
        }

        public EnrollmentResponse EnrollmentProcess(EnrollmentRequest request)
        {
            AppLog.ResponseTime = Convert.ToInt16(DateTime.Now.ToString("fff"));/*obtiene milisegundos*/
            AppLog.DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");/*se ejecuta cuando se llama a la base de datos*/
            EnrollmentResponse respuesta = null;
            Aspirante aspirante = buscarAspirante(request.NoExpediente);
            Console.WriteLine(aspirante.Email, aspirante.Estatus);
            if(aspirante == null)
            {
                respuesta = new EnrollmentResponse() {Codigo = 204, Respuesta= "No existen registros"};
                ImprimirLog(204, $"No existe el aspirante con el numero de expediente {request.NoExpediente}", "Information");
            }
            else
            {
                respuesta = EjecutarProcedimiento(request);
            }
            return respuesta;
        }

        private EnrollmentResponse EjecutarProcedimiento(EnrollmentRequest request)
        {
            EnrollmentResponse respuesta = null;
            SqlCommand cmd = new SqlCommand("sp_enrrollmentProcess",connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@NoExpediente",request.NoExpediente));
            cmd.Parameters.Add(new SqlParameter("@Ciclo",request.Ciclo));
            cmd.Parameters.Add(new SqlParameter("@MesInicioPago",request.MesInicioPago));
            cmd.Parameters.Add(new SqlParameter("@CarreraId",request.CarreraId));/*request de la peticion de .asmx*/
            SqlDataReader reader = null;
            try
            {
                connection.Open(); /*abro coneccion*/
                reader = cmd.ExecuteReader();/*se lee el sp, y se la alamacena en reader*/
                while(reader.Read())/*while porque puede devolver más de una linea*/
                {
                    respuesta = new EnrollmentResponse(){Respuesta = reader.GetValue(0).ToString(), Carne = reader.GetValue(1).ToString()}; /*recibe el valor de el sp_enrollmentProcess,*/
                    if(reader.GetValue(0).ToString().Equals("TRANSACTION SUCCESS")) /*columna de status*/
                    {
                        respuesta.Codigo = 201;
                        ImprimirLog(201,reader.GetValue(0).ToString(),"Information");
                    }
                    else if(reader.GetValue(0).ToString().Equals("TRANSACTION ERROR"))
                    {
                        respuesta.Codigo = 503;
                        ImprimirLog(503, reader.GetValue(0).ToString(),"Error");
                    }
                    else
                    {
                        respuesta.Codigo = 503;
                        ImprimirLog(503, "Error al momento de llavar al procedimiento almacenado", "Error");
                    }
                }
                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                respuesta = new EnrollmentResponse() {Codigo = 503, Respuesta = "Error al momento de llamar al procedimiento almacenado", Carne = "0"};
                ImprimirLog(503,"Error al momento de llamar al procedimiento almacenado","Error");
            }
            finally /*Exista o no siempre pasará por finally y cerrará la conexión.*/
            {
                connection.Close();                
            }
            return respuesta;    
        }


        private void ImprimirLog(int responseCode, string message, string typeLog)
        {
            AppLog.ResponseCode = responseCode;
            AppLog.Message = message;
            AppLog.ResponseTime = Convert.ToInt16(DateTime.Now.ToString("fff")) - AppLog.ResponseTime;
            if(typeLog.Equals("Information"))
            {
                    AppLog.Level = 20;
                    Log.Information(JsonSerializer.Serialize(AppLog));/*objeto serializado en formato json*/
            }
            else if(typeLog.Equals("Error"))
            {
                AppLog.Level = 40;
                Log.Error(JsonSerializer.Serialize(AppLog));
            }
            else if(typeLog.Equals("Debug"))
            {
                AppLog.Level = 10;
                Log.Debug(JsonSerializer.Serialize(AppLog));
            }
        }

        private Aspirante buscarAspirante(string noExpediente)/*ADO .net*/
        {
                Aspirante resultado = null;
                SqlDataAdapter daAspirante = new SqlDataAdapter($"select * from Aspirante where NoExpediente = '{noExpediente}'",connection);
                DataSet dsAspirante = new DataSet();
                daAspirante.Fill(dsAspirante,"Aspirante");
                if(dsAspirante.Tables["Aspirante"].Rows.Count > 0)
                {
                    resultado = new Aspirante() 
                    {
                        NoExpediente= dsAspirante.Tables["Aspirante"].Rows[0][0].ToString(),
                        Apellidos= dsAspirante.Tables["Aspirante"].Rows[0][1].ToString(),
                        Nombres= dsAspirante.Tables["Aspirante"].Rows[0][2].ToString(),
                        Direccion= dsAspirante.Tables["Aspirante"].Rows[0][3].ToString(),
                        Telefono= dsAspirante.Tables["Aspirante"].Rows[0][4].ToString(),
                        Email= dsAspirante.Tables["Aspirante"].Rows[0][5].ToString(),
                        Estatus= dsAspirante.Tables["Aspirante"].Rows[0][6].ToString(),
                        CarreraId= dsAspirante.Tables["Aspirante"].Rows[0][7].ToString(),
                        JornadaId = dsAspirante.Tables["Aspirante"].Rows[0][8].ToString()
                    };
                }
                return resultado;
        }

        public string Test(string s)
        {
            Console.WriteLine("Test method executed.");
            return s;
        }
    }
}