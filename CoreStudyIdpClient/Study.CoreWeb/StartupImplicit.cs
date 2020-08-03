using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Study.CoreWeb
{
    public class StartupImplicit
    {
        public StartupImplicit(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //�ر��� JWT �����Ϣ����ӳ��
            //���������� well-known �����Ϣ�����磬��sub�� �� ��idp�����޸��ŵ�������
            //��������Ϣ����ӳ��ġ����������ڵ��� AddAuthentication()֮ǰ���
            //����ɲο������ͼ
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //�����֤��Ϣ
            services.AddAuthentication()
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            //д�� UseMvc() ǰ��
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
