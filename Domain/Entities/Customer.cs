using System.Collections.Generic;

namespace AssessmentTest02.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; } // PK
        public string Name { get; set; } = string.Empty;

		// Navigation Properties
		public ICollection<Ride>? Rides { get; set; }
    }
}
