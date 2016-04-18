use [StoreKeeper]

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
	Quantity float not null,
	Updated bit not null,
	ProductArticleItemId uniqueidentifier null
)

go

alter table ArticleItemDatas add constraint PK1ArticleItemDatas primary key (Id)

alter table ArticleItemDatas add constraint FK1ArticleItemDatas foreign key (ArticleDataId) references ArticleDatas (Id)

