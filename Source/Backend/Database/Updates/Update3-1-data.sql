use [StoreKeeper]

begin tran

---------------------
----- duplicity -----
---------------------

declare dupItemsCur cursor for
select t.AId, t.BId, t.BOrigCount, t.BCurrentCount, t.BExtCount from (
	select 
		a.Id as AId, 
		a.ExternalId as AExtId, 
		a.Code, 
		b.Id as BId, 
		b.ExternalId as BExtId, 
		b.OriginalCount as BOrigCount,
		bss.CurrentCount as BCurrentCount,
		bss.ExternStorageCount as BExtCount
	from 
		Articles a 
		join Articles b on (a.Code = b.Code and ((a.InternalStorage is null and a.ExternalId < b.ExternalId) or (a.InternalStorage like 'V�roba%' and b.InternalStorage like 'Materi�l%')))
		join ArticleStats bss on (bss.ArticleId = b.Id)
) t

declare @targetId uniqueidentifier
declare @id uniqueidentifier
declare @origCount float
declare @curCount float
declare @extCount float

open dupItemsCur

fetch next from dupItemsCur into @targetId, @id, @origCount, @curCount, @extCount

while @@FETCH_STATUS <> -1
begin
	if @@FETCH_STATUS <> -2
	begin
		update Articles set OriginalCount = OriginalCount + @origCount where Id = @targetId
		update ArticleStats set CurrentCount = CurrentCount + @curCount, ExternStorageCount = ExternStorageCount + @extCount where ArticleId = @targetId

		delete from ProductArticleReservations where ProductArticleItemId in (select Id from ProductArticleItems where ProductArticleId = (select Id from ProductArticles where ArticleId = @id))
		delete from ProductArticleItems where ProductArticleId = (select Id from ProductArticles where ArticleId = @id)
		delete from ArticleOrders where ArticleId = @id
		delete from ProductArticleReservations where ProductArticleItemId in (select Id from ProductArticleItems where ArticleId = @id)
		delete from ProductArticleItems where ArticleId = @id
		delete from ProductArticles where ArticleId = @id
		delete from ArticleStats where ArticleId = @id
		delete from Articles where Id = @id
	end
	fetch next from dupItemsCur into @targetId, @id, @origCount, @curCount, @extCount
end

close dupItemsCur
deallocate dupItemsCur

--------------------------------------
----- reset extern storage stats -----
--------------------------------------

declare articleCur cursor for
select 
	a.Id
from
	Articles a
	join ArticleStats ass on (ass.ArticleId = a.Id)
	join (
		select ArticleId, SUM(ProductCount) as Suma from ExternStorageStats group by ArticleId
	) t on (t.ArticleId = a.Id)
where
	ass.ExternStorageCount != t.Suma

declare @artId uniqueidentifier

open articleCur

fetch next from articleCur into @artId

while @@FETCH_STATUS <> -1
begin
	if @@FETCH_STATUS <> -2
	begin
		exec ResetArticleStats @artId, null
	end
	fetch next from articleCur into @artId
end

close articleCur
deallocate articleCur

commit