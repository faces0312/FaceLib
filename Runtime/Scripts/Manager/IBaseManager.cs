namespace FaceLib
{
    public interface IBaseManager
    {
        bool IsInitialized { get; }

        void Initialize();

        void Shutdown();
    }
}


