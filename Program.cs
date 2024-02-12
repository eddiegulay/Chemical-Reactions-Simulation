class Program
{
    static void Main()
    {
        try
        {
            TitrationSimulator titrationSimulator = new TitrationSimulator("Chemicals.json");
            titrationSimulator.SimulateTitration();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
