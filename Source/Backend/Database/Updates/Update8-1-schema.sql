use [StoreKeeper]

-----------------------------------

create table ArticleCounts
(
	Id uniqueidentifier not null,
	Code nvarchar(100) not null,
	ExternalId int not null,
	InternalStorage nvarchar(200) null,
	OriginalCount float not null,
	OrderCount float not null
)

go

alter table ArticleCounts add constraint PK1ArticleCounts primary key (Id)

go
