namespace StoreWebApi.Models
{
    public class ExternalLog
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public string  Provider {get;set;}
        public string  Operation {get;set;}
        public string  RequestPayload {get;set;}
        public string  ResponsePayload {get;set;}
        public string  Status {get;set;}
        public DateTime CreatedAt { get; set; }
    }
}
