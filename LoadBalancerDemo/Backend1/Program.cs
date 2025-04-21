var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello from Backend 1!");
app.MapGet("/health", () => "Healthy");

app.Run("http://0.0.0.0:5001");
