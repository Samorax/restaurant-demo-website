using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using restaurant_demo_website.Services;

namespace restaurant_demo_website.Controllers
{
    [Authorize]
    public class RewardsController : Controller
    {
        private IEntitiesRequest _entitiesRequest;
        
        public RewardsController(IEntitiesRequest entitiesRequest)
        {
            _entitiesRequest = entitiesRequest;
        }
        public async Task<IActionResult> Index()
        {
            var rewards = await _entitiesRequest.GetRewardsAsync();
            
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
    }
}
