using System.Text.Json;
using System.Text.Json.Serialization;
using MyTelegram.BotServer;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();
var logger = app.Logger;

// Health check.
app.MapGet("/", () => Results.Ok(new { ok = true, service = "MyTelegram Bot Server" }));

// Telegram Bot API entrypoint: https://core.telegram.org/bots/api
// All methods are addressed as /bot<token>/<method> and may be called with GET or POST.
app.MapMethods("/bot{token}/{method}", ["GET", "POST"], async (string token, string method, HttpContext context) =>
{
    if (!BotToken.TryParse(token, out var botToken))
    {
        return BotApiResults.Error(401, "Unauthorized: invalid token");
    }

    logger.LogInformation("Bot API call: botId={BotId} method={Method}", botToken.BotUserId, method);

    switch (method.ToLowerInvariant())
    {
        case "getme":
            return BotApiResults.Ok(new BotUser
            {
                Id = botToken.BotUserId,
                IsBot = true,
                FirstName = context.RequestServices.GetRequiredService<IConfiguration>()
                    .GetValue<string>("Bot:FirstName") ?? "Bot",
                Username = context.RequestServices.GetRequiredService<IConfiguration>()
                    .GetValue<string>("Bot:Username"),
                CanJoinGroups = true,
                CanReadAllGroupMessages = false,
                SupportsInlineQueries = false
            });

        // Methods below require translation to the internal MTProto command/query
        // pipeline (command bus + read models). Until that wiring is in place we
        // return the standard Bot API "not implemented" error instead of a fake success.
        default:
            return BotApiResults.Error(501, $"METHOD_NOT_IMPLEMENTED: {method}");
    }
});

app.Run();
