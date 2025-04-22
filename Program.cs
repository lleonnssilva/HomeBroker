using Microsoft.AspNetCore.Authentication.OpenIdConnect;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var cognitoSettings = builder.Configuration.GetSection("Cognito");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "cookie";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("cookie",options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.ClientId = cognitoSettings["ClientId"];
        options.ClientSecret = cognitoSettings["ClientSecret"];
        options.Authority = $"https://cognito-idp.{cognitoSettings["Region"]}.amazonaws.com/{cognitoSettings["UserPoolId"]}";
        options.ResponseType = "code";

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;


        options.CallbackPath = "/signin-oidc";
        options.SignedOutCallbackPath = "/signout-oidc";

        options.Events = new OpenIdConnectEvents
        {
            //// Evento que ocorre quando a autenticação falha
            //OnAuthenticationFailed = context =>
            //{
            //    // Log de erro ou redirecionamento em caso de falha
            //    context.Response.Redirect("/Home/Error");
            //    context.HandleResponse(); // Impede o processamento adicional
            //    return Task.CompletedTask;
            //},

            //// Evento que ocorre quando o token é validado com sucesso
            //OnTokenValidated = context =>
            //{
            //    var claimsIdentity = (ClaimsIdentity)context.Principal.Identity;
            //    claimsIdentity.AddClaim(new Claim("customClaim", "someValue"));
            //    return Task.CompletedTask;
            //},

            //// Evento que ocorre antes de redirecionar para o Cognito
            //OnRedirectToIdentityProvider = context =>
            //{
            //    // Adicionar parâmetros personalizados ao redirecionamento
            //    context.ProtocolMessage.SetParameter("custom_param", "value");
            //    return Task.CompletedTask;
            //},

            //// Evento que ocorre após o logout
            //OnRedirectToIdentityProviderForSignOut = context =>
            //{
            //    context.Response.Redirect("/Home/Goodbye");
            //    return Task.CompletedTask;
            //}

            OnTokenValidated =  context =>
            {
                var tk = context.SecurityToken.RawData.ToString();
                //var _ResultTokenToken = await GetPerfilUserAsync(tk.ToString());
                //if (_ResultTokenToken.StausCode == System.Net.HttpStatusCode.OK)
                //{
                //    if (_ResultTokenToken.Claims.Count > 0)
                //    {
                //        var appIdentity = new ClaimsIdentity(_ResultTokenToken.Claims.ToList());
                //        context.Principal.AddIdentity(appIdentity);
                //        var perfil = _ResultTokenToken.Claims.FirstOrDefault(z => z.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
                //        if (perfil == "Candidato") context.Properties.RedirectUri = "CursosProfissionalizantes/Inscricoes";
                //        if (perfil == "Admin") context.Properties.RedirectUri = "Relatorio";
                //        if (perfil == "Executora") context.Properties.RedirectUri = "Solicitacoes";
                //        if (string.IsNullOrEmpty(perfil)) context.Properties.RedirectUri = "CursosProfissionalizantes/Error?message=Acesso restrito:403.";
                //    }
                //}
                //else if (_ResultTokenToken.StausCode == System.Net.HttpStatusCode.NotFound) { context.Properties.RedirectUri = "CursosProfissionalizantes/Confirmacao"; }
                //else if (_ResultTokenToken.StausCode == System.Net.HttpStatusCode.Forbidden) { context.Properties.RedirectUri = "CursosProfissionalizantes/Error?message=Acesso restrito:403."; }
                //else if (_ResultTokenToken.StausCode == System.Net.HttpStatusCode.Unauthorized) { context.Properties.RedirectUri = "CursosProfissionalizantes/Error?message=Acesso não autorizado:401."; }
                //else { context.Properties.RedirectUri = "CursosProfissionalizantes/Error?message=Erro não esperado!"; }
                //context.Properties.RedirectUri = "Home/Error?message=Erro não esperado!";
                return Task.CompletedTask;
            },
            OnRedirectToIdentityProviderForSignOut = (context) =>
            {
                var id_token_hint = context.ProtocolMessage.IdTokenHint;
                var logoutUri = $"{cognitoSettings["UrlAuth"]}/logout?client_id={cognitoSettings["ClientId"]}&logout_uri={cognitoSettings["RedirectUri"].ToString().Replace("signin-oidc", "")}";
                context.Response.Redirect(logoutUri);
                context.HandleResponse(); 
                return Task.CompletedTask;
            },
            OnRedirectToIdentityProvider = async n =>
            {
                n.ProtocolMessage.RedirectUri = cognitoSettings["RedirectUri"];
                await Task.FromResult(0);
            },
            OnTokenResponseReceived = async g => { await Task.FromResult(0); }
        };


    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Investidor", policy => policy.RequireClaim("cognito:groups", "Investidor"));
});



    var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
