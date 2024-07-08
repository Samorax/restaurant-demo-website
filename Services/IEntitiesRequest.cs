using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Identity;
using restaurant_demo_website.Models;

namespace restaurant_demo_website.Services
{
    public interface IEntitiesRequest
    {
        public Task<IEnumerable<Product>> GetProductsAsync();
        public Task<IEnumerable<CartOrder>> GetCartOrdersAsync();
        public Task AddCartOrderAsync(CartOrder cartOrder);

        public Task UpdateCartOrderAsync(CartOrder cartOrder);
        public Task DeleteCartOrderAsync(CartOrder cartOrder);

        public Task<IEnumerable<Order>> GetOrdersAsync();

        public Task<IEnumerable<Rewards>> GetRewardsAsync();
        public Task<RewardClaimResponse> ClaimRewardAsync(ClaimRewardModel claimRewardModel);

        public Task<Order> AddOrderAsync(Order order);
        public Task UpdateOrderAsync(Order order);

        public Task PostOrderToQueue(Order order);
        public Task<OrderDetail> AddOrderDetailsAsync(OrderDetail orderDetail);

        public Task<Customer> CreateCustomerAsync(CustomerTDO customer);

        public Task<Customer> CustomerSignInAsync(CustomerSignIn customer);

        public Task<string> CreateSetupIntent(PaymentObject paymentObject);
        public Task<Customer> GetCustomerAsync(string customerId);
        Task UpdateCustomerAsync(Customer customer);
        Task RemoveCustomerAsync(Customer? customer);
        Task<VoucherChargeResponse> ChargeVoucher(ChargeVoucherModel c);

        Task<ApplicationUser> GetRestaurantInfo();
    }
}
