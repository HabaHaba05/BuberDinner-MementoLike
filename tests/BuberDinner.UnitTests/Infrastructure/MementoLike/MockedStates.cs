namespace MementoLike.UnitTests;

internal class MockedStates
{
    internal static readonly DateTime FreezedTime = DateTime.Now;

    internal static Dictionary<string, object?> MenuState = new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", "1_Id" },
        { "Name", "BeautifulName" },
        { "Description", "Best" },
        { "AverageRating_Value", 1.5 },
        { "AverageRating_NumRatings", 2 },
        { "HostId", "6e6a7440-8db7-4401-8168-c945b92e3925" },
        { "CreatedDateTime", FreezedTime },
        { "MenuReviewIds", new[]
            {
                new Dictionary<string, object?>
                {
                    { "Keys", new[] { "ReviewId", "MenuId" } },
                    { "ReviewId", 1 },
                    { "MenuId", "1_Id" },
                },
                new Dictionary<string, object?>
                {
                    { "Keys", new[] { "ReviewId", "MenuId" } },
                    { "ReviewId", 2 },
                    { "MenuId", "1_Id" },
                },
            }
        },
        { "MenuDinnerIds", Array.Empty<object?>() },
        { "MenuSections", new[]
            {
                new Dictionary<string, object?>
                {
                    { "Keys", new[] {"Id"} },
                    { "Id", 1 },
                    { "MenuId", "1_Id" },
                    { "Description", "Description of section" },
                    { "Name", "Name of section" },
                    { "MenuItems", new[]
                        {
                            new Dictionary<string, object?>
                            {
                                { "Keys", new[] { "MenuItemId" } },
                                { "MenuItemId", "ef8475ae-5530-497d-b505-bd4e4abfc21e" },
                                { "Name",  "Item name" },
                                { "Description",  "Item desc" },
                            },
                        }
                    },
                },
                new Dictionary<string, object?>
                {
                    { "Keys", new[] {"Id"} },
                    { "Id", 33 },
                    { "MenuId", "33_Id" },
                    { "Description", "33_Description of section" },
                    { "Name", "33_Name of section" },
                    { "MenuItems", Array.Empty<object?>() },
                },
            }
        },
    };

    /// <summary>
    /// Updates made on MenuState
    /// Differences:
    /// Changed AverageRatings
    /// Added new MenuReview with Id3 and Removed with Id1
    /// Updated MenuItemName
    /// </summary>
    internal static Dictionary<string, object?> MenuState2 = new()
    {
        { "Keys", new[] { "Id" } },
        { "Id", "1_Id" },
        { "Name", "BeautifulName" },
        { "Description", "Best" },
        { "AverageRating_Value", 3 },   // Changed
        { "AverageRating_NumRatings", 5 }, // Changed
        { "HostId", "6e6a7440-8db7-4401-8168-c945b92e3925" },
        { "CreatedDateTime", FreezedTime },
        { "MenuReviewIds", new[]
            {
                // Removed:
                // new Dictionary<string, object?>
                // {
                //     { "Keys", new[] { "ReviewId", "MenuId" } },
                //     { "ReviewId", 1 },
                //     { "MenuId", "1_Id" },
                // },
                new Dictionary<string, object?>
                {
                    { "Keys", new[] { "ReviewId", "MenuId" } },
                    { "ReviewId", 2 },
                    { "MenuId", "1_Id" },
                },
                new Dictionary<string, object?>
                {
                    { "Keys", new[] { "ReviewId", "MenuId" } },
                    { "ReviewId", 3 },
                    { "MenuId", "1_Id" },
                },
            }
        },
        { "MenuDinnerIds", Array.Empty<object?>() },
        { "MenuSections", new[]
            {
                new Dictionary<string, object?>
                {
                    { "Keys", new[] { "Id" } },
                    { "Id", 1 },
                    { "MenuId", "1_Id" },
                    { "Description", "Description of section" },
                    { "Name", "Name of section" },
                    { "MenuItems", new[]
                        {
                            new Dictionary<string, object?>
                            {
                                { "Keys", new[] { "MenuItemId" } },
                                { "MenuItemId", "ef8475ae-5530-497d-b505-bd4e4abfc21e" },
                                { "Name",  "Item name Updated" }, // Updated
                                { "Description",  "Item desc" },
                            },
                        }
                    },
                },
                new Dictionary<string, object?>
                {
                    { "Keys", new[] {"Id"} },
                    { "Id", 33 },
                    { "MenuId", "33_Id" },
                    { "Description", "33_Description of section" },
                    { "Name", "33_Name of section" },
                    { "MenuItems", Array.Empty<object?>() },
                },
            }
        },
    };
}