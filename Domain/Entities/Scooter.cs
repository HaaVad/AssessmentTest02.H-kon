using System.Collections.Generic;

namespace AssessmentTest02.Domain.Entities
{
    public class Scooter
    {
        public int Id { get; set; } // PK
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int ElectricityTotal { get; set; }
        public int ElectricityLeft { get; set; }
		public MaintenanceState MaintenanceState { get; set; }
		public int BrandId { get; set; } // FK

        // Navigation Properties
		public Brand? Brand { get; set; }
        public ICollection<Ride>? Rides { get; set; }
    }
}
