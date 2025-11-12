using PadelAgent.Configurations;
using PadelAgent.Engine;
using PadelAgent.Engine.Clients;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
var configuration = builder.Configuration;

builder.Services.AddPlaytomicClient(configuration);
builder.Services.AddServices();
builder.Services.Configure<ClubsSettings>(configuration.GetSection(nameof(ClubsSettings)));
builder.Services.Configure<OpenAISettings>(configuration.GetSection(nameof(OpenAISettings)));
builder.Services.Configure<CalendarSettings>(configuration.GetSection(nameof(CalendarSettings)));

builder.Services.AddRouting(options => options.LowercaseUrls = true)
    .AddControllers(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
