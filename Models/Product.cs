namespace restaurant_demo_website.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public Guid ApplicationUserID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public required string Category { get; set; }
        public string Code { get; set; }
        public byte[]? photosUrl {get;set;}

        public int LoyaltyPoints {get;set;}

        public string Allergens { get; set; }
        public string[] DiffAllergen { get; set; }


        public string imgUrl { get; set; }

    }
}
