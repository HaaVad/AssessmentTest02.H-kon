using System.Collections.Generic;
using System.Linq;
using AssessmentTest02.DataAccess;
using AssessmentTest02.Domain;
using AssessmentTest02.Domain.Entities;
using NUnit.Framework;

namespace NUnitTest
{
    public class UnitTest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            EfMethods.RebuildDatabase();
        }


        [SetUp]
        public void Setup()
        {
            EfMethods.ClearAllData();
        }


        [Test]
        public void Part1_Test1_CreateThenReadCustomers()
        {
            EfMethods.CreateCustomer("Justine");
            List<string> result = EfMethods.ReadCustomerNames();
            CollectionAssert.AreEqual(new List<string>() { "Justine" }, result);

            EfMethods.CreateCustomer("Agnes");
            List<string> result2 = EfMethods.ReadCustomerNames();
            CollectionAssert.AreEqual(new List<string>() { "Justine", "Agnes" }, result2);
        }


        [Test]
        public void Part1_Test2_CreateThenReadCustomersSorted()
        {
            EfMethods.CreateCustomer("Justine");
            EfMethods.CreateCustomer("Agnes");
            EfMethods.CreateCustomer("Emma");

            List<string> result = EfMethods.ReadCustomerNamesSorted();
            CollectionAssert.AreEqual(new List<string>() { "Agnes", "Emma", "Justine" }, result);
        }


        [Test]
        public void Part1_Test3_CreateBrandsAndScootersThenReadByElectricityLeft()
        {
            int glionId = EfMethods.CreateBrand("Glion");
            int xiaomiId = EfMethods.CreateBrand("Xiaomi");
            int ninebotId = EfMethods.CreateBrand("Ninebot");
            EfMethods.CreateScooter(glionId, 500); // Electricity left: 500
            EfMethods.CreateScooter(xiaomiId, 1200); // Electricity left: 1200
            EfMethods.CreateScooter(ninebotId, 1700); // Electricity left: 1700

            List<string> result = EfMethods.ReadScooterBrandNamesWithMinimumElectricityLeft(1200);

            CollectionAssert.AreEqual(new List<string>() { "Xiaomi", "Ninebot" }, result);
        }


        [Test]
        public void Part2_Test4_CreateThenReadRide()
        {
            int brandId = EfMethods.CreateBrand("Xiaomi");
            int scooterId = EfMethods.CreateScooter(brandId, 1000);
            EfMethods.CreateRide(
                scooterId,
                new Customer() { Name = "Kari" },
                27, // Fee
                27 // Paid amount
            );

            List<Ride> rides = EfMethods.ReadRides();

            Assert.Multiple(() =>
            {
                Assert.That(rides[0].Customer?.Name, Is.EqualTo("Kari"));
                Assert.That(rides[0].Fee, Is.EqualTo(27));
                Assert.That(rides[0].PaidAmount, Is.EqualTo(27));
            });
        }


        [Test]
        public void Part2_Test5_CreateThenReadScooterByBrandName()
        {
            int kabooId = EfMethods.CreateBrand("Kaabo");
            EfMethods.CreateScooter(kabooId, 100);
            int eWheelsId = EfMethods.CreateBrand("E-wheels");
            int scooterId = EfMethods.CreateScooter(
                eWheelsId,
                1400, // Electricity left
                2000, // Electricity total
                MaintenanceState.NeedMaintenance);
            EfMethods.UpdateScooterSetCurrentPosition(
                scooterId,
                60.1, // Latitude
                14.0); // Longitude

            Scooter? scooter = EfMethods.ReadScooterByBrandName("E-wheels");

            Assert.Multiple(() =>
            {
                Assert.That(scooter?.ElectricityLeft, Is.EqualTo(1400));
                Assert.That(scooter?.ElectricityTotal, Is.EqualTo(2000));
                Assert.That(scooter?.MaintenanceState, Is.EqualTo(MaintenanceState.NeedMaintenance));
                Assert.That(scooter?.Latitude, Is.EqualTo(60.1));
                Assert.That(scooter?.Longitude, Is.EqualTo(14.0));
            });
        }


        [Test]
        public void Part2_Test6_CreateBrandAndScooterWithTwoRidesThenReadEverything()
        {
            int brandId = EfMethods.CreateBrand("Ninebot");
            int scooterId = EfMethods.CreateScooter(brandId, 500, 2000, MaintenanceState.NeedMaintenance);
            EfMethods.UpdateScooterSetCurrentPosition(scooterId, 57.7, 11.9);
            Customer alma = new() { Name = "Alma" };
            Customer oliver = new() { Name = "Oliver" };
            EfMethods.CreateRide(scooterId, alma, 16, 16);
            EfMethods.CreateRide(scooterId, oliver, null, null);

            Scooter? scooter = EfMethods.ReadScooterById(scooterId);

            Assert.Multiple(() =>
            {
                Assert.That(scooter?.Brand?.Name, Is.EqualTo("Ninebot"));
                Assert.That(scooter?.ElectricityLeft, Is.EqualTo(500));
                Assert.That(scooter?.MaintenanceState, Is.EqualTo(MaintenanceState.NeedMaintenance));
                Assert.That(scooter?.Latitude, Is.EqualTo(57.7));
                Assert.That(scooter?.Longitude, Is.EqualTo(11.9));
                Assert.That(scooter?.Rides?.Count, Is.EqualTo(2));
                Assert.That(scooter?.Rides?.ElementAt(0).Customer?.Name, Is.EqualTo("Alma"));
                Assert.That(scooter?.Rides?.ElementAt(0).Fee, Is.EqualTo(16));
                Assert.That(scooter?.Rides?.ElementAt(0).PaidAmount, Is.EqualTo(16));
                Assert.That(scooter?.Rides?.ElementAt(1).Customer?.Name, Is.EqualTo("Oliver"));
                Assert.That(scooter?.Rides?.ElementAt(1).Fee, Is.Null);
                Assert.That(scooter?.Rides?.ElementAt(1).PaidAmount, Is.Null);
            });

        }



        [TestCase("Java Wheels", 666, true)]
        [TestCase("sCoot#", 1000000, false)]

        public void TestDeletetScootersWithTrashyBrand(string name, int el, bool expected)
        {
            int brandId = EfMethods.CreateBrand(name);
            int scooterId = EfMethods.CreateScooter(brandId, el);

            bool actual = EfMethods.DeleteScooterIfTrashyBrand("Java Wheels");

            Assert.That(actual, Is.EqualTo(expected));

        }

    }
}