-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'aresu_txt_editor')
    BEGIN
        CREATE DATABASE [aresu_txt_editor];
    END
GO

-- Create a SQL Server login if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'txt_editor_login')
    BEGIN
        CREATE LOGIN [txt_editor_login] WITH PASSWORD = 'fancy_password';
    END
GO

-- Switch to your database context
USE [aresu_txt_editor];
GO

-- Create a database user for the login if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = 'txt_editor')
    BEGIN
        CREATE USER [txt_editor] FOR LOGIN [txt_editor_login];
    END
GO

-- Grant CRUD permissions
ALTER ROLE db_datareader ADD MEMBER [txt_editor];
ALTER ROLE db_datawriter ADD MEMBER [txt_editor];
ALTER ROLE db_ddladmin ADD MEMBER [txt_editor];
GO