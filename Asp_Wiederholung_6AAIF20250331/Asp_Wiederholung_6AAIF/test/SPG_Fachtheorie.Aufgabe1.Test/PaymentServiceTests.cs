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

        [Theory]
        [InlineData(0, "Cash", 1, "Cash desk not found")]
        [InlineData(9999, "Cash", 1, "Cash desk not found")]
        [InlineData(1, "Invalid Type", 1, "Invalid payment type")] 
        [InlineData(1, "Cash", 0, "Employee not found")]
        [InlineData(1, "Cash", 999, "Employee not found")]
        [InlineData(1, "CreditCard", 1, "Insufficent rights to create a credit card payment")]
        public void CreatePaymentExceptionsTest(int cashDeskNum, string paymentType, int employeeNum, string errorMessage)
        {
            using var db = GetEmptyDbContext();
            var service = new PaymentService(db);

            var cashDesk = new CashDesk(1);
            var cashier = new Cashier(1, "D1", "J1", new DateOnly(1990, 1, 1), 5000, null, "General");
            var manager = new Manager(2, "D2", "J2", new DateOnly(2000, 1, 1), 4000, null, "Manager");

            db.CashDesks.Add(cashDesk);
            db.Employees.Add(cashier);
            db.Employees.Add(manager);
            db.SaveChanges();

            PaymentServiceException argumentException = Assert.Throws<PaymentServiceException>(() =>
                service.CreatePayment(new NewPaymentCommand(cashDeskNum, paymentType, employeeNum)));

            var exception = argumentException;

            Assert.Equal(errorMessage, exception.Message);
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

        [Theory]
        [InlineData(9999, "Payment not found")]
        [InlineData(1, "Payment already confirmed")]
        public void ConfirmPaymentExceptionsTest(int paymentId, string errorMessage)
        {
            using var db = GetEmptyDbContext();
            var service = new PaymentService(db);

            var cashDesk = new CashDesk(1);
            var cashier = new Cashier(1, "D1", "J1", new DateOnly(1990, 1, 1), 5000, null, "General");

            var payment = new Payment(cashDesk, DateTime.UtcNow, cashier, PaymentType.Cash)
            {
                Id = 1,
                Confirmed = paymentId == 1 ? DateTime.UtcNow : null
            };

            db.CashDesks.Add(cashDesk);
            db.Employees.Add(cashier);
            db.Payments.Add(payment);
            db.SaveChanges();

            PaymentServiceException argumentException = Assert.Throws<PaymentServiceException>(() =>
                service.ConfirmPayment(paymentId));

            var exception = argumentException;

            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public void ConfirmPaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var service = new PaymentService(db);

            var cashier = new Cashier(1001, "fn", "ln",
                new DateOnly(2000, 1, 1), 3000,
                null, "Kassier");
            var cashDesk = new CashDesk(1);

            var payment = new Payment(cashDesk, DateTime.UtcNow, cashier, PaymentType.Cash)
            {
                Id = 1,
                Confirmed = null
            };

            db.Employees.Add(cashier);
            db.CashDesks.Add(cashDesk);
            db.Payments.Add(payment);
            db.SaveChanges();

            service.ConfirmPayment(payment.Id);

            var updatedPayment = db.Payments.First(p => p.Id == payment.Id);
            Assert.NotNull(updatedPayment.Confirmed);
            Assert.True(updatedPayment.Confirmed <= DateTime.UtcNow);
        }

        [Theory]
        [InlineData("name", 5, 30.0, 9999, "Payment not found")]
        [InlineData("name", 5, 30.0, 1, "Payment not confirmed")]
        public void AddPaymentItemExceptionsTest(string articleName, int amount, decimal price, int paymentId, string errorMessage)
        {
            using var db = GetEmptyDbContext();
            var service = new PaymentService(db);

            var cashier = new Cashier(1001, "fn", "ln",
                new DateOnly(2000, 1, 1), 3000,
                null, "Kassier");
            var cashDesk = new CashDesk(1);

            var payment = new Payment(cashDesk, DateTime.UtcNow, cashier, PaymentType.Cash)
            {
                Id = 1,
            };

            var paymentItem = new PaymentItem(articleName, amount, price, payment);

            db.CashDesks.Add(cashDesk);
            db.Employees.Add(cashier);
            db.Payments.Add(payment);
            db.PaymentItems.Add(paymentItem);
            db.SaveChanges();

            PaymentServiceException argumentException = Assert.Throws<PaymentServiceException>(() =>
                service.AddPaymentItem(new NewPaymentItemCommand(
                    articleName, amount, price, paymentId)));

            var exception = argumentException;

            Assert.Equal(errorMessage, exception.Message);
        }

        [Fact]
        public void AddPaymentItemSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var service = new PaymentService(db);

            var cashier = new Cashier(1001, "fn", "ln",
                new DateOnly(2000, 1, 1), 3000,
                null, "Kassier");
            var cashDesk = new CashDesk(1);

            var payment = new Payment(cashDesk, DateTime.UtcNow, cashier, PaymentType.Cash)
            {
                Id = 1,
                Confirmed = DateTime.UtcNow
            };

            db.CashDesks.Add(cashDesk);
            db.Employees.Add(cashier);
            db.Payments.Add(payment);
            db.SaveChanges();

            var cmd = new NewPaymentItemCommand("name", 20, 30.0m, 1);

            service.AddPaymentItem(cmd);

            var paymentItem = db.PaymentItems.FirstOrDefault();

            Assert.NotNull(paymentItem);
            Assert.Equal("name", paymentItem.ArticleName);
            Assert.Equal(20, paymentItem.Amount);
            Assert.Equal(30.0m, paymentItem.Price);
            Assert.Equal(payment, paymentItem.Payment);
        }

        [Theory]
        [InlineData(999, "Payment not found")]
        public void DeletePaymentExceptionsTest(int paymentId, string errorMessage)
        {
            using var db = GetEmptyDbContext();
            var service = new PaymentService(db);

            var cashier = new Cashier(1001, "fn", "ln",
                new DateOnly(2000, 1, 1), 3000,
                null, "Kassier");
            var cashDesk = new CashDesk(1);

            var payment = new Payment(cashDesk, DateTime.UtcNow, cashier, PaymentType.Cash)
            {
                Id = 1,
            };

            db.CashDesks.Add(cashDesk);
            db.Employees.Add(cashier);
            db.Payments.Add(payment);

            db.SaveChanges();

            PaymentServiceException e = Assert.Throws<PaymentServiceException>(() => service.DeletePayment(paymentId, true));
            Assert.Equal(errorMessage, e.Message);
        }

        [Fact]
        public void DeletePaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var service = new PaymentService(db);

            var cashier = new Cashier(1001, "fn", "ln",
                new DateOnly(2000, 1, 1), 3000,
                null, "Kassier");
            var cashDesk = new CashDesk(1);

            var payment = new Payment(cashDesk, DateTime.UtcNow, cashier, PaymentType.Cash)
            {
                Id = 1,
            };

            db.CashDesks.Add(cashDesk);
            db.Employees.Add(cashier);
            db.Payments.Add(payment);
            
            db.SaveChanges();

            service.DeletePayment(1001, true);

            Assert.Null(db.Payments.Where(p => p.Id == 1001).FirstOrDefault());
            Assert.Null(db.PaymentItems.Where(p => p.Payment.Id == 1).FirstOrDefault());
        }
    }
}
