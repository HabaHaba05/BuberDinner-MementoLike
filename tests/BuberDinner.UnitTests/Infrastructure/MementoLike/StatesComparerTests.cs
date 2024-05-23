using BuberDinner.Domain.MenuAggregate;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
using BuberDinner.UnitTests;

using Newtonsoft.Json.Linq;

using Xunit;

namespace MementoLike.UnitTests;

public class StatesComparerTests
{
    [Fact]
    public void When_Identical_Should_Return_Empty_List()
    {
        // Arrange
        var oldState = Menu.FromState(MenuStates.MenuState).GetState();
        var newState = Menu.FromState(MenuStates.MenuState).GetState();

        // Act
        var result = StatesComparator.Handle(oldState, newState, "Menus");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void It_Should_Convert_To_Proper_Object()
    {
        // Arrange
        var oldState = Menu.FromState(MenuStates.MenuState).GetState();
        var newState = Menu.FromState(MenuStates.MenuState2).GetState();
        var menuId = Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1");

        // Act
        var result = StatesComparator.Handle(oldState, newState, "Menus");

        // Assert
        Assert.NotNull(result);

        // Menu: Updated AverageRating
        Assert.True(result.ContainsKey("Menus"));
        Assert.Single(result["Menus"]);
        Assert.Equal(ActionType.UPDATE, result["Menus"][0].ActionType);
        Assert.Equivalent(new List<KeyValuePair<string, object>>() { new("Id", menuId) }, result["Menus"][0].Keys);
        Assert.True(JToken.DeepEquals(result["Menus"][0].Changes, new JObject()
        {
            { "AverageRating_Value", 3d },
            { "AverageRating_NumRatings", 5 },
        }));

        // MenuReviews: 1 Removed, 1 Added
        Assert.True(result.ContainsKey("MenuReviewIds"));
        Assert.Equal(2, result["MenuReviewIds"].Count);
        Assert.Equal(ActionType.INSERT, result["MenuReviewIds"][0].ActionType);
        Assert.Equivalent(
            new List<KeyValuePair<string, object>>()
            {
                new("ReviewId", Guid.Parse("682289d8-91d2-43d7-addf-fbfd0c59d233")),
                new("MenuId", menuId),
            },
            result["MenuReviewIds"][0].Keys);
        Assert.True(JToken.DeepEquals(result["MenuReviewIds"][0].Changes, new JObject()
        {
            { "ReviewId", Guid.Parse("682289d8-91d2-43d7-addf-fbfd0c59d233") },
            { "MenuId", menuId },
        }));

        Assert.Equal(ActionType.REMOVE, result["MenuReviewIds"][1].ActionType);
        Assert.Equivalent(
            new List<KeyValuePair<string, object>>()
            {
                new("ReviewId", Guid.Parse("1583cbc3-d422-4932-8b7f-9cabcecf9b7c")),
                new("MenuId", menuId),
            },
            result["MenuReviewIds"][1].Keys);
        Assert.True(JToken.DeepEquals(result["MenuReviewIds"][1].Changes, new JObject()
        {
            { "ReviewId", Guid.Parse("1583cbc3-d422-4932-8b7f-9cabcecf9b7c") },
            { "MenuId", menuId },
        }));

        // MenuItem: Updated Description
        Assert.True(result.ContainsKey("MenuItems"));
        Assert.Single(result["MenuItems"]);
        Assert.Equal(ActionType.UPDATE, result["MenuItems"][0].ActionType);
        Assert.Equivalent(
            new List<KeyValuePair<string, object>>()
            {
                new("MenuItemId", Guid.Parse("ef8475ae-5530-497d-b505-bd4e4abfc21e")),
                new("MenuId", menuId),
                new("MenuSectionId", Guid.Parse("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6")),
            },
            result["MenuItems"][0].Keys);
        Assert.True(JToken.DeepEquals(result["MenuItems"][0].Changes, new JObject()
        {
            { "Name",  "Item name Updated" },
        }));
    }
}