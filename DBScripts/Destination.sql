use [Sample Task];
go

if object_id(N'dbo.Destination', 'U') is null
begin
  create table dbo.Destination
  (
    ID int not null primary key,
    FirstName varchar(50) null,
    LastName varchar(50) null     );
end;
go
