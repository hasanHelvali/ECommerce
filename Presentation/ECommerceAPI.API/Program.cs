using ECommerceAPI.Application;
using ECommerceAPI.Application.Validators.Products;
using ECommerceAPI.Infrastructure;
using ECommerceAPI.Infrastructure.Filters;
using ECommerceAPI.Infrastructure.Services.Storage.Azure;
using ECommerceAPI.Infrastructure.Services.Storage.Local;
using ECommerceAPI.Persistance;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistanceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();//Burada application katman�ndaki konfigurasyonlar� projeme dahil etmis oldum.

builder.Services.AddStorage<LocalStorage>();
//Burada IStorage talep ettigimde bunun gelmesini sagl�yorum. Baska bir storage verirsem o gelir. 
//Tek sat�r kod uzerinden tum bunlar� duzenleyebilirim.
//builder.Services.AddStorage<AzureStorage>();



//builder.Services.AddStorage(ECommerceAPI.Infrastructure.Enums.StorageType.Local);
//builder.Services.AddStorage(ECommerceAPI.Infrastructure.Enums.StorageType.Azure);
//builder.Services.AddStorage(ECommerceAPI.Infrastructure.Enums.StorageType.AWS);
//Bu sekilde bir talep etme islemi de yap�yor olabilirim. Burada da bir enum yap�s� kullanm�s oldum.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers(options=>options.Filters.Add<ValidationFilter>())
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter=true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //Bir authorization islemi yapacaksan admin isminde yap.
    .AddJwtBearer("Admin", options =>
    {
        //Jwt kullan
        options.TokenValidationParameters = new()
        {
            //jwt ile ilgili buradaki konfigurasyonel degerleri kullan.
            ValidateAudience = true,//Olusturulacak token degerini kimlerin/hangi originlerin/sitelerin kullanacag�n� belirledigimiz degerdir.
            ValidateIssuer = true,//Olusturulacak token degerini kimin dag�tt�g�n� ifade ettigimiz aland�r.
            ValidateLifetime = true,//Olusturulan token degerinin suresini kontrol edecek olan dogrulamad�r.
            ValidateIssuerSigningKey = true,//simetrik secrek bir keydir. Bu key bizim urettigimiz key lerden biri mi? sorusunun dogrulanmas�d�r.
            //Gelen key de nelere bak�lacag�n� burada bildirmis oldum.

            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]))
        };
    });
//builder.Services.AddAuthorization();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseCors();
}
app.UseCors();

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
//Bu iki middleware authentication ve authorization islemleri icin onemlidir.


app.MapControllers();

app.Run();
