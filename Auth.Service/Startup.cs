using Auth.Service.Respositories.Login;
using Auth.Service.Respositories.Lookup;
using Auth.Service.Respositories.MobileVersion;
using Auth.Service.Respositories.Registeration;
using Auth.Service.Services.Login;
using Auth.Service.Services.Lookup;
using Auth.Service.Services.MobileVersion;
using Auth.Service.Services.Registeration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wkhtmltopdf.NetCore;

namespace Auth.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
            services.AddScoped<IChangePasswordService, ChangePasswordService>();
            services.AddScoped<IEmailCheckService, EmailCheckService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEnrollPartnerService, EnrollPartnerService>();
            services.AddScoped<IUploadPanService, UploadPanService>();
            services.AddScoped<IMentorLookupService, MentorLookupService>();
            services.AddScoped<IUploadAadharService, UploadAadharService>();
            services.AddScoped<IUploadBankDetailsService, UploadBankDetailsService>();
            services.AddScoped<IUserInfoService, UserInfoService>();
            services.AddScoped<ICountryService, CountryLookupService>();
            services.AddScoped<IGetVersionDetails, VersionDetailsService>();
            services.AddScoped<IStateService, StateLookupService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("MyPolicy");
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
