using BuberDinner.Domain.MenuAggregate;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers;
using BuberDinner.UnitTests;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace MementoLike.UnitTests;

public class StateToTableStructureConverterTests
{
    [Fact]
    public void It_Should_Convert_To_Proper_Object()
    {
        var menu = Menu.FromState(MenuStates.MenuState);
        var result = StateToTableStructureConverter.Handle(menu.GetState(), "Menus");

        // Assert
        // Menus
        Assert.True(result.ContainsKey("Menus"));
        Assert.True(new[] { "Id" }.SequenceEqual(result["Menus"].Keys));
        Assert.Single(result["Menus"].States);
        Assert.Equal(
            JsonConvert.SerializeObject(result["Menus"].States[0]),
            JsonConvert.SerializeObject(new JObject()
            {
                { "Id", "9e2ab586-27cf-4b8d-b50b-3275800536e1" },
                { "Name", "BeautifulName" },
                { "Description", "Best" },
                { "AverageRating_Value", 1.5 },
                { "AverageRating_NumRatings", 2 },
                { "HostId", "6e6a7440-8db7-4401-8168-c945b92e3925" },
                { "CreatedDateTime", MenuStates.FreezedTime },
            }));

        // MenuSections
        Assert.True(result.ContainsKey("MenuSections"));
        Assert.True(new[] { "MenuSectionId", "MenuId" }.SequenceEqual(result["MenuSections"].Keys));
        Assert.Equal(2, result["MenuSections"].States.Count);
        Assert.Equal(
            JsonConvert.SerializeObject(result["MenuSections"].States[0]),
            JsonConvert.SerializeObject(new JObject()
            {
                { "MenuSectionId", "41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6" },
                { "MenuId", "9e2ab586-27cf-4b8d-b50b-3275800536e1" },
                { "Name", "Name of section" },
                { "Description", "Description of section" },
            }));
        Assert.Equal(
            JsonConvert.SerializeObject(result["MenuSections"].States[1]),
            JsonConvert.SerializeObject(new JObject()
            {
                { "MenuSectionId", "7ba296e5-a6db-4b6e-8c9e-93ab976f31b3" },
                { "MenuId", "9e2ab586-27cf-4b8d-b50b-3275800536e1" },
                { "Name", "33_Name of section" },
                { "Description", "33_Description of section" },
            }));

        // MenuItems
        Assert.True(result.ContainsKey("MenuItems"));
        Assert.True(new[] { "MenuItemId", "MenuId", "MenuSectionId" }.SequenceEqual(result["MenuItems"].Keys));
        Assert.Single(result["MenuItems"].States);
        Assert.Equal(
            JsonConvert.SerializeObject(result["MenuItems"].States[0]),
            JsonConvert.SerializeObject(new JObject()
            {
                { "MenuItemId", "ef8475ae-5530-497d-b505-bd4e4abfc21e" },
                { "MenuId", "9e2ab586-27cf-4b8d-b50b-3275800536e1" },
                { "MenuSectionId", "41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6" },
                { "Name",  "Item name" },
                { "Description",  "Item desc" },
            }));

        // MenuReviewIds
        Assert.True(result.ContainsKey("MenuReviewIds"));
        Assert.True(new[] { "ReviewId", "MenuId" }.SequenceEqual(result["MenuReviewIds"].Keys));
        Assert.Equal(2, result["MenuReviewIds"].States.Count);
        Assert.Equal(
            JsonConvert.SerializeObject(result["MenuReviewIds"].States[0]),
            JsonConvert.SerializeObject(new JObject()
            {
                { "ReviewId", "1583cbc3-d422-4932-8b7f-9cabcecf9b7c" },
                { "MenuId", "9e2ab586-27cf-4b8d-b50b-3275800536e1" },
            }));
        Assert.Equal(
            JsonConvert.SerializeObject(result["MenuReviewIds"].States[1]),
            JsonConvert.SerializeObject(new JObject()
            {
                { "ReviewId", "cb7784af-0d32-4bcf-9c9b-f3305998e28c" },
                { "MenuId", "9e2ab586-27cf-4b8d-b50b-3275800536e1" },
            }));
    }
}