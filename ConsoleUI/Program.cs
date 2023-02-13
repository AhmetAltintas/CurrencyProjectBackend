using Business.Concrete;

static class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter a date: ");
        Console.Write("(YYYY,aa,gg)");
        DateTime date = DateTime.Parse(Console.ReadLine());

        DateTime inputtedDate = new DateTime(date.Year,date.Month,date.Day);


        ExchRateManager service = new ExchRateManager(inputtedDate);
        service.LoadExchRate();
        Console.WriteLine("Veriler veritabanına eklendi");
        Console.ReadLine();

        //Console.WriteLine("İstenilen tarih: " + service.CurrencyDate.Date);
        //Console.WriteLine("");
        //Console.WriteLine("Alınan tarih: " + service.ActualCurrencyDate.Date);
        //Console.WriteLine("");
        //Console.WriteLine("Api linki: " + service.ApiUrl);
        //Console.WriteLine("");

        //// USD
        //Console.WriteLine("USD - Döviz Alış: " + service.GetExchRate("USD", ExchRateType.ForexBuying).ToString());
        //Console.WriteLine("USD - Döviz Satış: " + service.GetExchRate("USD", ExchRateType.ForexSelling).ToString());
        //Console.WriteLine("USD - Efektif Alış: " + service.GetExchRate("USD", ExchRateType.BanknoteBuying).ToString());
        //Console.WriteLine("USD - Efektif Satış: " + service.GetExchRate("USD", ExchRateType.BanknoteSelling).ToString());
        //Console.WriteLine("");

        //// diğer para birimleri
        //Console.WriteLine("EUR - Döviz Alış: " + service.GetExchRate("EUR", ExchRateType.ForexBuying).ToString());
        //Console.WriteLine("GBP - Döviz Alış: " + service.GetExchRate("GBP", ExchRateType.ForexBuying).ToString());
        //Console.WriteLine("CAD - Döviz Alış: " + service.GetExchRate("CAD", ExchRateType.ForexBuying).ToString());

        Console.ReadLine();
    }
}