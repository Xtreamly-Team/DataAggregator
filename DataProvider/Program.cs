using DataProvider.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Transient);
builder.Services.AddHttpClient();
builder.Services.AddHostedService<UniSwapAggregator>();
builder.Services.AddHostedService<MempoolAggregator>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();