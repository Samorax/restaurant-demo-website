using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;

namespace restaurant_demo_website.Controllers
{
    [Authorize]
    public class RewardsController : Controller
    {
        private IEntitiesRequest _entitiesRequest;
        private IMemoryCache _memoryCache;

        public RewardsController(IEntitiesRequest entitiesRequest, IMemoryCache memoryCache)
        {
            _entitiesRequest = entitiesRequest;
            _memoryCache = memoryCache;
        }
        public async Task<IActionResult> Index()
        {
            ApplicationUser restaurantinfo = await GetCache();
            
            ViewData["RestaurantName"] = restaurantinfo.BusinessName;

            var rewards = await _entitiesRequest.GetRewardsAsync();
            foreach (var reward in rewards) 
            {
                reward.photoUrl = GetImagesFromByteArray(reward.RewardImage);
            }
            
            
            return View(rewards);
        }

        public async Task<IActionResult> ClaimRewardAsync(int id)
        {
            var customerId = User.Claims.Single(c => c.Type == "UserId").Value;
            ClaimRewardModel claimRewardModel = new ClaimRewardModel { CustomerId = Guid.Parse(customerId) ,RewardId = id };
            RewardClaimResponse respnse = await _entitiesRequest.ClaimRewardAsync(claimRewardModel);
            ViewData["Reason"] = respnse.Reason;
            return new PartialViewResult {ViewName = "_ClaimRewardPartial", ViewData = this.ViewData};
        }

        private string GetImagesFromByteArray(byte[]? photosUrl)
        {
            var dataString = Convert.ToBase64String(photosUrl);
            var imgString = string.Format("data:image/png;base64,{0}", dataString);
            return imgString;
        }

        private async Task<ApplicationUser> GetCache()
        {
            ApplicationUser restaurantinfo = new ApplicationUser();
            if (ShoppingCart.CartSessionKey != null)
            {
                if (!_memoryCache.TryGetValue(ShoppingCart.CartSessionKey, out ApplicationUser u))
                {
                    restaurantinfo = await _entitiesRequest.GetRestaurantInfo();
                    _memoryCache.Set(ShoppingCart.CartSessionKey, restaurantinfo);
                }
                else
                {
                    restaurantinfo = u;
                }

            }

            return restaurantinfo;
        }

    }
}
