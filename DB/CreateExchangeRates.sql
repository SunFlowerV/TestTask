use [TestDatabase];
create table [ExchangeRates]
(
[Id] int identity not null,
[CurrenciesId] nvarchar(10) not null,
[Value] money not null,
[Date] date not null,
constraint [PK_ExchangeRates] primary key clustered ([Id]),
constraint [FK_ExchangeRates_Currencies_CurrenciesId] foreign key ([CurrenciesId]) references [Currencies] ([Id]) on delete cascade
)