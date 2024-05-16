using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Chemical;

class Program
{

    public class TitrationSimulator
    {
        public class Chemical
        {
            public string Name { get; set; }
            public string Symbol { get; set; }
            public double InitialConcentration { get; set; }
            public double pH { get; set; }
            public string Color { get; set; }
        }

        private List<Chemical> chemicals;
        private double simulationIncrement = 1.0;

        public TitrationSimulator(string jsonFilePath)
        {
            LoadChemicals(jsonFilePath);
        }

        private void LoadChemicals(string jsonFilePath)
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                chemicals = JsonSerializer.Deserialize<List<Chemical>>(jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chemicals: {ex.Message}");
                chemicals = new List<Chemical>(); // Initialize an empty list
            }
        }

        public void SetSimulationIncrement(double increment)
        {
            simulationIncrement = increment;
        }

        public void SimulateTitration()
        {
            if (chemicals == null || chemicals.Count < 2)
            {
                Console.WriteLine("Error: Insufficient chemicals for simulation.");
                return;
            }

            Chemical acid = chemicals.Find(c => c.Name.Equals("Acid"));
            Chemical baseChem = chemicals.Find(c => c.Name.Equals("Base"));

            Console.WriteLine("Simulation started...\n");

            while (true)
            {
                double addedVolume = Math.Min(simulationIncrement, acid.InitialConcentration, baseChem.InitialConcentration);

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
            // Check if either solution is exhausted
            if (acid.InitialConcentration <= 0 || baseChem.InitialConcentration <= 0)
            {
                return true; // Endpoint reached if either solution is exhausted
            }

            // Calculate the absolute difference in pH between the acid and base
            double deltaPH = Math.Abs(acid.pH - baseChem.pH);

            // Check for a significant change in pH
            if (deltaPH < 0.1)
            {
                // pH change is small, indicating the reaction is not complete
                return false; // Endpoint not reached
            }

            // Calculate the rate of change of pH
            double deltaPHRate = deltaPH / simulationIncrement; // Adjust based on simulation increment

            // Check if the rate of pH change is slow
            if (deltaPHRate < 0.05)
            {
                // Rate of pH change is slow, suggesting the reaction is nearing completion
                return false; // Endpoint not reached
            }

            // Check if the acid or base concentration is very close to zero
            if (acid.InitialConcentration < 0.01 || baseChem.InitialConcentration < 0.01)
            {
                // Concentration is very low, indicating the reaction is likely complete
                return true; // Endpoint reached
            }

            // Check if the pH of either solution is close to a known endpoint pH value
            double acidEndpointPH = 7.0; // Example endpoint pH for acid
            double baseEndpointPH = 7.0; // Example endpoint pH for base
            double pHThreshold = 0.1; // pH threshold for endpoint detection

            if (Math.Abs(acid.pH - acidEndpointPH) < pHThreshold || Math.Abs(baseChem.pH - baseEndpointPH) < pHThreshold)
            {
                // pH of one of the solutions is close to the expected endpoint pH
                return true; // Endpoint reached
            }

            // If none of the conditions above are met, consider the endpoint not reached
            return false;
        }

    }

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
