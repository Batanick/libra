namespace libra.core.utils
{
    public interface IResourceLogger
    {
        void LogInfo(string msg);
        void LogWarn(string msg);
        void LogError(string msg);
    }
}