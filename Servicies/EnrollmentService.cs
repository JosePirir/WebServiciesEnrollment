using WebServiciesEnrollment.Models;
using WebServiciesEnrollment.Helpers;
using System.Data.SqlClient;
using System.Data;
using System.Text.Json;
using Serilog;
using WebServiciesEnrollment.Models;

namespace WebServiciesEnrollment.Servicies
{
    public class EnrollmentService : IEnrollmentService
    {
        private IConfiguration Configuration;
        private SqlConnection Connection = null;
        //private SqlConnection connection = new SqlConnection("Server=sqlserver;Database=kalum_test;User Id=sa;Password=Inicio.2022;");
        /*Agregar doble \\ para que no de error en el nombre del servidor */

        private AppLog AppLog = new AppLog();/*crear log y almacenarlo*/
        public EnrollmentService(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            this.Connection = new SqlConnection(this.Configuration.GetConnectionString("defaultConnection"));
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
            EnrollmentResponse response = null;
            SqlCommand cmd = new SqlCommand("sp_enrrollmentProcess",this.Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@NoExpediente",request.NoExpediente));
            cmd.Parameters.Add(new SqlParameter("@Ciclo",request.Ciclo));
            cmd.Parameters.Add(new SqlParameter("@MesInicioPago",request.MesInicioPago));
            cmd.Parameters.Add(new SqlParameter("@CarreraId",request.CarreraId));/*request de la peticion de .asmx*/
            SqlDataReader reader = null;
            try
            {
                this.Connection.Open(); /*abro coneccion*/
                reader = cmd.ExecuteReader();/*se lee el sp, y se la alamacena en reader*/
                while(reader.Read())/*while porque puede devolver más de una linea*/
                {
                    response = new EnrollmentResponse()
                        { 
                            Respuesta = reader.GetValue(0).ToString(), 
                            Carne = reader.GetValue(1).ToString()
                        };
                    if(reader.GetValue(0).ToString().Equals("TRANSACTION SUCCESS"))
                    {
                        response.Codigo = 201;
                        Utils.ImprimirLog(201,reader.GetValue(0).ToString(),"Information",this.AppLog);
                    }
                    else if(reader.GetValue(0).ToString().Equals("TRANSACTION ERROR"))
                    {
                        response.Codigo = 503;
                        Utils.ImprimirLog(503,reader.GetValue(0).ToString(),"Error",this.AppLog);
                    }
                    else 
                    {
                        response.Codigo = 503;
                        Utils.ImprimirLog(503,"Error al momento de llamar al procedimiento almacenado","Error",this.AppLog);
                    }
                }
                reader.Close();
                this.Connection.Close();
            }
            catch (Exception e)
            {
                response = new EnrollmentResponse() {Codigo = 503, Respuesta = "Error al momento de llamar al procedimiento almacenado", Carne = "0"};
                Utils.ImprimirLog(503,e.Message, "Error",this.AppLog);
            }
            finally /*Exista o no siempre pasará por finally y cerrará la conexión.*/
            {
                this.Connection.Close();
            }
            return response;
        }

        private Aspirante buscarAspirante(string noExpediente)/*ADO .net*/
        {
                Aspirante resultado = null;
                SqlDataAdapter daAspirante = new SqlDataAdapter($"select * from Aspirante where NoExpediente = '{noExpediente}'",this.Connection);
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

        public CandidateRecordResponse CandidateRegisterProcess(CandidateRecordRequest request)
        {
            AppLog.ResponseTime = Convert.ToInt16(DateTime.Now.ToString("fff"));
            AppLog.DateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            CandidateRecordResponse response = null;
            SqlCommand cmd = new SqlCommand("sp_CandidateRecordCreate", this.Connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@apellidos", request.Apellidos));
            cmd.Parameters.Add(new SqlParameter("@nombres", request.Nombres));
            cmd.Parameters.Add(new SqlParameter("@direccion", request.Direccion));
            cmd.Parameters.Add(new SqlParameter("@telefono", request.Telefono));
            cmd.Parameters.Add(new SqlParameter("@email", request.Email));
            cmd.Parameters.Add(new SqlParameter("@carreraId", request.CarreraId));
            cmd.Parameters.Add(new SqlParameter("@examenId", request.ExamenId));
            cmd.Parameters.Add(new SqlParameter("@jornadaId", request.JornadaId));
            SqlDataReader reader = null;
            try {
                this.Connection.Open();
                reader = cmd.ExecuteReader();
                while(reader.Read()) 
                {
                    response = new CandidateRecordResponse() 
                    { 
                        Respuesta = reader.GetValue(0).ToString(), 
                        NoExpediente = reader.GetValue(1).ToString()
                    };
                    if(reader.GetValue(0).ToString().Equals("TRANSACTION SUCCESS"))
                    {
                        response.Codigo = 201;
                        Utils.ImprimirLog(201,reader.GetValue(0).ToString(),"Information",this.AppLog);
                    }
                    else if(reader.GetValue(0).ToString().Equals("TRANSACTION ERROR"))
                    {
                        response.Codigo = 503;
                        Utils.ImprimirLog(503,reader.GetValue(0).ToString(),"Information",this.AppLog);

                    } else 
                    {
                        response.Codigo = 500;
                        Utils.ImprimirLog(500,"Error en el proceso para generar el expediente","Information",this.AppLog);
                    }
                }
                reader.Close();
                this.Connection.Close();
            }
            catch(Exception e) 
            {
                response = new CandidateRecordResponse(){Codigo = 503, Respuesta = "Error al momento de ejecutar el proceso de registro", NoExpediente = "0"};                
                Utils.ImprimirLog(500,e.Message,"Information",this.AppLog);

            }
            finally
            {
                this.Connection.Close();
            }
            return response;
        }
    }
}