use [Sample Task];
go

set nocount on;

truncate table dbo.Source;

declare 
  @i int = 1,
  @recordCount int = 100000;

while @i<=@recordCount
begin
  insert into dbo.Source
  (
    FirstName,
    LastName
  )
  values
  (
    'First Name ' + cast(@i as varchar),
    'Last Name ' + cast(@i as varchar)
  );
  set @i += 1;
end;
go

