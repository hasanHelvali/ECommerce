using ECommerceAPI.Persistance;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistanceServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options=>
    options.AddDefaultPolicy(policy=>
            policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
/*Burada bir cors politikasi belirlemis olduk. 
 Default yani uygulaman�n tamma�nda gecerli olan bir policy belirledik. Bunun icerisinde butun header lara butun metod lara ve 
butun origin lere izin ver demis olduk.
K�sacasi her Sa diyen benim api mi tuketebilir hale gelmis olur.
Bize herhangi �ustehcen bir siteden veya bir teror orgutu sitesinden de bir istek geliyor olabilir. Onun icin bunu bu sekilde 
b�rakm�yoruz.*/

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()));
//Bu sekilde sadece client imizdan gelecek olan isteklere izin vermis olduk.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
app.UseCors();
}
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
