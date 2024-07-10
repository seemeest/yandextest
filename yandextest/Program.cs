using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
app.UseMiddleware<RequestLoggingMiddleware>(); // Подключение нашего middleware
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Сбор информации о запросе
        var requestInfo = new
        {
            Method = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            Headers = context.Request.Headers,
            Body = await ReadRequestBodyAsync(context.Request)
        };

        // Сериализация информации в JSON без экранирования
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var requestInfoJson = JsonSerializer.Serialize(requestInfo, jsonSerializerOptions);

        // Логирование JSON в консоль
        Console.WriteLine(requestInfoJson);

        // Вызов следующего middleware в конвейере
        await _next(context);
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        using (var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
        {
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0; // Сброс позиции потока для повторного чтения
            return body;
        }
    }
}