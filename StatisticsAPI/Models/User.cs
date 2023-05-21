
namespace StatisticsAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string APIKey { get; set; }
        public bool IsAdmin { get; set; }
    }
}
