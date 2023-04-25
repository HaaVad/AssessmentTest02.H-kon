namespace AssessmentTest02.Domain.Entities
{
    public class Ride
    {
        public int Id { get; set; } // PK
        public decimal? Fee { get; set; }
        public decimal? PaidAmount { get; set; }

        public int CustomerId { get; set; } // FK
        public int ScooterId { get; set; } // FK
		
        // Navigation Peroperties
        public Customer? Customer { get; set; }
		public Scooter? Scooter { get; set; }
    }
}
