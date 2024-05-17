using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;

using Newtonsoft.Json.Linq;

using Xunit;

namespace MementoLike.UnitTests;

public class StateToTableStructureConverterTests
{
    [Fact]
    public void It_Should_Convert_To_Proper_Object()
    {
        var result = StateToTableStructureConverter.Handle(MockedStates.MenuState, "Menus");

        // Assert
        // Menus
        Assert.True(result.ContainsKey("Menus"));
        Assert.True(new[] { "Id" }.SequenceEqual(result["Menus"].Keys));
        Assert.Single(result["Menus"].States);
        Assert.True(JToken.DeepEquals(result["Menus"].States[0], new JObject()
        {
            { "Id", "1_Id" },
            { "Name", "BeautifulName" },
            { "Description", "Best" },
            { "AverageRating_Value", 1.5 },
            { "AverageRating_NumRatings", 2 },
            { "HostId", "6e6a7440-8db7-4401-8168-c945b92e3925" },
            { "CreatedDateTime", MockedStates.FreezedTime },
        }));

        // MenuSections
        Assert.True(result.ContainsKey("MenuSections"));
        Assert.True(new[] { "Id" }.SequenceEqual(result["MenuSections"].Keys));
        Assert.Equal(2, result["MenuSections"].States.Count);
        Assert.True(JToken.DeepEquals(result["MenuSections"].States[0], new JObject()
        {
                { "Id", 1 },
                { "MenuId", "1_Id" },
                { "Description", "Description of section" },
                { "Name", "Name of section" },
        }));
        Assert.True(JToken.DeepEquals(result["MenuSections"].States[1], new JObject()
        {
                { "Id", 33 },
                { "MenuId", "33_Id" },
                { "Description", "33_Description of section" },
                { "Name", "33_Name of section" },
        }));

        // MenuItems
        Assert.True(result.ContainsKey("MenuItems"));
        Assert.True(new[] { "MenuItemId" }.SequenceEqual(result["MenuItems"].Keys));
        Assert.Single(result["MenuItems"].States);
        Assert.True(JToken.DeepEquals(result["MenuItems"].States[0], new JObject()
        {
            { "MenuItemId", "ef8475ae-5530-497d-b505-bd4e4abfc21e" },
            { "Name",  "Item name" },
            { "Description",  "Item desc" },
        }));

        // MenuReviewIds
        Assert.True(result.ContainsKey("MenuReviewIds"));
        Assert.True(new[] { "ReviewId", "MenuId" }.SequenceEqual(result["MenuReviewIds"].Keys));
        Assert.Equal(2, result["MenuReviewIds"].States.Count);
        Assert.True(JToken.DeepEquals(result["MenuReviewIds"].States[0], new JObject()
        {
            { "ReviewId", 1 },
            { "MenuId", "1_Id" },
        }));
        Assert.True(JToken.DeepEquals(result["MenuReviewIds"].States[1], new JObject()
        {
            { "ReviewId", 2 },
            { "MenuId", "1_Id" },
        }));
    }
}