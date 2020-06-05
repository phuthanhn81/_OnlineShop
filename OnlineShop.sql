restore database OnlineShop from disk = 'D:\OnlineShop.bak' With NoRecovery

RESTORE DATABASE OnlineShop 
FROM  DISK = N'D:\OnlineShop.bak'
WITH
MOVE N'OnlineShop' TO N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\OnlineShop.mdf',  
MOVE N'OnlineShop_Log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\OnlineShop_log.ldf',
REPLACE,  STATS = 10
GO

--C:\Program Files\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA
