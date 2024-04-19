using FoodloyaleApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using restaurant_demo_website.Controllers;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace restaurant_demo_website.Tests
{
    public class HomeControllerTest
    {
        [Fact]
        public async Task Return_Restaurant_DetailAsync()
        {
            var mock = new Mock<IEntitiesRequest>();
            mock.Setup(i=>i.GetRestaurantInfo()).ReturnsAsync(GetRestaurantinfo());

            var mock1 = new Mock<IMemoryCache>();
            var controller = new HomeController(mock.Object, mock1.Object);

            var result = await controller.IndexAsync();

            var viewresult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ApplicationUser>(viewresult.ViewData.Model);
            Assert.Equal("Muffins", model.BusinessName);


        }

        private ApplicationUser GetRestaurantinfo()
        {
            return new ApplicationUser
            {
                BusinessName = "chicken republic"
            };
        }
    }
}
