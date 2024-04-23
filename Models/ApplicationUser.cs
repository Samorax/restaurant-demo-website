using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using restaurant_demo_website.Models;

namespace FoodloyaleApi.Models
{
    
    public class ApplicationUser : IdentityUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public string Id { get; set; }
        public string BusinessName { get; set; }

      
        public string BusinessAddress1 { get; set; }
        public string BusinessAddress2 { get; set; }

      
        public string PostalCode { get; set; }
        
        public string State { get; set; }
       
        public string Country { get; set; }
        public List<OpenTimes> OpeningTimes { get; set; }
        public decimal DeliveryFee { get; set; }

        public string FirstName {get;set;}
        public string LastName {get;set;}
        
        public string Password { get; set; }
      
        public string AccountType { get; set; }

        public PaymentProcessor PaymentProcessor { get; set; }

        public string ApiKey { get; set; }
        public string Currency { get; set; }
        public string MessagingServiceID { get; set; }
        public decimal VATCharge { get; internal set; }
    }
}
