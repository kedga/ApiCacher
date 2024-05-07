using ApiCacher.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
	options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApiCacheDbContext>(options =>
	options.UseSqlServer(connectionString));

var app = builder.Build();

app.UseCors();

{
	using var scope = app.Services.CreateScope();

	var context = scope.ServiceProvider.GetRequiredService<ApiCacheDbContext>();

	var fullReset = true;

	if (fullReset)
	{
		context.Database.CloseConnection();
		context.Database.EnsureDeleted();
		context.Database.EnsureCreated();
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.ConfigureApi();

app.Run();