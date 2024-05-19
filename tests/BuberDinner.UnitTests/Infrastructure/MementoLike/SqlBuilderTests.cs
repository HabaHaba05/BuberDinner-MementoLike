using BuberDinner.Domain.MenuAggregate;
using BuberDinner.Infrastructure.Persistence.MementoLikeHelpers.Builders;

using Dapper;

using Xunit;

namespace BuberDinner.UnitTests.Infrastructure.MementoLike;
public class SqlBuilderTests
{
    [Fact]
    public void Update_Creates_Proper_Queries()
    {
        // Arrange
        var oldState = Menu.FromState(MenuStates.MenuState).GetState();
        var newState = Menu.FromState(MenuStates.MenuState2).GetState();
        var tableName = "Menus";

        // Act
        var result = SqlQueryBuilder.Update(oldState, newState, tableName);

        // Assert
        Assert.Equal(4, result.Count);

        // Update menu ratings
        var builderResult = result[0];
        var parameters = builderResult.Parameters as SqlMapper.IParameterLookup;
        Assert.Equal(
            "UPDATE Menus SET AverageRating_Value=@AverageRating_Value, AverageRating_NumRatings=@AverageRating_NumRatings WHERE (Id=@Id);",
            builderResult.Query);
        Assert.Equal(3d, parameters["AverageRating_Value"]);
        Assert.Equal(5, parameters["AverageRating_NumRatings"]);
        Assert.Equal(Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1"), parameters["Id"]);

        // Update MenuItem Name
        builderResult = result[1];
        parameters = builderResult.Parameters;
        Assert.Equal(
            "UPDATE MenuItems SET Name=@Name WHERE (MenuItemId=@MenuItemId AND  MenuId=@MenuId AND  MenuSectionId=@MenuSectionId);",
            builderResult.Query);
        Assert.Equal("Item name Updated", parameters["Name"]);
        Assert.Equal(Guid.Parse("ef8475ae-5530-497d-b505-bd4e4abfc21e"), parameters["MenuItemId"]);
        Assert.Equal(Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1"), parameters["MenuId"]);
        Assert.Equal(Guid.Parse("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6"), parameters["MenuSectionId"]);

        // Add MenuReviewId
        builderResult = result[2];
        parameters = builderResult.Parameters;
        Assert.Equal(
            "INSERT INTO MenuReviewIds (ReviewId, MenuId) VALUES (@ReviewId, @MenuId);",
            builderResult.Query);
        Assert.Equal("682289d8-91d2-43d7-addf-fbfd0c59d233", parameters["ReviewId"]);
        Assert.Equal("9e2ab586-27cf-4b8d-b50b-3275800536e1", parameters["MenuId"]);

        // Remove MenuReviewId
        builderResult = result[3];
        parameters = builderResult.Parameters;
        Assert.Equal(
            "DELETE FROM MenuReviewIds WHERE ReviewId=@ReviewId AND  MenuId=@MenuId;",
            builderResult.Query);
        Assert.Equal(Guid.Parse("1583cbc3-d422-4932-8b7f-9cabcecf9b7c"), parameters["ReviewId"]);
        Assert.Equal(Guid.Parse("9e2ab586-27cf-4b8d-b50b-3275800536e1"), parameters["MenuId"]);
    }

    [Fact]
    public void Insert_Creates_Queries_For_Owned_Objects()
    {
        // Arrange
        var state = Menu.FromState(MenuStates.MenuState).GetState();
        var tableName = "Menus";

        // Act
        var result = SqlQueryBuilder.Insert(state, tableName);

        // Assert
        Assert.Equal(4, result.Count);

        // Insert menu
        var builderResult = result[0];
        var parameters = builderResult.Parameters as SqlMapper.IParameterLookup;
        Assert.Equal(
            "INSERT INTO Menus (Id, Name, Description, AverageRating_Value, AverageRating_NumRatings, HostId, CreatedDateTime) VALUES(@0_Id, @0_Name, @0_Description, @0_AverageRating_Value, @0_AverageRating_NumRatings, @0_HostId, @0_CreatedDateTime);",
            builderResult.Query);
        Assert.Equal("9e2ab586-27cf-4b8d-b50b-3275800536e1", parameters["0_Id"]);
        Assert.Equal("BeautifulName", parameters["0_Name"]);
        Assert.Equal("Best", parameters["0_Description"]);
        Assert.Equal(1.5, parameters["0_AverageRating_Value"]);
        Assert.Equal(2, parameters["0_AverageRating_NumRatings"]);
        Assert.Equal("6e6a7440-8db7-4401-8168-c945b92e3925", parameters["0_HostId"]);
        Assert.Equal(MenuStates.FreezedTime, parameters["0_CreatedDateTime"]);

        // Insert Menu Sections
        builderResult = result[1];
        parameters = builderResult.Parameters;
        Assert.Equal(
            "INSERT INTO MenuSections (MenuSectionId, MenuId, Name, Description) VALUES(@0_MenuSectionId, @0_MenuId, @0_Name, @0_Description),(@1_MenuSectionId, @1_MenuId, @1_Name, @1_Description);",
            builderResult.Query);
        Assert.Equal("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6", parameters["0_MenuSectionId"]);
        Assert.Equal("9e2ab586-27cf-4b8d-b50b-3275800536e1", parameters["0_MenuId"]);
        Assert.Equal("Name of section", parameters["0_Name"]);
        Assert.Equal("Description of section", parameters["0_Description"]);
        Assert.Equal("7ba296e5-a6db-4b6e-8c9e-93ab976f31b3", parameters["1_MenuSectionId"]);
        Assert.Equal("9e2ab586-27cf-4b8d-b50b-3275800536e1", parameters["1_MenuId"]);
        Assert.Equal("33_Name of section", parameters["1_Name"]);
        Assert.Equal("33_Description of section", parameters["1_Description"]);

        // Insert MenuItems
        builderResult = result[2];
        parameters = builderResult.Parameters;
        Assert.Equal(
            "INSERT INTO MenuItems (MenuItemId, MenuId, MenuSectionId, Name, Description) VALUES(@0_MenuItemId, @0_MenuId, @0_MenuSectionId, @0_Name, @0_Description);",
            builderResult.Query);
        Assert.Equal("ef8475ae-5530-497d-b505-bd4e4abfc21e", parameters["0_MenuItemId"]);
        Assert.Equal("41631e97-8f5d-47dd-a5a4-f49d2cbb6dd6", parameters["0_MenuSectionId"]);
        Assert.Equal("9e2ab586-27cf-4b8d-b50b-3275800536e1", parameters["0_MenuId"]);
        Assert.Equal("Item name", parameters["0_Name"]);
        Assert.Equal("Item desc", parameters["0_Description"]);

        // Insert MenuReviewIds
        builderResult = result[3];
        parameters = builderResult.Parameters;
        Assert.Equal(
            "INSERT INTO MenuReviewIds (ReviewId, MenuId) VALUES(@0_ReviewId, @0_MenuId),(@1_ReviewId, @1_MenuId);",
            builderResult.Query);
        Assert.Equal("1583cbc3-d422-4932-8b7f-9cabcecf9b7c", parameters["0_ReviewId"]);
        Assert.Equal("9e2ab586-27cf-4b8d-b50b-3275800536e1", parameters["0_MenuId"]);
        Assert.Equal("cb7784af-0d32-4bcf-9c9b-f3305998e28c", parameters["1_ReviewId"]);
        Assert.Equal("9e2ab586-27cf-4b8d-b50b-3275800536e1", parameters["1_MenuId"]);
    }

    [Fact]
    public void Select_Creates_Proper_Query()
    {
        // Arrange
        var tableName = "TestTableName";

        // Act
        var result = SqlQueryBuilder.Select(tableName, "*", [new("Prop1", "Val1"), new("Prop2", 3)]);

        // Assert
        Assert.Equal($"SELECT * FROM {tableName} WHERE Prop1=@Prop1 AND  Prop2=@Prop2;", result.Query);
        Assert.Equal("Val1", (result.Parameters as SqlMapper.IParameterLookup)["Prop1"]);
        Assert.Equal(3, (result.Parameters as SqlMapper.IParameterLookup)["Prop2"]);
    }
}