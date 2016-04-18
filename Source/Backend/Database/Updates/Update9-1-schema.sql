use [StoreKeeper]

-----------------------------------

--alter table Storages add Prefix nvarchar(200)

begin transaction

update Storages set Prefix = 'SKLAD_K1' where Id = 'A417F801-8B70-45E1-9E17-C5D3E00A18A4'
update Storages set Prefix = 'SKLAD_K2' where Id = '7DDD25C3-F931-4498-96A4-253ED148D522'
update Storages set Prefix = 'SKLAD_K3' where Id = '2C1EF512-D838-481C-BB16-EBED65795AC4'
update Storages set Prefix = 'SKLAD_K4' where Id = '270D12FB-33B1-4F07-86A9-A7658E72D7C5'

delete from ExternStorageStats
delete from ArticleStats
delete from ArticleOrders
delete from ProductOrderCompletions
delete from ProductArticleReservations
delete from ProductArticleOrders
delete from ProductArticleItems
delete from ProductArticles
delete from Articles
delete from ArticleCounts

--rollback
commit

alter table ArticleCounts drop column ExternalId

alter table ArticleItemDatas add Code nvarchar(100) not null
alter table ArticleItemDatas add InternalStorage nvarchar(200) not null

drop table ExternStorageStats
drop table ArticleTransfers
drop table ArticleTransferIdentifiers
drop table ProductArticleOrderHistories

alter table ArticleStats drop column ExternStorageCount
alter table ArticleStats drop column UserId

alter table ArticleStats add StorageId uniqueidentifier not null

alter table ArticleStats add constraint FK2ArticleStats foreign key (StorageId) references Storages (Id)

create index I2ArticleStats on ArticleStats (StorageId)

drop index IDX1Articles on Articles
alter table Articles drop column ExternalId
alter table Articles drop column InternalStorage
alter table Articles drop column OriginalCount

alter table Articles add ExternStorageCount float not null

alter table Articles add constraint FK1Articles foreign key (StorageId) references Storages (Id)

create unique index U1Articles on Articles(Code asc)


-- upravit nacitani - rozlisovat podle skladu
-- zkontrolovat vypocet
-- jaky ma smysl vyreseni objednavky

-- OSAZENI RNE4DRX - duplicitni kod, ruzne nazvy v ruznych skladech
-- F-SCK duplicitni vyrobek?

--select * from Articles where Code = 'LED0603 Y'

--select * from ProductArticles where ArticleId = '2B8E12AE-8BAB-4A3E-B1BB-B32B9CE353BB'
--select * from ProductArticleItems where ProductArticleId = 'A7F1D4C4-5272-4A04-B13B-A2E0ED4EA745'