namespace MyHTTPServer.Sessions;

public static class SessionStorage
{
    private static readonly Dictionary<string, string> Sessions = new Dictionary<string, string>();
 
    // Сохранение токена и его соответствующего ID пользователя
    public static void SaveSession(string token, string userId)
    {
        Sessions[token] = userId;
    }
 
    // Проверка токена
    public static bool ValidateToken(string token)
    {
        return Sessions.ContainsKey(token);
    }
 
    // Получение ID пользователя по токену
    public static string GetUserId(string token)
    {
        return Sessions.TryGetValue(token, out var userId) ? userId : null;
    }
}