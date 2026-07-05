using Microsoft.AspNetCore.Http;

namespace MyTelegram.BotServer;

/// <summary>
/// Helpers producing the standard Telegram Bot API response envelope:
/// <c>{ "ok": true, "result": ... }</c> on success and
/// <c>{ "ok": false, "error_code": N, "description": "..." }</c> on failure.
/// </summary>
public static class BotApiResults
{
    public static IResult Ok(object result) => Results.Json(new BotApiResponse
    {
        Ok = true,
        Result = result
    });

    public static IResult Error(int errorCode, string description) => Results.Json(new BotApiResponse
    {
        Ok = false,
        ErrorCode = errorCode,
        Description = description
    }, statusCode: errorCode);
}

public sealed class BotApiResponse
{
    public bool Ok { get; set; }
    public object? Result { get; set; }
    public int? ErrorCode { get; set; }
    public string? Description { get; set; }
}

/// <summary>Subset of the Bot API <c>User</c> object returned by <c>getMe</c>.</summary>
public sealed class BotUser
{
    public long Id { get; set; }
    public bool IsBot { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? Username { get; set; }
    public bool CanJoinGroups { get; set; }
    public bool CanReadAllGroupMessages { get; set; }
    public bool SupportsInlineQueries { get; set; }
}
