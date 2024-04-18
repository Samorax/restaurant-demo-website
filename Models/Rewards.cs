namespace restaurant_demo_website.Models
{
    public class Rewards
    {
        public int RewardsId { get; set; }
        public int RedeemPoint { get; set; }
        public string Title {get;set;}
        public string Description { get; set; }
        public byte[] RewardImage {get;set;}
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate {get;set;}
        public int Units {get;set;}
        public Guid CustomerId {get;set;}
    }

     public class RewardDto
    {
        public int RewardsId { get; set; }
        public int RedeemPoint { get; set; }
        public string Title {get;set;}
        public string Description { get; set; }
        public IFormFile RewardImage {get;set;}
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiryDate {get;set;}
        public int Units {get;set;}
        public Guid CustomerId {get;set;}
    }
}