using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace MementoLike.UnitTests;

public class StatesComparerTests
{
    [Fact]
    public void When_Identical_Should_Return_Empty_List()
    {
        // Arrange
        var oldState = MockedStates.MenuState;
        var newState = MockedStates.MenuState;

        // Act
        var result = StatesComparer.Handle(oldState, newState, "Menus");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void It_Should_Convert_To_Proper_Object()
    {
        // Arrange
        var oldState = MockedStates.MenuState;
        var newState = MockedStates.MenuState2;

        var old = JsonConvert.SerializeObject(oldState);
        var neww = JsonConvert.SerializeObject(newState);

        // Act
        var result = StatesComparer.Handle(oldState, newState, "Menus");
        var resulst = JsonConvert.SerializeObject(result);

        // Assert
        Assert.NotNull(result);

        // Menu: Updated AverageRating
        Assert.True(result.ContainsKey("Menus"));
        Assert.Single(result["Menus"]);
        Assert.Equal(ActionType.UPDATE, result["Menus"][0].ActionType);
        Assert.Equivalent(new List<KeyValuePair<string, object>>() { new("Id", "1_Id") }, result["Menus"][0].Keys);
        Assert.True(JToken.DeepEquals(result["Menus"][0].Changes, new JObject()
        {
            { "AverageRating_Value", 3 },
            { "AverageRating_NumRatings", 5 },
        }));

        // MenuReviews: 1 Removed, 1 Added
        Assert.True(result.ContainsKey("MenuReviewIds"));
        Assert.Equal(2, result["MenuReviewIds"].Count);
        Assert.Equal(ActionType.INSERT, result["MenuReviewIds"][0].ActionType);
        Assert.Equivalent(new List<KeyValuePair<string, object>>() { new("ReviewId", 3), new("MenuId", "1_Id") }, result["MenuReviewIds"][0].Keys);
        Assert.True(JToken.DeepEquals(result["MenuReviewIds"][0].Changes, new JObject()
        {
            { "ReviewId", 3 },
            { "MenuId", "1_Id" },
        }));
        Assert.Equal(ActionType.REMOVE, result["MenuReviewIds"][1].ActionType);
        Assert.Equivalent(new List<KeyValuePair<string, object>>() { new("ReviewId", 1), new("MenuId", "1_Id") }, result["MenuReviewIds"][1].Keys);
        Assert.True(JToken.DeepEquals(result["MenuReviewIds"][1].Changes, new JObject()
        {
            { "ReviewId", 1 },
            { "MenuId", "1_Id" },
        }));

        // MenuItem: Updated Description
        Assert.True(result.ContainsKey("MenuItems"));
        Assert.Single(result["MenuItems"]);
        Assert.Equal(ActionType.UPDATE, result["MenuItems"][0].ActionType);
        Assert.Equivalent(new List<KeyValuePair<string, object>>() { new("MenuItemId", "ef8475ae-5530-497d-b505-bd4e4abfc21e") }, result["MenuItems"][0].Keys);
        Assert.True(JToken.DeepEquals(result["MenuItems"][0].Changes, new JObject()
        {
            { "Name",  "Item name Updated" },
        }));
    }
}