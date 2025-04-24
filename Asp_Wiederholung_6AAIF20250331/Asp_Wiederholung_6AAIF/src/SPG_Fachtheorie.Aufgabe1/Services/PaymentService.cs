using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using SPG_Fachtheorie.Aufgabe1.Commands;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPG_Fachtheorie.Aufgabe1.Services
{
    public class PaymentService
    {
        private readonly AppointmentContext _db;
        public IQueryable<PaymentItem> PaymentItems => _db.PaymentItems.AsQueryable();
        public IQueryable<Payment> Payments => _db.Payments.AsQueryable();

        public PaymentService(AppointmentContext db)
        {
            _db = db;
        }

        public Payment CreatePayment(NewPaymentCommand cmd)
        {
            DateTime paymentDateTime = DateTime.UtcNow;

            var cashDesk = _db.CashDesks
                .FirstOrDefault(c => c.Number == cmd.CashDeskNumber);
            if (cashDesk is null)
                throw new PaymentServiceException("Cash desk not found") { NotFoundException = true };
            var employee = _db.Employees
                .FirstOrDefault(e => e.RegistrationNumber == cmd.EmployeeRegistrationNumber);
            if (employee is null)
                throw new PaymentServiceException("Employee not found") { NotFoundException = true };
            var existingPayment = _db.Payments
                .FirstOrDefault(p => p.CashDesk.Number == cmd.CashDeskNumber
                    && p.Confirmed == null);
            if(existingPayment is not null)
                throw new PaymentServiceException("Open payment for cashdesk") { NotFoundException = true };

            if(!Enum.TryParse<PaymentType>(cmd.PaymentType, true, out var paymentType))
                throw new PaymentServiceException("Invalid payment type") { NotFoundException = true };

            var manager = _db.Managers
                .FirstOrDefault(m => m.RegistrationNumber == cmd.EmployeeRegistrationNumber);

            if(manager is null && paymentType == PaymentType.CreditCard)
                throw new PaymentServiceException("Insufficent rights to create a credit card payment") { NotFoundException = true };

            var payment = new Payment(cashDesk, paymentDateTime, employee, paymentType);
            _db.Payments.Add(payment);
            SaveOrThrow();
            return payment;
        }

        public void ConfirmPayment(int paymentId)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment is null)
                throw new PaymentServiceException("Payment not found") { NotFoundException = true };
            if(payment.Confirmed is not null)
                throw new PaymentServiceException("Payment already confirmed") { NotFoundException = true };
            payment.Confirmed = DateTime.UtcNow;
            _db.Payments.Update(payment);
            SaveOrThrow();
        }

        public void AddPaymentItem(NewPaymentItemCommand cmd)
        {
            var payment = _db.Payments
                .FirstOrDefault(p => p.Id == cmd.Payment.Id);
            if (payment is null)
                throw new PaymentServiceException("Payment not found") { NotFoundException = true };
            if (payment.Confirmed is null)
                throw new PaymentServiceException("Payment not confirmed") { NotFoundException = true };
            var paymentItem = new PaymentItem(cmd.ArticleName, cmd.Amount, cmd.Price, payment);
            _db.PaymentItems.Add(paymentItem);
            SaveOrThrow();
        }

        public void DeletePayment(int paymentId, bool deleteItems)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment is null) return;
            var paymentItems = _db.PaymentItems.Where(p => p.Payment.Id == paymentId).ToList();
            if (paymentItems.Any() && deleteItems)
            {
                try
                {
                    _db.PaymentItems.RemoveRange(paymentItems);
                    _db.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    throw new PaymentServiceException(e.InnerException?.Message ?? e.Message);
                }
                catch (InvalidOperationException e)
                {
                    throw new PaymentServiceException(
                        e.InnerException?.Message ?? e.Message);
                }
            }
            try
            {
                _db.Payments.Remove(payment);
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new PaymentServiceException(e.InnerException?.Message ?? e.Message);
            }
            catch (InvalidOperationException e)
            {
                throw new PaymentServiceException(
                    e.InnerException?.Message ?? e.Message);
            }
        }

        private void SaveOrThrow()
        {
            try
            {
                _db.SaveChanges();  // INSERT INTO
            }
            catch (DbUpdateException e)
            {
                throw new EmployeeServiceException(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}
