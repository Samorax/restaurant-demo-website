namespace restaurant_demo_website.Models
{
    public class CustomerSignIn
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public bool lockoutOnFailure { get; set; }
    }
}
