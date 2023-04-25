using System;
using System.Collections.Generic;
using System.Linq;
using AssessmentTest02.Domain;
using AssessmentTest02.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssessmentTest02.DataAccess
{
    public static class EfMethods
    {
        #region Customer
        public static int CreateCustomer(string name)
        {
            using ScooterDbContext db = new();
            Customer customer = new() { Name = name };
            CreateCustomer(customer);
            return customer.Id;
        }
        public static int CreateCustomer(Customer customer)
        {
            using ScooterDbContext db = new();
            db.Customer.Add(customer);
            db.SaveChanges();
            return customer.Id;
        }
        public static List<string> ReadCustomerNames()
        {
            using ScooterDbContext db = new();
            List<string> customerNames = db.Customer
                                                   .Select(q => q.Name).ToList()!;
            return customerNames;
        }
        public static List<string> ReadCustomerNamesSorted()
        {
            using ScooterDbContext db = new();
            List<string> customerNames = db.Customer.OrderBy(q => q.Name)
                                                    .Select(q => q.Name).ToList()!;
            return customerNames;
        }

        public static bool UpdateCustomerName(int id, string name)
        {
            using ScooterDbContext db = new();
            Customer customer = db.Customer.FirstOrDefault(c => c.Id == id)!;
            if (customer == null)
            {
                return false;
            }
            customer.Name = name;
            db.Update(customer);
            db.SaveChanges();
            return true;
        }

        #endregion Customer

        #region Brand
        public static int CreateBrand(string name)
        {
            using ScooterDbContext db = new();
            Brand brand = new() { Name = name };
            db.Brand.Add(brand);
            db.SaveChanges();
            return brand.Id;
        }
        #endregion Brand

        #region Scooter
        public static int CreateScooter(int brandId, int elLeft)
        {
            using ScooterDbContext db = new();
            Brand brand = db.Brand.FirstOrDefault(b => b.Id == brandId)!;
            Scooter scooter = new() { Brand = brand, ElectricityLeft = elLeft };
            db.Scooter.Add(scooter);
            return db.SaveChanges();
            //return scooter.Id;

        }
        public static int CreateScooter(int brandId, int elLeft, int elTotal, MaintenanceState maintenanceState)
        {
            using ScooterDbContext db = new();
            Brand brand = db.Brand.FirstOrDefault(b => b.Id == brandId)!;
            Scooter scooter = new() { Brand = brand, ElectricityLeft = elLeft, ElectricityTotal = elTotal, MaintenanceState = maintenanceState };
            db.Scooter.Add(scooter);
            db.SaveChanges();
            return scooter.Id;
        }

        public static List<string> ReadScooterBrandNamesWithMinimumElectricityLeft(int minValue)
        {
            using ScooterDbContext db = new();
            List<string> scooters = db.Scooter.Where(s => s.ElectricityLeft >= minValue)
                                                    .Select(s => s.Brand.Name).ToList();
            return scooters;
        }

        public static Scooter? ReadScooterById(int scooterId)
        {
            using ScooterDbContext db = new();
            return db.Scooter.Include(s => s.Brand)
                             .Include(s => s.Rides)!
                             .ThenInclude(r => r.Customer)
                .FirstOrDefault(s => s.Id == scooterId);
        }


        public static bool UpdateScooterSetCurrentPosition(int id, double latitude, double longitude)
        {
            using ScooterDbContext db = new();
            Scooter? scooter = db.Scooter.FirstOrDefault(p => p.Id == id);
            if (scooter == null)
            {
                return false;
            }
            scooter.Latitude = latitude;
            scooter.Longitude = longitude;
            db.Update(scooter);
            db.SaveChanges();
            return true;
        }

        public static Scooter? ReadScooterByBrandName(string brand)
        {
            using ScooterDbContext db = new();
            return db.Scooter.Include(s => s.Brand)
                             .FirstOrDefault(s => s.Brand.Name == brand);
        }

        public static bool DeleteScooterIfTrashyBrand(string brand) 
        {
            using ScooterDbContext db = new();
            List<Scooter> scooters = db.Scooter.Include(s =>s.Brand)
                                               .Where(s => s.Brand.Name == brand).ToList();
            if (scooters.Count > 0)
            {
                foreach (Scooter scooter in scooters)
                {
                    db.Scooter.Remove(scooter);
                }
                db.SaveChanges();
                return true;
            }
            return false;
        }

        #endregion Scooter

        #region Ride
        public static int CreateRide(int id, Customer customer, int? fee, int? paid)
        {
            using ScooterDbContext db = new();
            Scooter scooter = db.Scooter.FirstOrDefault(s => s.Id == id)!;
            Ride ride = new() { Scooter = scooter, Customer = customer, Fee = fee, PaidAmount = paid };
            db.Ride.Add(ride);
            db.SaveChanges();
            return ride.Id;
        }

        public static List<Ride> ReadRides()
        {
            using ScooterDbContext db = new();
            List<Ride> rides = db.Ride.Include(r => r.Customer)
                                       .Include(r => r.Scooter)
                                       .Select(r => r).ToList()!;
            return rides;
        }

        #endregion Ride

        #region HelperMethods

        public static void RebuildDatabase()
		{
			using ScooterDbContext db = new();
			db.Database.EnsureDeleted();
			db.Database.EnsureCreated();
		}


		public static void ClearAllData()
		{
			using ScooterDbContext db = new();
			db.RemoveRange(db.Scooter);
			db.RemoveRange(db.Brand);
			db.RemoveRange(db.Ride);
			db.RemoveRange(db.Customer);

			db.ResetIdentityStartingValue("Scooter");
			db.ResetIdentityStartingValue("Brand");
			db.ResetIdentityStartingValue("Ride");
			db.ResetIdentityStartingValue("Customer");

			db.SaveChanges();
		}


        #endregion HelperMethods
    }
}
