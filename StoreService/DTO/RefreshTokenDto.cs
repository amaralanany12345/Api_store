namespace StoreService.DTO
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
