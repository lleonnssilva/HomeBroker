using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços MVC
builder.Services.AddControllersWithViews();


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = "Cookies"; // Usar cookies para armazenar a sessão do usuário
//    options.DefaultChallengeScheme = "Cognito"; // Usar Cognito como provedor de autenticação
//})
//.AddOpenIdConnect("Cognito", options =>
//{
    var cognitoSettings = builder.Configuration.GetSection("Cognito");

//    options.Authority = $"https://cognito-idp.{cognitoSettings["Region"]}.amazonaws.com/{cognitoSettings["UserPoolId"]}";
//    options.ClientId = cognitoSettings["ClientId"];
//    options.ClientSecret = cognitoSettings["ClientSecret"];
//    options.ResponseType = "code"; // Fluxo de autorização com código

//    options.SaveTokens = true; // Salvar tokens em cookies para futuras requisições

//    // Escopos válidos
//    options.Scope.Add("openid");
//    options.Scope.Add("profile");
//    options.Scope.Add("email");

//    // Define o caminho de retorno (callback) após autenticação
//    options.CallbackPath = "/signin-oidc";  // Esse é o caminho de retorno esperado

//    // Configuração para mapeamento de claims, se necessário
//    options.ClaimActions.MapJsonKey("role", "custom:role");
//})
//.AddCookie("Cookies"); 
//builder.Services.AddAuthorization(options =>
//{
//    // Exemplo de política de autorização para "Admin"
//    options.AddPolicy("Admin", policy =>
//    {
//        policy.RequireClaim("role", "admin");
//    });
//});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    })
    .AddOpenIdConnect(options =>
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