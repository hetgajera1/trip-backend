var builder = WebApplication.CreateBuilder(args);

// Add CORS policy to allow frontend requests
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register MongoDB and custom services
builder.Services.AddSingleton<Backend.Services.MongoDbService>();
builder.Services.AddSingleton<Backend.Services.ContactMessageService>();
builder.Services.AddSingleton<Backend.Services.CabinService>();
builder.Services.AddSingleton<Backend.Services.UserService>();
builder.Services.AddSingleton<Backend.Services.BookingService>();
builder.Services.AddSingleton<Backend.Services.ReviewService>();
builder.Services.AddSingleton<Backend.Services.PasswordResetService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
