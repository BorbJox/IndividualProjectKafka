namespace StatisticsAPI.Exceptions
{
    public abstract class BaseException : Exception
    {
        public int code;
        public string? message;

        public BaseException()
        {
            SetParams();
        }

        public abstract void SetParams();

        public string GetMessage() { return code + message; }
    }
}
