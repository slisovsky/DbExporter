use [Sample Task];
go

if object_id(N'dbo.Source', N'U') is null
begin
  create table dbo.Source
  (
    ID int not null identity(1, 1) primary key,
    FirstName varchar(50) null,
    LastName varchar(50) null    
  );
end;
go

