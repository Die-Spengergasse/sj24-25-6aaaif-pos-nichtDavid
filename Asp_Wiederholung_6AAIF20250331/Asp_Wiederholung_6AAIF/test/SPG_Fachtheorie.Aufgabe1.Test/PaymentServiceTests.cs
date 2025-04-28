using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe1.Services;
using SPG_Fachtheorie.Aufgabe1.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    [Collection("Sequential")]
    public class PaymentServiceTests
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

        [Fact]
        public void CreatePaymentExceptionsTest()
        {

        }

        [Fact]
        public void CreatePaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var service = new PaymentService(db);

            var employee = new Cashier(1001, "fn", "ln",
                new DateOnly(2000, 1, 1), 3000,
                null, "Kassier");
            var cashDesk = new CashDesk(1);
            var paymentType = PaymentType.Cash;

            db.Employees.Add(employee);
            db.CashDesks.Add(cashDesk);
            db.SaveChanges();

            var payment = service.CreatePayment(new NewPaymentCommand(
                    cashDesk.Number, paymentType.ToString(), employee.RegistrationNumber));


            Assert.Equal(employee, payment.Employee);
            Assert.Equal(cashDesk, payment.CashDesk);
            Assert.Equal(paymentType, payment.PaymentType);
        }
    }
}
