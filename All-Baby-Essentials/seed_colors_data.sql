-- T-SQL Script: Seed Standard Colors and Product Color Variants
-- This script contains example data for KiddoMart / E-Commerce-All-Baby-Essentials database.

-- 1. Seed Global Standard Colors (if not exists)
SET IDENTITY_INSERT [Colors] ON;

IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 1)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (1, 'Red', '#FF0000');
IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 2)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (2, 'Blue', '#0000FF');
IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 3)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (3, 'Pink', '#FFC0CB');
IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 4)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (4, 'White', '#FFFFFF');
IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 5)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (5, 'Black', '#000000');
IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 6)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (6, 'Yellow', '#FFFF00');
IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 7)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (7, 'Green', '#008000');
IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 8)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (8, 'Purple', '#800080');
IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 9)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (9, 'Beige', '#F5F5DC');
IF NOT EXISTS (SELECT 1 FROM [Colors] WHERE [Id] = 10)
    INSERT INTO [Colors] ([Id], [Name], [HexCode]) VALUES (10, 'Gray', '#808080');

SET IDENTITY_INSERT [Colors] OFF;

-- 2. Seed Example Product Colors (Pivot Table) for an existing product (e.g., Product ID = 1)
-- Assuming Product ID = 1 exists in the database.
IF EXISTS (SELECT 1 FROM [Products] WHERE [Id] = 1)
BEGIN
    -- Associate Red (Color ID = 1), Stock = 25
    IF NOT EXISTS (SELECT 1 FROM [ProductColors] WHERE [ProductId] = 1 AND [ColorId] = 1)
        INSERT INTO [ProductColors] ([ProductId], [ColorId], [StockQuantity], [ImageUrl])
        VALUES (1, 1, 25, NULL);

    -- Associate Blue (Color ID = 2), Stock = 15
    IF NOT EXISTS (SELECT 1 FROM [ProductColors] WHERE [ProductId] = 1 AND [ColorId] = 2)
        INSERT INTO [ProductColors] ([ProductId], [ColorId], [StockQuantity], [ImageUrl])
        VALUES (1, 2, 15, NULL);

    -- Associate Pink (Color ID = 3), Stock = 40
    IF NOT EXISTS (SELECT 1 FROM [ProductColors] WHERE [ProductId] = 1 AND [ColorId] = 3)
        INSERT INTO [ProductColors] ([ProductId], [ColorId], [StockQuantity], [ImageUrl])
        VALUES (1, 3, 40, NULL);

    -- Automatically update Product Stock Quantity as sum of color variants (25 + 15 + 40 = 80)
    UPDATE [Products]
    SET [StockQuantity] = (SELECT SUM([StockQuantity]) FROM [ProductColors] WHERE [ProductId] = 1),
        [UpdatedAt] = GETDATE()
    WHERE [Id] = 1;
END
GO
