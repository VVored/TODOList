using Microsoft.EntityFrameworkCore;
using TODOList.GrpcService;
using TODOList.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TodolistDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("TodolistDbContext")));
builder.Services.AddCors(setupAction => { setupAction.AddDefaultPolicy(policy => { policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod().WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding"); }); });
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();


var app = builder.Build();

app.UseRouting();
app.UseCors();
app.UseGrpcWeb();
app.MapGrpcService<DoingsApiService>().EnableGrpcWeb();

IWebHostEnvironment env = app.Environment;

if (env.IsDevelopment())
{
    app.MapGrpcReflectionService().AllowAnonymous();
}

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
