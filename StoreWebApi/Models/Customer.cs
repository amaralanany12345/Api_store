namespace StoreWebApi.Models
{
    public class Customer:User
    {
        public int Balance { get; set; }
        public List<Order> Orders { get; set; }
    }
}
