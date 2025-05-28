using Q.FilterBuilder.JsonConverter.Extensions;
using Q.FilterBuilder.JsonConverter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Simple DI registration - just register the converter as a singleton
// You can use default options or customize for your query builder library
builder.Services.AddQueryBuilderJsonConverter(options =>
{
    // Configure for React Query Builder format (optional)
    options.ConditionPropertyName = "combinator";  // Default: "condition"
    options.FieldPropertyName = "field";           // Default: "field"
    options.OperatorPropertyName = "operator";     // Default: "operator"
    options.ValuePropertyName = "value";           // Default: "value"
    options.TypePropertyName = "type";             // Default: "type"
});

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
