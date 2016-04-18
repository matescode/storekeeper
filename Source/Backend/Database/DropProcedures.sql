use [StoreKeeper]

-----------------------------------

if exists (select * from sysobjects where id = object_id('MaxOf2'))
begin
    drop function MaxOf2
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('LogDataJournal'))
begin
    drop procedure LogDataJournal
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('LockDatabase'))
begin
    drop procedure LockDatabase
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('AssureLock'))
begin
    drop function AssureLock
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('ResetArticleStats'))
begin
    drop procedure ResetArticleStats
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('PublishData'))
begin
    drop procedure PublishData
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('RefreshAllPriorities'))
begin
    drop procedure RefreshAllPriorities
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('PrepareCalculation'))
begin
    drop procedure PrepareCalculation
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CalculateMaterialCache'))
begin
    drop procedure CalculateMaterialCache
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('GetMaxProductionCount'))
begin
    drop function GetMaxProductionCount
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CreateReservations'))
begin
    drop procedure CreateReservations
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CalculateMaterialOrderCounts'))
begin
    drop procedure CalculateMaterialOrderCounts
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CalculateData'))
begin
    drop procedure CalculateData
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('ResolveProductOrder'))
begin
    drop procedure ResolveProductOrder
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('TransferMaterial'))
begin
    drop procedure TransferMaterial
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('RemoveAllTransfers'))
begin
    drop procedure RemoveAllTransfers
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CreateSubOrder'))
begin
    drop procedure CreateSubOrder
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('DeleteLoss'))
begin
    drop procedure DeleteLoss
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('PrepareLoading'))
begin
    drop procedure PrepareLoading
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('ProcessLoadedArticleItems'))
begin
    drop procedure ProcessLoadedArticleItems
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('ProcessLoadedData'))
begin
    drop procedure ProcessLoadedData
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('GroupLoadedData'))
begin
    drop procedure GroupLoadedData
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('RemoveDuplicities'))
begin
    drop procedure RemoveDuplicities
end
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('PostLoadUpdate'))
begin
    drop procedure PostLoadUpdate
end
go
