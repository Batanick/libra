namespace libra.core.exceptions
{
    public class NullResourceReferenceException : ResourceSystemException
    {
        public NullResourceReferenceException() : base("Resource reference is null")
        {
        }
    }
}