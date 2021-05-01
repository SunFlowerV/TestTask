using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FunctionReturnExchageRate
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Function retExRate = new Function();
            Console.WriteLine("Введите название валюты.");
            string valuteName = Console.ReadLine();
            Console.WriteLine("Введите дату курса в числовой форме.");
            Console.WriteLine("Год:");
            int Y;
            bool fY = int.TryParse(Console.ReadLine(), out Y);
            Console.WriteLine("Месяц:");
            int M;
            bool fM = int.TryParse(Console.ReadLine(), out M);
            Console.WriteLine("Число:");
            int D;
            bool fD = int.TryParse(Console.ReadLine(), out D);
            DateTime date = new DateTime(Y, M, D);
            decimal erRate = await retExRate.ReturnExchageRate(valuteName, date);
            Console.WriteLine($"{erRate}");
        }
    }
    public class Function
    {
        public async Task<decimal> ReturnExchageRate(string ValuteName, DateTime date)
        {
            decimal exchageRate = 0;
            string connectionString = @"Server=.\SQLEXPRESS;Database=TestDataBase;Trusted_Connection=True;MultipleActiveResultSets=true";
            string commandread = "select [e].[Value]/[c].[Nominal] from [ExchangeRates] as e left join[Currencies] as c on[e].CurrenciesId = [c].[Id] where[c].Name = @valuteName and[e].[Date] = @date";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(commandread, connection);
                SqlParameter valuteNameParameter = new SqlParameter("@valuteName", ValuteName);
                SqlParameter dateParameter = new SqlParameter("@date", date);
                command.Parameters.Add(valuteNameParameter);
                command.Parameters.Add(dateParameter);
                
                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync()) // построчно считываем данные
                        {
                            exchageRate = reader.GetDecimal(0);
                        }
                    }
                }
            }
            return exchageRate;
        }
    }
}
