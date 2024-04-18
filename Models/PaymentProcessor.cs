namespace FoodloyaleApi.Models
{
    public class PaymentProcessor
    {
        public int PaymentProcessorID { get; set; }
        public string AccountId { get; set; }
        public string SoftwareHouseId { get; set; }
        public string Name { get; set; }
        public string ApiKey1 { get; set; }
        public string? ApiKey2 { get; set; }
        public bool ConnectionStatus { get; set; }
        public string ApplicationUserID { get; set; }
    }
}