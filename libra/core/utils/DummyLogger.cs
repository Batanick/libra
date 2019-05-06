namespace libra.core.utils
{
    public class DummyLogger : IResourceLogger
    {
        public static readonly IResourceLogger Instance = new DummyLogger();

        private DummyLogger()
        {
            // nope
        }

        public void LogInfo(string msg)
        {
        }

        public void LogWarn(string msg)
        {
        }

        public void LogError(string msg)
        {
        }
    }
}