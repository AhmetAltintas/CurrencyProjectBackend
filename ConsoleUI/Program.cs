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

        Console.ReadLine();
    }
}