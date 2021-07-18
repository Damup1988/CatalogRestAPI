using System;
using System.Threading.Tasks;
using Catalog.Controllers;
using Catalog.Domain;
using Catalog.Dto;
using Catalog.Repository;
using DnsClient.Internal;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ILogger = DnsClient.Internal.ILogger;

namespace Catalog.UnitTests
{
    public class ItemControllerTests
    {
        private readonly Mock<IItemRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemController>> _loggerStub = new();
        private readonly Random _random = new();
        
        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => 
                    repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item) null);
            var controller = new ItemController(repositoryStub.Object, _loggerStub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            result.Result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            // Arrange
            var expectedItem = CreateRandomItem();
            repositoryStub.Setup(repo =>
                    repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);
            var controller = new ItemController(repositoryStub.Object, _loggerStub.Object);

            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());
            
            // Assert
            result.Value.Should().BeEquivalentTo(
                expectedItem,
                options => options.ComparingByMembers<Item>()
                );
            // Also the same but not a good way as we can have many properties :
            //Assert.IsType<ReadItemDto>(result.Value);
            //var dto = (result as ActionResult<ReadItemDto>).Value;
            //Assert.Equal(expectedItem.Id, dto.Id);
            //Assert.Equal(expectedItem.Name, dto.Name);
        }

        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
        {
            // Arrange
            var expectedItems = new[] {CreateRandomItem(), CreateRandomItem(), CreateRandomItem()};
            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);
            var controller = new ItemController(repositoryStub.Object, _loggerStub.Object);

            // Act
            var actualItems = await controller.GetItemsAsync();

            // Assert
            actualItems.Should().BeEquivalentTo(
                expectedItems,
                options => options.ComparingByMembers<Item>()
                );
        }

        [Fact]
        public async Task CreateItemsAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            // Arrange
            var itemToCreate = new CreateItemDto()
            {
                Name = Guid.NewGuid().ToString(),
                Price = _random.Next(1000)
            };
            var controller = new ItemController(repositoryStub.Object, _loggerStub.Object);

            // Act
            var result = await controller.CreateItemAsync(itemToCreate);

            // Assert
            var createdItem = (result.Result as CreatedAtActionResult)?.Value as ReadItemDto;
            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ReadItemDto>().ExcludingMissingMembers()
                );
            if (createdItem is not null)
            {
                createdItem.Id.Should().NotBeEmpty();
                //createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(10));
            }
        }
        
        [Fact]
        public async Task UpdateItemsAsync_WithExistingItem_ReturnsNoContent()
        {
            // Arrange
            var existingItem = CreateRandomItem();
            repositoryStub.Setup(repo =>
                    repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);
            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto()
            {
                Name = Guid.NewGuid().ToString(),
                Price = existingItem.Price + 5
            };
            var controller = new ItemController(repositoryStub.Object, _loggerStub.Object);

            // Act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
        
        [Fact]
        public async Task DeleteItemsAsync_WithExistingItem_ReturnsNoContent()
        {
            // Arrange
            var existingItem = CreateRandomItem();
            repositoryStub.Setup(repo =>
                    repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(existingItem);
            var controller = new ItemController(repositoryStub.Object, _loggerStub.Object);

            // Act
            var result = await controller.DeleteItemAsync(existingItem.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        private Item CreateRandomItem()
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = _random.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}