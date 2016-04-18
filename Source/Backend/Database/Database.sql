--use master
--go
--create database StoreKeeper
--go

use StoreKeeper
go

create table Users
(
	Id uniqueidentifier not null,
	Name nvarchar(50) not null,
	SecurityToken nvarchar(50) not null
)

go

alter table Users add constraint PK1Users primary key (Id)

alter table Users add constraint UC1Users unique (Name)

go

create table ActiveSessions
(
	Id uniqueidentifier not null,
	SessionId nvarchar(100) not null,
	Created datetime not null,
	UserId uniqueidentifier not null,
	ClientComputer nvarchar(255) not null,
	Port int not null
)

go

alter table ActiveSessions add constraint PK1ActiveSessions primary key (Id)

alter table ActiveSessions add constraint UC1ActiveSessions unique (SessionId)

alter table ActiveSessions add constraint FK1ActiveSessions foreign key (UserId) references Users (Id)

go

create table SystemInformations
(
	Id uniqueidentifier not null,
	Name nvarchar(50) not null,
	Value nvarchar(200) not null
)

go

alter table SystemInformations add constraint PK1SystemInformations primary key (Id)

alter table SystemInformations add constraint UC1SystemInformations unique (Name)

go

create table ArticleDatas
(
	Id uniqueidentifier not null,
	ExternalId int not null,
	Name nvarchar(200) not null,
	[Type] int not null,
	Code nvarchar(100) not null,
	SpecialCode nvarchar(100),
	OriginalCount float not null,
	InternalStorage nvarchar(200) null,
	PurchasingPrice float not null,
	SellingPrice float not null,
	OrderName nvarchar(200),
	OrderCount float not null
)

go

alter table ArticleDatas add constraint PK1ArticleDatas primary key (Id)

go

create table ArticleItemDatas
(
	Id uniqueidentifier not null,
	ArticleDataId uniqueidentifier not null,
	ExternalId int not null,
	Code nvarchar(100) not null,
	InternalStorage nvarchar(200) not null,
	Quantity float not null,
	Updated bit not null,
	ProductArticleItemId uniqueidentifier null
)

go

alter table ArticleItemDatas add constraint PK1ArticleItemDatas primary key (Id)

alter table ArticleItemDatas add constraint FK1ArticleItemDatas foreign key (ArticleDataId) references ArticleDatas (Id)

go

create table ArticleCounts
(
	Id uniqueidentifier not null,
	Code nvarchar(100) not null,
	StorageId uniqueidentifier not null,
	OriginalCount float not null,
	OrderCount float not null
)

go

alter table ArticleCounts add constraint PK1ArticleCounts primary key (Id)

go

create table Storages
(
	Id uniqueidentifier not null,
	Name nvarchar(200) not null,
	Prefix nvarchar(200),
	IsExtern bit not null default 1,
	CompanyName nvarchar(200) not null,
	Street nvarchar(200),
	Number nvarchar(50) not null,
	ZipCode nvarchar(10) not null,
	City nvarchar(100) not null,
	CompanyId nvarchar(20),
	TaxId nvarchar(20)
)

go

alter table Storages add constraint PK1Storages primary key (Id)

alter table Storages add constraint UC1Storages unique (Name)

go

create table Articles
(
	Id uniqueidentifier not null,
	Name nvarchar(200) not null,
	[Type] int not null,
	Code nvarchar(100) not null,
	SpecialCode nvarchar(100),
	ExternStorageCount float not null,
	PurchasingPrice float not null,
	SellingPrice float not null,
	OrderName nvarchar(200),
	OrderCount float not null,
	Updated bit not null
)

go

alter table Articles add constraint PK1Articles primary key (Id)

create unique index U1Articles on Articles(Code asc)

go

create table ArticleOrders
(
	Id uniqueidentifier not null,
	ArticleId uniqueidentifier not null,
	[Count] float not null,
	UserId char(36)
)

go

alter table ArticleOrders add constraint PK1ArticleOrders primary key (Id)

alter table ArticleOrders add constraint FK1ArticleOrders foreign key (ArticleId) references Articles (Id)

go

create table ArticleStats
(
	Id uniqueidentifier not null,
	ArticleId uniqueidentifier not null,
	StorageId uniqueidentifier not null,
	CurrentCount float not null,
	MissingInOrders float not null,
	ProductCount int not null
)

go

alter table ArticleStats add constraint PK1ArticleStats primary key (Id)

alter table ArticleStats add constraint FK1ArticleStats foreign key (ArticleId) references Articles (Id)

alter table ArticleStats add constraint FK2ArticleStats foreign key (StorageId) references Storages (Id)

go

create table ProductArticles
(
	Id uniqueidentifier not null,
	ArticleId uniqueidentifier not null
)

go

alter table ProductArticles add constraint PK1ProductArticles primary key (Id)

alter table ProductArticles add constraint FK1ProductArticles foreign key (ArticleId) references Articles (Id)

go

create table ProductArticleItems
(
	Id uniqueidentifier not null,
	ProductArticleId uniqueidentifier not null,
	ArticleId uniqueidentifier not null,
	Quantity float not null,
	StorageId uniqueidentifier not null,
	SkipCalculation bit not null
)

go

alter table ProductArticleItems add constraint PK1ProductArticleItems primary key (Id)

alter table ProductArticleItems add constraint FK1ProductArticleItems foreign key (ProductArticleId) references ProductArticles (Id)

alter table ProductArticleItems add constraint FK2ProductArticleItems foreign key (ArticleId) references Articles (Id)

alter table ProductArticleItems add constraint FK3ProductArticleItems foreign key (StorageId) references Storages (Id)

go

create table ProductArticleOrders
(
	Id uniqueidentifier not null,
	ProductArticleId uniqueidentifier not null,
	[Count] float not null,
	[Priority] int not null,
	ProductionCount float not null,
	OrderPeriod datetime,
	PlannedPeriod datetime,
	EndPeriod datetime,
	UserId char(36)
)

go

alter table ProductArticleOrders add constraint PK1ProductArticleOrders primary key (Id)

alter table ProductArticleOrders add constraint FK1ProductArticleOrders foreign key (ProductArticleId) references ProductArticles (Id)

go

create table ProductArticleReservations
(
	Id uniqueidentifier not null,
	ProductArticleOrderId uniqueidentifier not null,
	ProductArticleItemId uniqueidentifier not null,
	CurrentCount float not null,
	ReservationCount float not null,
	OrderCount float not null,
	UserId char(36)
)

go

alter table ProductArticleReservations add constraint PK1ProductArticleReservations primary key (Id)

alter table ProductArticleReservations add constraint FK1ProductArticleReservations foreign key (ProductArticleOrderId) references ProductArticleOrders (Id)

alter table ProductArticleReservations add constraint FK2ProductArticleReservations foreign key (ProductArticleItemId) references ProductArticleItems (Id)

go

create table ProductOrderCompletions
(
	Id uniqueidentifier not null,
	ProductArticleOrderId uniqueidentifier not null,
	[Status] integer not null,
	UserId char(36)
)

go

alter table ProductOrderCompletions add constraint PK1ProductOrderCompletions primary key (Id)

go

create table MaterialCache
(
	UserId uniqueidentifier not null,
	ArticleId uniqueidentifier not null,
	StorageId uniqueidentifier not null,
	CurrentCount float not null
)

go

alter table MaterialCache add constraint PK1MaterialCache primary key(UserId, ArticleId, StorageId)

alter table MaterialCache add constraint FK1MaterialCache foreign key (UserId) references Users (Id)

alter table MaterialCache add constraint FK2MaterialCache foreign key (ArticleId) references Articles (Id)

alter table MaterialCache add constraint FK3MaterialCache foreign key (StorageId) references Storages (Id)

go

create table DataJournal
(
	Id uniqueidentifier not null,
	[User] nvarchar(50) not null,
	Code nvarchar(50) not null,
	StampTime datetime not null
)

go

alter table DataJournal add constraint PK1DataJournal primary key (Id)

go

-- indexes

create index I1ArticleStats on ArticleStats (ArticleId)
go

create index I2ArticleStats on ArticleStats (StorageId)
go

create index I1MaterialCache on MaterialCache (ArticleId)
go
create index I2MaterialCache on MaterialCache (StorageId)
go

create index I1ProductArticleItems on ProductArticleItems (ProductArticleId)
go

checkpoint

go

-- initial data

begin tran

insert into Users (Id, Name, SecurityToken) values ('F2024637-2251-479A-8ED9-940E4354F37B', 'Radek', 'EE05F575-6816-461D-8444-C1D9FBE8350E')

insert into Users (Id, Name, SecurityToken) values ('F9F5ABF2-18D7-4091-B9D8-74CBDD8674D5', 'Libor', '90D2103F-5958-450C-B248-B8C022E54A5B')


-- START 'SystemInformations' data

INSERT INTO SystemInformations (Id,Name,Value) VALUES ('46FE59B7-6836-4AD5-996B-121FDC105D7D','DN_Parlor','FLAJZAR, s.r.o.')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('71608427-8788-4C64-842F-35D7EBA82AF4','DN_Number','151')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('76295D69-6A53-4783-8815-B25BD8A51E47','DN_Phone','511191207')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('76847BB0-04CC-4D90-87D8-FF1F3FDF27BC','DN_Web','www.flajzar.cz')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('81CB3C22-5937-4794-9EF0-26B738DEEAD6','DN_Email','libor.kotasek@flajzar.cz')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('99DDF1A4-8F88-4793-8A5B-C9AB78E9B62F','DN_Zip','696 61')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('A41D5A81-E236-4BB2-9D8B-CFB8B2DDF4CC','DN_Street','Lidéřovice')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('D53EF18C-8D24-4783-84A2-F400B89A8C26','DN_City','Vnorovy')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('EE4A298A-3CAB-4940-A732-FB3000F2A5C6','DN_CellPhone','')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('8E160596-8062-4683-8555-D73F4C273EF7','LastUpdate','8. 2. 2016 12:46:12')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('046F5C44-B9C7-493F-88C6-F13E684B99EC','LastSave','5. 12. 2014 10:21:47')
INSERT INTO SystemInformations (Id,Name,Value) VALUES ('066576D1-D9A2-49AF-BCC2-AE762B2BA1A8','ResponsibleUser','Radek')

-- END 'SystemInformations' data

-- START 'Storages' data

INSERT INTO Storages (Id,Name,Prefix,IsExtern,CompanyName,Street,Number,ZipCode,City,CompanyId,TaxId) VALUES ('D67BFF48-E36F-4066-888B-23BA5E622FB0','Flajzar',NULL,0,'FLAJZAR, s.r.o.','Kasárna','500','69681','Bzenec','26916436','CZ26916436')
INSERT INTO Storages (Id,Name,Prefix,IsExtern,CompanyName,Street,Number,ZipCode,City,CompanyId,TaxId) VALUES ('A417F801-8B70-45E1-9E17-C5D3E00A18A4','Jablo PCB','SKLAD_K1',1,'JABLO PCB s.r.o.','U přehrady','5129/67','46601','Jablonec nad Nisou','27274705','CZ27274705')
INSERT INTO Storages (Id,Name,Prefix,IsExtern,CompanyName,Street,Number,ZipCode,City,CompanyId,TaxId) VALUES ('7DDD25C3-F931-4498-96A4-253ED148D522','Mesit přístroje','SKLAD_K2',1,'Mesit přístroje, spol. s r.o.','Sokolovská','573','68601','Uherské Hradiště 1','','')
INSERT INTO Storages (Id,Name,Prefix,IsExtern,CompanyName,Street,Number,ZipCode,City,CompanyId,TaxId) VALUES ('270D12FB-33B1-4F07-86A9-A7658E72D7C5','Integra','SKLAD_K4',1,'Integra VD Zlín','Bartošova','4341','76177','Zlín','00030970','CZ00030970')
INSERT INTO Storages (Id,Name,Prefix,IsExtern,CompanyName,Street,Number,ZipCode,City,CompanyId,TaxId) VALUES ('2C1EF512-D838-481C-BB16-EBED65795AC4','Lambert','SKLAD_K3',1,'Lambert','Lambert','Lambert','Lambert','Uherské Hradiště','111','111')

-- END 'Storages' data




--insert into Storages (Id, Name, IsExtern, CompanyName, Street, Number, ZipCode, City, CompanyId, TaxId) 
--values ('D67BFF48-E36F-4066-888B-23BA5E622FB0', 'Flajzar', 0, 'FLAJZAR, s.r.o.', 'Kasárna', '500', '69681', 'Bzenec', '26916436', 'CZ26916436')

--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'DN_Parlor', 'FLAJZAR, s.r.o.')
--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'DN_Number', '151')
--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'DN_Phone', '518 628 596')
--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'DN_Web', 'www.flajzar.cz')
--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'DN_Email', 'flajzar@flajzar.cz')
--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'DN_Zip', '696 61')
--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'DN_Street', 'Lidéřovice')
--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'DN_City', 'Vnorovy')
--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'DN_CellPhone', '776 586 866')
--insert into SystemInformations (Id, Name, Value) values (NEWID(), 'ResponsibleUser', 'SYSTEM')
insert into SystemInformations (Id, Name, Value) values (NEWID(), 'LockedBy', '')

commit

