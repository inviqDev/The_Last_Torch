namespace Runtime
{
    // Получатель данных в новой сцене
    public interface ISceneLoadHook
    {
        void OnSceneLoaded(TransitionPayload payload);
    }
}