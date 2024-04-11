using CmdaFunding.Data;
using CmdaFunding.Models;
using CmdaFunding.Repositories.Implementation;
using CmdaFunding.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
//builder.Services.AddDbContext<FundingContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddDbContext<FundingContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddTransient<IUserRepository,UserRepository>();                                //Ram
builder.Services.AddTransient<IGatewayReportRepository, GatewayReportRepository>();
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


// Add CORS middleware here                                                                 //Ram
app.UseCors(builder =>
    builder.WithOrigins("http://localhost:3000") // Replace with your frontend URL
           .AllowAnyHeader()
           .AllowAnyMethod()
);

app.UseAuthorization();

app.MapControllers();

app.Run();
