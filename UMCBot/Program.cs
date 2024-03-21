namespace UMCBot;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var bot = new UMCBot();
        await bot.Initialize();
    }
}