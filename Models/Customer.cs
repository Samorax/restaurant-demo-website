using Microsoft.AspNetCore.Identity;

namespace restaurant_demo_website.Models
{
    public class Customer:IdentityUser
    {
        
        public Guid ApplicationUserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Favourite { get; set; }
        public long TotalAmountSpent { get; set; }
        public string HouseAddress { get; set; }
        public string LastOrderDate { get; set; }
        public bool HighSpender { get; set; }

        public string Membership { get; set; }
        public long LoyaltyPoints { get; set; }
    }
}
