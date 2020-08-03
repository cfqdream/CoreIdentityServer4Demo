using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Study.CoreWeb
{
    public class StartupHybrid
    {
        public StartupHybrid(IConfiguration configuration)
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
            //����ɲο������ͼ��
            //����� 
            //jwt �� key ӳ������� http://xxxxxxxxxxxxxxx
            //well-known ӳ������� sub idp ���������ַ�
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //�����֤��Ϣ
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            options =>
            {
                options.AccessDeniedPath = "/Authorization/NoPermission";
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
             {
                 //IdentityServer4 ��������ַ
                 options.Authority = "http://localhost:5002";
                 options.ClientId = "mvc client Hybrid";
                 options.ClientSecret = "mvc secret Hybrid";
                 options.RequireHttpsMetadata = false;
                 options.SaveTokens = true;
                 options.ResponseType = OidcConstants.ResponseTypes.CodeIdToken;

                 //�������token �ͱ����ٶ���ͻ��˵�ʱ����������ͨ�������������AccessToken
                 //options.ResponseType = OidcConstants.ResponseTypes.CodeToken;
                 //options.ResponseType = OidcConstants.ResponseTypes.CodeIdTokenToken;

                 options.Scope.Clear();
                 options.Scope.Add("api1");
                 options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                 options.Scope.Add(OidcConstants.StandardScopes.Email);
                 options.Scope.Add(OidcConstants.StandardScopes.Phone);
                 options.Scope.Add(OidcConstants.StandardScopes.Address);
                 options.Scope.Add(OidcConstants.StandardScopes.Profile);
                 options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
                 options.Scope.Add("roles"); 

                 //ȥ��Ĭ�Ϲ��˵� claim������ User.Claims ��ͻ������� claim
                 options.ClaimActions.Remove("nbf");

                 //���ӹ��˵� claim������ User.Claims ��ͻ�ɾ����� claim
                 options.ClaimActions.DeleteClaim("sid");

                 options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                 {
                     //ӳ�� User.Name
                     NameClaimType = JwtClaimTypes.Name,
                     RoleClaimType = JwtClaimTypes.Role
                 };
             });

            #region �Զ���Ȩ��
            /*
             * �������Կ�ȡ��ע�� TestController.Bob() 
             * [Authorize(Policy = "bob2")][Authorize(Policy = "bob")]  
             */
            services.AddSingleton<IAuthorizationHandler, BobAuthorizationHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("bob", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireClaim(JwtClaimTypes.FamilyName, "Smith", "Smith1", "Smith2", "Smith3");
                });
                options.AddPolicy("bob2", builder =>
                {
                    builder.Requirements.Add(new BobRequirement());
                });
            }); 
            #endregion

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
