use [StoreKeeper]

begin tran

delete from ProductArticleReservations where ProductArticleItemId in (
	select Id from ProductArticleItems where ArticleId in (
		select Id from Articles where InternalStorage is null
	)
)

delete from ProductArticleItems where ProductArticleId in (
	select Id from ProductArticles where ArticleId in (
		select Id from Articles where InternalStorage is null
	)
)

delete from ProductArticleItems where ArticleId in (
	select Id from Articles where InternalStorage is null
)

delete from ProductArticles where ArticleId in (
	select Id from Articles where InternalStorage is null
)

delete from ArticleStats where ArticleId in (
	select Id from Articles where InternalStorage is null
)

delete from ExternStorageStats where ArticleId in (
	select Id from Articles where InternalStorage is null
)

delete from Articles where InternalStorage is null

commit
--rollback