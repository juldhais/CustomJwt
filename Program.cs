using System.Net;
using System.Text.Json;
using CustomJwt.Exceptions;
using CustomJwt.Middlewares;
using CustomJwt.Repositories;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// extensions method untuk me-register sql server data context ke dalam IoC container
builder.Services.AddSqlServer<DataContext>(builder.Configuration.GetConnectionString("DataContext"));

// add default cors policy
builder.Services.AddCors(options => options
    .AddDefaultPolicy(cors =>
        cors.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // ensure database is created
    var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope();
    var db = serviceScope?.ServiceProvider.GetRequiredService<DataContext>();
    db?.Database.EnsureCreated();
    DataInitializer.Run(db);
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

// global error handler
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature == null) return;

        context.Response.StatusCode = contextFeature.Error switch
        {
            OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
            BadRequestException => (int)HttpStatusCode.BadRequest,
            NotFoundException => (int)HttpStatusCode.NotFound,
            ForbiddenException => (int)HttpStatusCode.Forbidden,
            UnauthorizedException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var errorResponse = new
        {
            statusCode = context.Response.StatusCode,
            message = contextFeature.Error.GetBaseException().Message
        };
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    });
});

app.UseMiddleware<JwtMiddleware>();
app.MapControllers();
app.Run();