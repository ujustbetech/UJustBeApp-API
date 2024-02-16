using Business.Service.Repositories.AddProductService;
using Business.Service.Repositories.ApproveBusinessKYC;
using Business.Service.Repositories.Company;
using Business.Service.Repositories.ProductService;
using Business.Service.Repositories.ProductServices.AddProductService;
using Business.Service.Services.ApproveBusinessKYC;
using Business.Service.Services.Company;
using Business.Service.Services.ProductServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Business.Service.Repositories.DeleteProductService;
using Wkhtmltopdf.NetCore;

namespace Business.Service
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

            services.AddScoped<IApproveBusinessKYCService, ApproveBusinessKYCService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUpdateBusinessService, UpdateBusinessService>();
            services.AddScoped<IUpdateCompanyService, UpdateCompanyService>();
            services.AddScoped<IAddProductService, AddProductService>();
            services.AddScoped<IClientPartnerService, ClientPartnerService>();
            services.AddScoped<IDeleteProductService, DeleteProductService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IConfiguration>(Configuration);
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
