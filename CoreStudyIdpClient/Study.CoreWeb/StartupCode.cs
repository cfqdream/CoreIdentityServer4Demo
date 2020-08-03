using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Study.CoreWeb
{
    public class StartupCode
    {
        public StartupCode(IConfiguration configuration)
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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //�����֤��Ϣ
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    //��֤������
                    options.Authority = "http://localhost:5002";
                    //ȥ��  https
                    options.RequireHttpsMetadata = false;
                    options.ClientId = "mvc client";
                    options.ClientSecret = "mvc secret";
                    //�� token �洢�� cookie
                    options.SaveTokens = true;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    //������� Ȩ�� ������� scope ��������Ȩ����˶���Ŀͻ��� AllowedScopes ��
                    options.Scope.Clear();
                    options.Scope.Add("api1");
                    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                    options.Scope.Add(OidcConstants.StandardScopes.Email);
                    options.Scope.Add(OidcConstants.StandardScopes.Address);
                    options.Scope.Add(OidcConstants.StandardScopes.Phone);
                    options.Scope.Add(OidcConstants.StandardScopes.Profile);
                    options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);

                    options.Events = new OpenIdConnectEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            context.HandleResponse();
                            context.Response.WriteAsync("��֤ʧ��");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            //д�� UseMvc() ǰ��
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
        }
    }
}
