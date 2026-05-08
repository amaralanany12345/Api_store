namespace StoreWebApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? Balance { get; set; }
        public List<Order>? orders { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
