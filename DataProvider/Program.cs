using DataProvider.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Transient);
builder.Services.AddHttpClient();
builder.Services.AddHostedService<UniSwapAggregator>();
var app = builder.Build();
System.IO.Directory.CreateDirectory("/uniswap/");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine("/uniswap/")),
    RequestPath = "/uniswap-swaps"
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();