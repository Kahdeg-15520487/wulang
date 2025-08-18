using System;
using WuLangSpellcraft.Core;
using WuLangSpellcraft.Demo.Demonstrations;

namespace WuLangSpellcraft.Demo.Interactive
{
    /// <summary>
    /// Interactive artifact creation workshop
    /// </summary>
    public static class ArtifactCreationWorkshop
    {
        public static void Run()
        {
            Console.WriteLine("🏺 ARTIFACT CREATION WORKSHOP");
            Console.WriteLine(new string('-', 30));
            
            ShowArtifactTypes();
            var artifactChoice = GetArtifactTypeChoice();
            
            if (artifactChoice.HasValue)
            {
                var name = GetArtifactName();
                CreateAndDisplayArtifact(artifactChoice.Value, name);
            }
        }

        private static void ShowArtifactTypes()
        {
            Console.WriteLine("Artifact Types:");
            Console.WriteLine("  1. Fire Crucible (Forge + Fire)");
            Console.WriteLine("  2. Water Chalice (Forge + Water)");
            Console.WriteLine("  3. Earth Foundation (Forge + Earth)");
            Console.WriteLine("  4. Metal Conductor (Forge + Metal)");
            Console.WriteLine("  5. Wood Catalyst (Forge + Wood)");
        }

        private static int? GetArtifactTypeChoice()
        {
            Console.Write("Choose artifact type (1-5): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= 5)
            {
                return choice;
            }
            
            Console.WriteLine("❌ Invalid artifact type.");
            return null;
        }

        private static string GetArtifactName()
        {
            Console.Write("Enter artifact name: ");
            return Console.ReadLine() ?? "Custom Artifact";
        }

        private static void CreateAndDisplayArtifact(int choice, string name)
        {
            var (artifactType, element) = choice switch
            {
                1 => (ArtifactType.CrucibleOfPower, ElementType.Fire),
                2 => (ArtifactType.ChaliceOfFlow, ElementType.Water),
                3 => (ArtifactType.FoundationStone, ElementType.Earth),
                4 => (ArtifactType.ConductorsRing, ElementType.Metal),
                5 => (ArtifactType.LivingCatalyst, ElementType.Wood),
                _ => (ArtifactType.CrucibleOfPower, ElementType.Fire)
            };
            
            var artifact = new ElementalArtifact(artifactType, ElementType.Forge, element, name);
            
            Console.WriteLine();
            Console.WriteLine("✅ Artifact created successfully!");
            ArtifactSystemDemo.DisplayArtifact($"🏺 {name}", artifact);
        }
    }
}
