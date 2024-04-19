using Microsoft.AspNetCore.Mvc;
using Moq;
using restaurant_demo_website.Controllers;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;
using restaurant_demo_website.ViewModels;
using System.Collections.Generic;

namespace restaurant_demo_website.Tests
{
    public class MenuControllerTest
    {
        [Fact]
        public async Task Check_DistinctCategories_Of_Products_Returned()
        {
            var mock = new Mock<IEntitiesRequest>();
            mock.Setup(m => m.GetProductsAsync()).ReturnsAsync(GetProducts());

            var controller = new MenuController(mock.Object);
            var result = await controller.IndexAsync();
            var viewResult =Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ProductsCategoryViewModel>(viewResult.ViewData.Model);
            Assert.Contains("Dessert", model.Categories);
        }

        [InlineData("Dessert")]
        [Theory]
        public async Task Return_Products_of_A_CategoryAsync(string categoryName)
        {
            var mock = new Mock<IEntitiesRequest>();
            mock.Setup(m => m.GetProductsAsync()).ReturnsAsync(GetProducts());

            var controller = new MenuController(mock.Object);
            var result = await controller.Category(categoryName);
            var viewresult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewresult.ViewData.Model);
            Assert.Equal("Muffins",model.First().Name);

        }

        [Theory]
        [InlineData(2)]
        public async Task Return_A_Product_DetailAsync(int id)
        {
            var mock = new Mock<IEntitiesRequest>();
            mock.Setup(m => m.GetProductsAsync()).ReturnsAsync(GetProducts());

            var controller = new MenuController(mock.Object);
            var result = await controller.DetailsAsync(id);
            var viewresult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Product>(viewresult.Model);
            Assert.Equal("Beans", model.Name);

        }

        private IEnumerable<Product> GetProducts()
        {
            return new List<Product>()
            {
                new Product
                {
                    ProductID = 1,
                    Name = "Rice",
                    Category = "Starter",
                    Price = decimal.Parse("5.20")

                },

                new Product
                {
                    ProductID=2,
                    Name = "Beans",
                    Category = "Starter",
                    Price = decimal.Parse("5.90")
                },

                new Product
                {
                    ProductID=3,
                    Name = "Muffins",
                    Category = "Dessert",
                    Price = decimal.Parse("5.20")
                }

            }.AsEnumerable();
        }
    }
}