using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//配置身份驗證中間件,告訴應用程序如何進行身份驗證
var secretKey = builder.Configuration.GetValue<string>("SecretKey");
builder.Services.AddAuthentication(options =>
{
    //用來確定用戶是否已經認證的方案
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    //當用戶嘗試訪問需要認證但他們未認證的資源時，這個方案會被觸發。挑戰方案通常會將用戶重定向到登入頁面或發送特定的HTTP響應（例如，401 Unauthorized）。
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
    //將某個用戶標記為已登入時，將使用此方案
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // 使用 AddJwtBearer 方法，您需要安裝 Microsoft.AspNetCore.Authentication.JwtBearer NuGet套件
    // 設定如何處理和驗證JWT
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //ValidateIssuerSigningKey: 設為true意味著將驗證簽名密鑰。
        ValidateIssuerSigningKey = true,
        // IssuerSigningKey: 設定用於驗證JWT簽名的密鑰

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey ?? string.Empty)),
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        //不允許時鐘偏差。在分佈式系統中，不同的伺服器可能時鐘不同步，這可以允許一些小的時鐘偏差。
        ClockSkew = TimeSpan.Zero,
    };
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//使用認證中間件
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();