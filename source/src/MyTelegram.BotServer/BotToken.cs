namespace MyTelegram.BotServer;

/// <summary>
/// A Telegram Bot API token has the shape <c>&lt;bot_user_id&gt;:&lt;auth_hash&gt;</c>,
/// e.g. <c>123456:AAErs...</c>. The numeric part before the colon is the bot's user id.
/// </summary>
public readonly record struct BotToken(long BotUserId, string AuthHash)
{
    public static bool TryParse(string? token, out BotToken botToken)
    {
        botToken = default;
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var separatorIndex = token.IndexOf(':');
        if (separatorIndex <= 0 || separatorIndex == token.Length - 1)
        {
            return false;
        }

        if (!long.TryParse(token.AsSpan(0, separatorIndex), out var botUserId) || botUserId <= 0)
        {
            return false;
        }

        botToken = new BotToken(botUserId, token[(separatorIndex + 1)..]);
        return true;
    }
}
