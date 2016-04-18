use [StoreKeeper]

delete from ArticleItemDatas
delete from ArticleDatas

insert into ArticleDatas (Id, ExternalId, Name, Type, Code, SpecialCode, OriginalCount, InternalStorage, PurchasingPrice, SellingPrice, OrderName, OrderCount)
select 
	NEWID(),
	ExternalId,
	Name,
	Type,
	Code,
	SpecialCode,
	OriginalCount,
	InternalStorage,
	PurchasingPrice,
	SellingPrice,
	OrderName,
	OrderCount
from Articles
--where Updated = 1

insert into ArticleItemDatas (Id, ArticleDataId, ExternalId, Quantity, Updated)
select 
	NEWID(),
	ad.Id,
	i.ExternalId,
	pai.Quantity, 
	0
from 
	ProductArticleItems pai
	join Articles i on i.Id = pai.ArticleId
	join ProductArticles pa on (pa.Id = pai.ProductArticleId)
	join Articles a on (a.Id = pa.ArticleId)
	join ArticleDatas ad on (ad.ExternalId = a.ExternalId)