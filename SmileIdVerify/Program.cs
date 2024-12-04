using SmileIdVerify.Configurations;
using SmileIdVerify.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<SmileIdService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["SmileId:BaseUrl"]);
}

);

builder.Services.AddScoped<SmileIdService>();
builder.Services.Configure<SmileIdConfig>(builder.Configuration.GetSection("SmileId"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
