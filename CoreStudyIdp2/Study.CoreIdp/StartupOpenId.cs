using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace Study.CoreIdp
{
    public class StartupOpenId
    {
        public IConfiguration Configuration { get; }

        public StartupOpenId(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer(options =>
            {
                //Ĭ�ϵĵ�½ҳ����/account/login
                options.UserInteraction.LoginUrl = "/login";
                options.UserInteraction.LogoutUrl = "/login/logout";
                //��Ȩȷ��ҳ�� Ĭ��/consent
                //options.UserInteraction.ConsentUrl = "";
            })
            .AddDeveloperSigningCredential()
            .AddInMemoryApiResources(IdpConfig.GetApis())
            .AddInMemoryClients(IdpConfig.GetClients())
            .AddTestUsers(TestUsers.Users)
            .AddInMemoryIdentityResources(IdpConfig.GetApiResources());

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseCookiePolicy();
            // Ҫд�� UseMvc()ǰ��
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }
    }
}
