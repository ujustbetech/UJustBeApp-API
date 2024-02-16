using Partner.Service.Repositories.PartnerDetails;
using Partner.Service.Repositories.GetPartnerService;
using Partner.Service.Repositories.ApproveDisapproveBPCP;

using Partner.Service.Services.GetPartnerService;
using Partner.Service.Services.GetPartnerDetailsService;
using Partner.Service.Services.ApproveDisapproveBPCP;
using Partner.Service.Services.GetPartnerProfileService;
using Partner.Service.Repositories.Partner;
using Partner.Service.Services.Partner;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wkhtmltopdf.NetCore;

namespace Partner.Service
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


            services.AddScoped<IGetPartnerService, GetPartnerService>();
            services.AddScoped<IGetPartnerDetails, GetPartnerDetailsService>();
            services.AddScoped<IApproveDisapproveBPCPService, ApproveDisapproveBPCPService>();
            services.AddScoped<IUpdatePartnerProfile, UpdatePartnerService>();
            services.AddScoped<IGetPartnerProfile, GetPartnerProfileService>();
            services.AddScoped<IGetPartnerKYC, GetPartneKYCService>();
            //services.AddScoped<IUpdatePartnerProfile, UpdatePartnerService>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddWkhtmltopdf();
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
