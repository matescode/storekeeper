use [StoreKeeper]

-----------------------------------

if exists (select * from sysobjects where id = object_id('MaxOf2'))
begin
    drop function MaxOf2
end
go

create function MaxOf2
(
	@val1 float,
	@val2 float
)
returns float
as
begin
	if @val1 > @val2
	begin
		return @val1
	end
	return @val2
end
go

grant exec on MaxOf2 to public
go
print 'SF: MaxOf2 added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('LogDataJournal'))
begin
    drop procedure LogDataJournal
end
go

create procedure LogDataJournal
(
	@UserId char(36),
	@Code nvarchar(50)
)
as
	set nocount on

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @username nvarchar(50)
	select @username = Name from Users where Id = @UserId

	insert into DataJournal (Id, [User], Code, StampTime) values (NEWID(), @username, @Code, GETDATE())

	delete from DataJournal where DATEDIFF(day, StampTime, GETDATE()) >= 30

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on LogDataJournal to public
go
print 'SP: LogDataJournal added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('LockDatabase'))
begin
    drop procedure LockDatabase
end
go

create procedure LockDatabase
(
	@UserId char(36),
	@Unlock bit
)
as
	set nocount on

	declare @usId nvarchar(200)
	select @usId = Value from SystemInformations where Name = 'LockedBy'

	declare @lockedBy nvarchar(50)
	set @lockedBy = ''

	declare @username nvarchar(50)
	select @username = Name from Users where Id = @UserId

	if @usId is not null and @usId != ''
	begin
		select @lockedBy = Name from Users where Id = @usId
	end

	declare @err nvarchar(500)
	set @err = ''

	if @Unlock = 1
	begin
		if @usId is null or @usId = ''
		begin
			raiserror ('Database is not locked!', 16, 1)
			return
		end

		if @usId is not null and @usId != '' and @usId != @UserId
		begin
			set @err = 'Database is not locked by ' + @username + '!'
			raiserror (@err, 16, 1)
			return
		end
	end
	else
	begin
		if @usId is not null and @usId != ''
		begin
			if @usId != @UserId
			begin
				set @err = 'Database is already locked by ' + @lockedBy + '!'
				raiserror (@err, 16, 1)
				return
			end
			else
			begin
				return
			end
		end
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @operation nvarchar(20)
	if @Unlock = 1
	begin
		set @operation = 'ReleaseLock'
		update SystemInformations set Value = '' where Name = 'LockedBy'
	end
	else
	begin
		set @operation = 'GetLock'
		update SystemInformations set Value = @UserId where Name = 'LockedBy'
	end

	exec LogDataJournal @UserId, @operation

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on LockDatabase to public
go
print 'SP: LockDatabase added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('AssureLock'))
begin
    drop function AssureLock
end
go

create function AssureLock()
returns bit
as
begin

	declare @usId nvarchar(200)
	select @usId = Value from SystemInformations where Name = 'LockedBy'

	if @usId is not null and @usId != ''
	begin
		return 1
	end

	return 0
end
go

grant exec on AssureLock to public
go
print 'SF: AssureLock added'
go

