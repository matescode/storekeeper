USE [master]
GO
CREATE LOGIN [StoreKeeperUser] WITH PASSWORD=N'Welcome1', DEFAULT_DATABASE=[StoreKeeper], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [StoreKeeper]
GO
CREATE USER [StoreKeeperUser] FOR LOGIN [StoreKeeperUser] WITH DEFAULT_SCHEMA=[dbo]
GO
USE [StoreKeeper]
GO
EXEC sp_addrolemember N'db_owner', N'StoreKeeperUser'
GO

