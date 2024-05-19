namespace BuberDinner.UnitTests;

internal class MenuStates
{
    internal static readonly DateTime FreezedTime = DateTime.Now;

    internal static Dictionary<string, object?> MenuState = new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
        { "Name", "BeautifulName" },
        { "Description", "Best" },
        { "AverageRating_Value", 1.5 },
        { "AverageRating_NumRatings", 2 },
        { "HostId", Guid.Parse("6e6a7440-8db7-4401-8168-c945b92e3925") },
        { "CreatedDateTime", FreezedTime },
        { "MenuReviewIds", new List<Guid>()
            {
                Guid.Parse("1583cbc3-d422-4932-8b7f-9cabcecf9b7c"),
                Guid.Parse("cb7784af-0d32-4bcf-9c9b-f3305998e28c"),
            }
        },
        { "MenuDinnerIds", new List<Guid>() },
        { "MenuSections", new List<Dictionary<string, object?>>()
            {
                new()
                {
                    { "Keys", new[] { "MenuId", "MenuSectionId" } },
                    { "MenuSectionId", Guid.Parse("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6") },
                    { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                    { "Description", "Description of section" },
                    { "Name", "Name of section" },
                    { "MenuItems", new List<Dictionary<string, object?>>()
                        {
                            new()
                            {
                                { "Keys", new[] { "MenuItemId", "MenuId", "MenuSectionId" } },
                                { "MenuItemId", Guid.Parse("ef8475ae-5530-497d-b505-bd4e4abfc21e") },
                                { "Name",  "Item name" },
                                { "Description",  "Item desc" },
                                { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                                { "MenuSectionId", Guid.Parse("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6") },
                            },
                        }
                    },
                },
                new()
                {
                    { "Keys", new[] { "MenuId", "MenuSectionId" } },
                    { "MenuSectionId", Guid.Parse("7ba296e5-a6db-4b6e-8c9e-93ab976f31b3") },
                    { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                    { "Description", "33_Description of section" },
                    { "Name", "33_Name of section" },
                    { "MenuItems", new List<Dictionary<string, object?>>() },
                },
            }
        },
    };

    /// <summary>
    /// Updates made on MenuState
    /// Differences:
    /// Changed AverageRatings
    /// Added new MenuReview with 682289d8-91d2-43d7-addf-fbfd0c59d233 and Removed with 1583cbc3-d422-4932-8b7f-9cabcecf9b7c
    /// Updated MenuItemName
    /// </summary>
    internal static Dictionary<string, object?> MenuState2 = new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
        { "Name", "BeautifulName" },
        { "Description", "Best" },
        { "AverageRating_Value", 3d },   // Changed
        { "AverageRating_NumRatings", 5 }, // Changed
        { "HostId", Guid.Parse("6e6a7440-8db7-4401-8168-c945b92e3925") },
        { "CreatedDateTime", FreezedTime },
        { "MenuReviewIds", new List<Guid>()
            {
                // Removed Guid.Parse("1583cbc3-d422-4932-8b7f-9cabcecf9b7c"),
                Guid.Parse("cb7784af-0d32-4bcf-9c9b-f3305998e28c"),
                Guid.Parse("682289d8-91d2-43d7-addf-fbfd0c59d233"),
            }
        },
        { "MenuDinnerIds", new List<Guid>() },
        { "MenuSections", new List<Dictionary<string, object?>>()
            {
                new() {
                    { "Keys", new[] { "MenuId", "MenuSectionId" } },
                    { "MenuSectionId", Guid.Parse("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6") },
                    { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                    { "Description", "Description of section" },
                    { "Name", "Name of section" },
                    { "MenuItems", new List<Dictionary<string, object?>>()
                        {
                            new()
                            {
                                { "Keys", new[] { "MenuItemId", "MenuId", "MenuSectionId" } },
                                { "MenuItemId", Guid.Parse("ef8475ae-5530-497d-b505-bd4e4abfc21e") },
                                { "Name",  "Item name Updated" }, // Updated
                                { "Description",  "Item desc" },
                                { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                                { "MenuSectionId", Guid.Parse("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6") },
                            },
                        }
                    },
                },
                new()
                {
                    { "Keys", new[] { "MenuId", "MenuSectionId" } },
                    { "MenuSectionId", Guid.Parse("7ba296e5-a6db-4b6e-8c9e-93ab976f31b3") },
                    { "MenuId", Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1") },
                    { "Description", "33_Description of section" },
                    { "Name", "33_Name of section" },
                    { "MenuItems", new List<Dictionary<string, object?>>() },
                },
            }
        },
    };
}