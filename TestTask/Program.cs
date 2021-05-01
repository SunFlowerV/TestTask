using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestTask
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();

            IJobDetail job = JobBuilder.Create<ExchangeRatesJob>()
                .WithIdentity("job1", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
            Console.ReadKey();


        }
        public class ExchangeRatesJob : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                XDocument xml = XDocument.Load("http://www.cbr.ru/scripts/XML_daily.asp");
                ExchangeRates exchangeRatesUSD = new ExchangeRates { Id = "R01235", Name = "USD" };
                ExchangeRates exchangeRatesEUR = new ExchangeRates { Id = "R01239", Name = "EUR" };
                ExchangeRates exchangeRatesCNY = new ExchangeRates { Id = "R01375", Name = "CNY" };
                ExchangeRates exchangeRatesJPY = new ExchangeRates { Id = "R01820", Name = "JPY" };
                exchangeRatesUSD.Value = Convert.ToDecimal(xml.Elements("ValCurs").Elements("Valute").FirstOrDefault(x => x.Element("NumCode").Value == "840").Elements("Value").FirstOrDefault().Value);
                exchangeRatesEUR.Value = Convert.ToDecimal(xml.Elements("ValCurs").Elements("Valute").FirstOrDefault(x => x.Element("NumCode").Value == "978").Elements("Value").FirstOrDefault().Value);
                exchangeRatesCNY.Value = Convert.ToDecimal(xml.Elements("ValCurs").Elements("Valute").FirstOrDefault(x => x.Element("NumCode").Value == "156").Elements("Value").FirstOrDefault().Value);
                exchangeRatesJPY.Value = Convert.ToDecimal(xml.Elements("ValCurs").Elements("Valute").FirstOrDefault(x => x.Element("NumCode").Value == "392").Elements("Value").FirstOrDefault().Value);
                List<ExchangeRates> exchangeRates = new List<ExchangeRates> { exchangeRatesUSD, exchangeRatesEUR, exchangeRatesCNY, exchangeRatesJPY };
                string connectionString = @"Server=.\SQLEXPRESS;Database=TestDataBase;Trusted_Connection=True;MultipleActiveResultSets=true";
                string commandread = "insert [ExchangeRates] ([CurrenciesId], [Value], [Date]) values (@currenciesid, @value, @date)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    foreach (var exRate in exchangeRates)
                    {
                        SqlCommand command = new SqlCommand(commandread, connection);
                        SqlParameter currenciesidParameter = new SqlParameter("@currenciesid", exRate.Id);
                        SqlParameter valueParameter = new SqlParameter("@value", exRate.Value);
                        SqlParameter dateParameter = new SqlParameter("@date", DateTime.Now);
                        command.Parameters.Add(currenciesidParameter);
                        command.Parameters.Add(valueParameter);
                        command.Parameters.Add(dateParameter);
                        int number = await command.ExecuteNonQueryAsync();
                    }
                }
                Console.WriteLine($"Добавлен курс на дату: {DateTime.Now}");
            }
        }

        public class ExchangeRates
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public decimal Value { get; set; }


        }
    }
}
