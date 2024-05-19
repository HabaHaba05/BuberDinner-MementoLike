using BuberDinner.Domain.MenuAggregate;

using Newtonsoft.Json;

using Xunit;

namespace BuberDinner.UnitTests.Domain;
public class MenuAggregateTests
{
    [Fact]
    public void FromState_Successfully_Creates_Menu_And_GetState_Returns_ProperState()
    {
        var menu = Menu.FromState(MenuStates.MenuState);
        Assert.NotNull(menu);

        var stateShouldBe = new Dictionary<string, object?>()
        {
            { "Keys", new[] { "Id" } },
            { "Id", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
            { "Name", "BeautifulName" },
            { "Description", "Best" },
            { "AverageRating_Value", 1.5 },
            { "AverageRating_NumRatings", 2 },
            { "HostId", Guid.Parse("6e6a7440-8db7-4401-8168-c945b92e3925") },
            { "CreatedDateTime", MenuStates.FreezedTime },
            { "MenuSections", new List<Dictionary<string, object?>>()
                {
                    new()
                    {
                        { "Keys", new[] { "MenuSectionId", "MenuId" } },
                        { "MenuSectionId", Guid.Parse("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6") },
                        { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                        { "Name", "Name of section" },
                        { "Description", "Description of section" },
                        { "MenuItems", new List<Dictionary<string, object?>>()
                            {
                                new()
                                {
                                    { "Keys", new[] { "MenuItemId", "MenuId", "MenuSectionId" } },
                                    { "MenuItemId", Guid.Parse("ef8475ae-5530-497d-b505-bd4e4abfc21e") },
                                    { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                                    { "MenuSectionId", Guid.Parse("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6") },
                                    { "Name",  "Item name" },
                                    { "Description",  "Item desc" },
                                },
                            }
                        },
                    },
                    new()
                    {
                        { "Keys", new[] { "MenuSectionId", "MenuId" } },
                        { "MenuSectionId", Guid.Parse("7ba296e5-a6db-4b6e-8c9e-93ab976f31b3") },
                        { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                        { "Name", "33_Name of section" },
                        { "Description", "33_Description of section" },
                        { "MenuItems", new List<Dictionary<string, object?>>() },
                    },
                }
            },
            { "MenuDinnerIds", new List<Guid>() },
            { "MenuReviewIds", new List<Dictionary<string, object?>>()
                {
                    new()
                    {
                        { "Keys", new[] { "ReviewId", "MenuId" } },
                        { "ReviewId", Guid.Parse("1583cbc3-d422-4932-8b7f-9cabcecf9b7c") },
                        { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                    },
                    new()
                    {
                        { "Keys", new[] { "ReviewId", "MenuId" } },
                        { "ReviewId", Guid.Parse("cb7784af-0d32-4bcf-9c9b-f3305998e28c") },
                        { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                    },
                }
            },
        };

        Assert.Equal(JsonConvert.SerializeObject(stateShouldBe), JsonConvert.SerializeObject(menu.GetState()));
    }
}