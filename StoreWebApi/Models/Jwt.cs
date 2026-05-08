namespace StoreWebApi.Models
{
    public class Jwt
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int DateTime { get; set; }
        public string Signingkey { get; set; }
    }
}
