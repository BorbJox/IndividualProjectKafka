namespace StatisticsAPI.Exceptions
{
    public class CustomException : BaseException
    {
        public override void SetParams()
        {
            this.message = "Hello";
            this.code = 123;
        }
    }
}
