use [TestDatabase];
create table [Currencies]
(
[Id] nvarchar(10) not null,
[Name] nvarchar(70) not null,
[EngName] nvarchar(70),
[Nominal] int not null,
[ISO_Num_Code] int,
[ISO_Char_Code] nvarchar(4),
constraint [PK_Ñurrencies] primary key clustered ([Id])
)