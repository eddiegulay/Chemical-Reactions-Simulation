using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class TitrationSimulator
{
    private List<Chemical> chemicals;

    public TitrationSimulator(string jsonFilePath)
    {
        string jsonContent = File.ReadAllText(jsonFilePath);
        chemicals = JsonSerializer.Deserialize<List<Chemical>>(jsonContent);
    }

    public void SimulateTitration()
    {
        Chemical acid = chemicals.Find(c => c.Name.Equals("Acid"));
        Chemical baseChem = chemicals.Find(c => c.Name.Equals("Base"));

        Console.WriteLine("Simulation started...\n");

        while (true)
        {
            double addedVolume = Math.Min(1.0, acid.InitialConcentration, baseChem.InitialConcentration);

            acid.InitialConcentration -= addedVolume;
            baseChem.InitialConcentration -= addedVolume;

            Console.WriteLine($"Acid Concentration: {acid.InitialConcentration:F2} mol/L | " +
                              $"Base Concentration: {baseChem.InitialConcentration:F2} mol/L | " +
                              $"pH: {acid.pH:F2} | Color: {acid.Color}");

            if (IsEndpointReached(acid, baseChem))
            {
                Console.WriteLine("Endpoint reached. Titration complete.");
                break;
            }
        }
    }

    private bool IsEndpointReached(Chemical acid, Chemical baseChem)
    {
        // Consider the reaction complete if either solution is exhausted
        if (acid.InitialConcentration <= 0 || baseChem.InitialConcentration <= 0)
        {
            return true;
        }

        // Add more sophisticated endpoint detection criteria here
        double deltaPH = Math.Abs(acid.pH - baseChem.pH);

        // Check for a significant change in pH
        if (deltaPH < 0.1)
        {
            // pH change is small, indicating the reaction is not complete
            return false;
        }

        // Check for the rate of change of pH
        double deltaPHRate = Math.Abs(acid.pH - baseChem.pH) / addedVolume; // Adjust based on your simulation increments
        if (deltaPHRate < 0.05)
        {
            // Rate of pH change is slow, suggesting the reaction is nearing completion
            return false;
        }

        // If none of the conditions above are met, consider the endpoint not reached
        return false;
    }

}
