using System;

public interface IMinigameController
{
    // Обязательный метод для запуска анализа
    void StartAnalysis(ScannableObject target, Action<bool, ScannableObject> onFinishedCallback);

    // Обязательный метод для очистки (вызывается после завершения)
    void Cleanup();
    bool RequiresInputBlocking { get; }
}