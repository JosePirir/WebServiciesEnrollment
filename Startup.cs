using Microsoft.Extensions.DependencyInjection.Extensions;
using SoapCore;
using WebServiciesEnrollment.Servicies;

namespace WebServiciesEnrollment
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IEnrollmentService, EnrollmentService>();
            services.AddMvc();
            services.AddSoapCore();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSoapEndpoint<IEnrollmentService>("/EnrollmentService.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer); /*.asmx, es una buena practica*/
        }
    }
}