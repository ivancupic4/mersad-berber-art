--ALTER DATABASE MersadBerberArt COLLATE Latin1_General_100_CI_AS_SC_UTF8;
/*
drop database MersadBerberArt

DROP TABLE [OrderItems];
DROP TABLE [Order];
DROP TABLE [Art];
DROP TABLE [ArtType];
*/

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MersadBerberArt')
BEGIN
	CREATE DATABASE MersadBerberArt COLLATE Latin1_General_100_CI_AS_SC_UTF8;
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ArtType')
BEGIN
	CREATE TABLE [ArtType] (
		[Id] INT PRIMARY KEY IDENTITY,
		[Name] VARCHAR(200)
	);
	
	INSERT INTO [ArtType]
	VALUES ('Painting'), ('Print');
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Art')
BEGIN
	CREATE TABLE [Art] (
		[Id] INT PRIMARY KEY IDENTITY,
		[Name] VARCHAR(200),
		[ArtTypeId] INT NOT NULL, 
		[Description] VARCHAR(1024),
		[DateCreated] DATE,
		[Price] DECIMAL(10, 2) NOT NULL,
		[ImageUrl] VARCHAR(200),

		FOREIGN KEY ([ArtTypeId]) REFERENCES [ArtType] ([Id])
	);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Order')
BEGIN
	CREATE TABLE [Order] (
		[Id] INT PRIMARY KEY IDENTITY,
		[DateCreated] DATE NOT NULL,
		[TotalPrice] DECIMAL(10, 2) NOT NULL,
		[FirstName] VARCHAR(200),
		[LastName] VARCHAR(200),
		[PhoneNumber] BIGINT NULL,
		[CountryId] INT NOT NULL,
		[Region] VARCHAR(200),
		[City] VARCHAR(200),
		[Address] VARCHAR(200),
		[PostalCode] VARCHAR(200),
		
		FOREIGN KEY ([CountryId]) REFERENCES [Country] ([Id]),
	);
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
	CREATE TABLE [OrderItems] (
		[Id] INT PRIMARY KEY IDENTITY,
		[OrderId] INT NOT NULL,
		[ArtId] INT NOT NULL,

		FOREIGN KEY ([OrderId]) REFERENCES [Order] ([Id]),
		FOREIGN KEY ([ArtId]) REFERENCES [Art] ([Id])
	);
END
