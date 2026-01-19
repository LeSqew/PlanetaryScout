public static class MissionStatus
{
    // Была ли хоть одна ошибка в мини-играх за всю высадку?
    public static bool HadMinigameErrors { get; private set; }

    // Сброс при начале новой высадки
    public static void Reset()
    {
        HadMinigameErrors = false;
    }

    // Вызывается из InteractionHandler, если мини-игра провалена
    public static void RegisterError()
    {
        HadMinigameErrors = true;
    }
}