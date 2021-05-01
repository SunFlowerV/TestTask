use [TestDatabase];
go
create function ReturnExchangeRate (@valuteName nvarchar(50), @date date)
	returns money
	begin
		declare @value money;
		select @value = [e].[Value]/[c].[Nominal]
		from [ExchangeRates] as e
		left join [Currencies] as c on [e].CurrenciesId = [c].[Id]
		where [c].Name = @valuteName and [e].[Date] = @date
		return @value;
	end;