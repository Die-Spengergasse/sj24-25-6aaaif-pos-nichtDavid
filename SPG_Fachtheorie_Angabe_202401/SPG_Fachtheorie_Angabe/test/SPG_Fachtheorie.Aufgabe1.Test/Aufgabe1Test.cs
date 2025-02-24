using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    [Collection("Sequential")]
    public class Aufgabe1Test
    {
        private AppointmentContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite(@"Data Source=cash.db")
                .Options;

            var db = new AppointmentContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        // Creates an empty DB in Debug\net8.0\cash.db
        [Fact]
        public void CreateDatabaseTest()
        {
            using var db = GetEmptyDbContext();
        }

        [Fact]
        public void AddCashierSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var cashier = new Cashier(1, "fn", "firstname", "lastname", new Address("street1", "city1", "1000"));
            db.Cashiers.Add(cashier);
            db.SaveChanges();
            db.ChangeTracker.Clear();
            var cashierFromDb = db.Cashiers.First();
            Assert.True(cashierFromDb.RegistrationNumber == 1);
            Assert.True(cashierFromDb.JobSerialization == "fn");
        }

        [Fact]
        public void AddPaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var cashier = new Cashier(1, "fn", "firstname", "lastname", new Address("street1", "city1", "1000"));
            var payment = new Payment(new CashDesk(1), DateTime.Now, PaymentType.Cash, cashier);
            db.Payments.Add(payment);
            db.SaveChanges();
            db.ChangeTracker.Clear();
            var paymentFromDb = db.Payments.First();
            Assert.True(paymentFromDb.Id == 1);
        }

        [Fact]
        public void EmployeeDiscriminatorSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var cashier = new Cashier(1, "fn", "firstname", "lastname", new Address("street1", "city1", "1000"));
            db.Cashiers.Add(cashier);
            db.SaveChanges();
            db.ChangeTracker.Clear();
            var cashierFromDb = db.Cashiers.First();
            Assert.True(cashierFromDb.Type == "Cashier");
        }
    }
}