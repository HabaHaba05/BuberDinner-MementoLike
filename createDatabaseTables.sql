/****** [Audit] ******/
CREATE TABLE [dbo].[Audit](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[ActionType] [nvarchar](50) NOT NULL,
	[TableName] [nvarchar](100) NOT NULL,
	[PreviousState] [nvarchar](max) NULL,
	[NewState] [nvarchar](max) NOT NULL,
	[Changes] [nvarchar](max) NULL,
	[ActionTime] [datetime2](7) NOT NULL
	)
GO

/****** [Admins] ******/
CREATE TABLE [dbo].[Admins](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
);


/****** [Users] ******/
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
)

/****** [Guests] ******/
CREATE TABLE [dbo].[Guests](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
)

/****** [BecomeHostRequests] ******/
CREATE TABLE [dbo].[BecomeHostRequests](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[UserId] [uniqueidentifier] NOT NULL,
	[ReviewedByAdminId] [uniqueidentifier] NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
	[ReviewedAt] [datetime2](7) NULL,
	[Status] [int] NOT NULL,
)

/****** [Hosts] ******/
CREATE TABLE [dbo].[Hosts](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[AverageRating_Value] [float] NULL,
	[AverageRating_NumRatings] [int] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
	[UpdatedDateTime] [datetime2](7) NULL,
)

/****** [Bills] ******/
CREATE TABLE [dbo].[Bills](
	[Id] [nvarchar](450) NOT NULL PRIMARY KEY,
	[DinnerId] [uniqueidentifier] NOT NULL,
	[GuestId] [uniqueidentifier] NOT NULL,
	[HostId] [uniqueidentifier] NOT NULL,
	[Amount_Amount] [decimal](10, 4) NOT NULL,
	[Amount_Currency] [nvarchar](max) NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
	[PaidAt] [datetime2](7) NULL,
	[Status] [int] NOT NULL DEFAULT (0),
)

/****** [Dinners] ******/
CREATE TABLE [dbo].[Dinners](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[StartDateTime] [datetime2](7) NOT NULL,
	[EndDateTime] [datetime2](7) NOT NULL,
	[StartedDateTime] [datetime2](7) NULL,
	[EndedDateTime] [datetime2](7) NULL,
	[Status] [int] NOT NULL,
	[IsPublic] [bit] NOT NULL,
	[MaxGuests] [int] NOT NULL,
	[Price_Amount] [decimal](10, 4) NOT NULL,
	[Price_Currency] [nvarchar](max) NOT NULL,
	[HostId] [uniqueidentifier] NOT NULL,
	[MenuId] [uniqueidentifier] NOT NULL,
	[Location_Name] [nvarchar](max) NOT NULL,
	[Location_Address] [nvarchar](max) NOT NULL,
	[Location_Latitude] [float] NOT NULL,
	[Location_Longitude] [float] NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
)

/****** [Menus] ******/
CREATE TABLE [dbo].[Menus](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[AverageRating_Value] [float] NULL,
	[AverageRating_NumRatings] [int] NOT NULL,
	[HostId] [uniqueidentifier] NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
)

/****** [MenuSections] ******/
CREATE TABLE [dbo].[MenuSections](
	[MenuSectionId] [uniqueidentifier] NOT NULL,
	[MenuId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	CONSTRAINT [PK_MenuSections] PRIMARY KEY ([MenuSectionId], [MenuId]),
	CONSTRAINT [FK_MenuSections_Menus_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([Id]) ON DELETE CASCADE,
)

/****** [MenuItems] ******/
CREATE TABLE [dbo].[MenuItems](
	[MenuItemId] [uniqueidentifier] NOT NULL,
	[MenuSectionId] [uniqueidentifier] NOT NULL,
	[MenuId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	CONSTRAINT [PK_MenuItems] PRIMARY KEY ([MenuId], [MenuSectionId], [MenuItemId]),
	CONSTRAINT [FK_MenuItems_MenuSections_MenuSectionId_MenuId] FOREIGN KEY ([MenuSectionId],[MenuId]) REFERENCES [dbo].[MenuSections] ([MenuSectionId], [MenuId]) ON DELETE CASCADE,
)

/****** [DinnerReservations] ******/
CREATE TABLE [dbo].[DinnerReservations](
	[ReservationId] [uniqueidentifier] NOT NULL,
	[DinnerId] [uniqueidentifier] NOT NULL,
	[GuestCount] [int] NOT NULL,
	[GuestId] [uniqueidentifier] NOT NULL,
	[BillId] [nvarchar](100) NULL,
	[Status] [int] NOT NULL,
	[ArrivalDateTime] [datetime2](7) NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
	[UpdatedDateTime] [datetime2](7) NULL,
	CONSTRAINT [PK_DinnerReservations] PRIMARY KEY ([DinnerId], [ReservationId]),
	CONSTRAINT [FK_DinnerReservations_Dinners_DinnerId] FOREIGN KEY ([DinnerId]) REFERENCES [dbo].[Dinners] ([Id]) ON DELETE CASCADE
)

/****** [MenuReviews] ******/
CREATE TABLE [dbo].[MenuReviews](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Rating_Value] [int] NOT NULL,
	[Comment] [nvarchar](500) NOT NULL,
	[HostId] [uniqueidentifier] NOT NULL,
	[MenuId] [uniqueidentifier] NOT NULL,
	[GuestId] [uniqueidentifier] NOT NULL,
	[DinnerId] [uniqueidentifier] NOT NULL,
)

/****** [GuestBillIds] ******/
CREATE TABLE [dbo].[GuestBillIds](
	[GuestId] [uniqueidentifier] NOT NULL,
	[BillId] [nvarchar](100) NOT NULL,
	CONSTRAINT [PK_GuestBillIds] PRIMARY KEY ([GuestId], [BillId]),
	CONSTRAINT [FK_GuestBillIds_Guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [dbo].[Guests] ([Id]) ON DELETE CASCADE,
)

/****** [GuestMenuReviewIds] ******/
CREATE TABLE [dbo].[GuestMenuReviewIds](
	[GuestId] [uniqueidentifier] NOT NULL,
	[MenuReviewId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_GuestMenuReviewIds] PRIMARY KEY ([GuestId], [MenuReviewId]),
	CONSTRAINT [FK_GuestMenuReviewIds_Guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [dbo].[Guests] ([Id]) ON DELETE CASCADE,

)

/****** [GuestOngoingReservationIds] ******/
CREATE TABLE [dbo].[GuestOngoingReservationIds](
	[GuestId] [uniqueidentifier] NOT NULL,
	[ReservationId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_GuestOngoingReservationIds] PRIMARY KEY ([GuestId], [ReservationId]),
	CONSTRAINT [FK_GuestOngoingReservationIds_Guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [dbo].[Guests] ([Id]) ON DELETE CASCADE,
)

/****** [GuestPastReservationIds] ******/
CREATE TABLE [dbo].[GuestPastReservationIds](
	[GuestId] [uniqueidentifier] NOT NULL,
	[ReservationId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_GuestPastReservationIds] PRIMARY KEY ([GuestId], [ReservationId]),
	CONSTRAINT [FK_GuestPastReservationIds_Guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [dbo].[Guests] ([Id]) ON DELETE CASCADE,
)

/****** [GuestPendingReservationIds] ******/
CREATE TABLE [dbo].[GuestPendingReservationIds](
	[GuestId] [uniqueidentifier] NOT NULL,
	[ReservationId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_GuestPendingReservationIds] PRIMARY KEY ([GuestId], [ReservationId]),
	CONSTRAINT [FK_GuestPendingReservationIds_Guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [dbo].[Guests] ([Id]) ON DELETE CASCADE,
)

/****** [GuestRatings] ******/
CREATE TABLE [dbo].[GuestRatings](
	[GuestRatingId] [uniqueidentifier] NOT NULL,
	[GuestId] [uniqueidentifier] NOT NULL,
	[HostId] [uniqueidentifier] NOT NULL,
	[DinnerId] [uniqueidentifier] NOT NULL,
	[Rating_Value] [int] NOT NULL,
	[CreatedDateTime] [datetime2](7) NOT NULL,
	CONSTRAINT [PK_GuestRatings] PRIMARY KEY ([GuestId], [GuestRatingId]),
	CONSTRAINT [FK_GuestRatings_Guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [dbo].[Guests] ([Id]) ON DELETE CASCADE,
)

/****** [GuestUpcomingReservationIds] ******/
CREATE TABLE [dbo].[GuestUpcomingReservationIds](
	[GuestId] [uniqueidentifier] NOT NULL,
	[ReservationId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_GuestUpcomingReservationIds] PRIMARY KEY ([GuestId], [ReservationId]),
	CONSTRAINT [FK_GuestUpcomingReservationIds_Guests_GuestId] FOREIGN KEY ([GuestId]) REFERENCES [dbo].[Guests] ([Id]) ON DELETE CASCADE,
)

/****** [HostDinnerIds] ******/
CREATE TABLE [dbo].[HostDinnerIds](
	[HostId] [uniqueidentifier] NOT NULL,
	[HostDinnerId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_HostDinnerIds] PRIMARY KEY ([HostId], [HostDinnerId]),
	CONSTRAINT [FK_HostDinnerIds_Hosts_HostId] FOREIGN KEY ([HostId]) REFERENCES [dbo].[Hosts] ([Id]) ON DELETE CASCADE,
)

/****** [HostMenuIds] ******/
CREATE TABLE [dbo].[HostMenuIds](
	[HostId] [uniqueidentifier] NOT NULL,
	[HostMenuId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_HostMenuIds] PRIMARY KEY ([HostId], [HostMenuId]),
	CONSTRAINT [FK_HostMenuIds_Hosts_HostId] FOREIGN KEY ([HostId]) REFERENCES [dbo].[Hosts] ([Id]) ON DELETE CASCADE,
)

/****** [MenuDinnerIds] ******/
CREATE TABLE [dbo].[MenuDinnerIds](
	[MenuId] [uniqueidentifier] NOT NULL,
	[DinnerId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_MenuDinnerIds] PRIMARY KEY ([MenuId], [DinnerId]),
	CONSTRAINT [FK_MenuDinnerIds_Menus_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([Id]) ON DELETE CASCADE,
)

/****** [MenuReviewIds] ******/
CREATE TABLE [dbo].[MenuReviewIds](
	[MenuId] [uniqueidentifier] NOT NULL,
	[ReviewId] [uniqueidentifier] NOT NULL,
	CONSTRAINT [PK_MenuReviewIds] PRIMARY KEY ([MenuId], [ReviewId]),
	CONSTRAINT [FK_MenuReviewIds_Menus_MenuId] FOREIGN KEY ([MenuId]) REFERENCES [dbo].[Menus] ([Id]) ON DELETE CASCADE,
)