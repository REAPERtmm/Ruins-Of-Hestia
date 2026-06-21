using System;

namespace Village.BuildingManagement.House
{
    public class House : BuildingScript
    {
        public override BuildingProduction[] GetCurrentProduction()
        {
            return Array.Empty<BuildingProduction>();
        }
    }
}