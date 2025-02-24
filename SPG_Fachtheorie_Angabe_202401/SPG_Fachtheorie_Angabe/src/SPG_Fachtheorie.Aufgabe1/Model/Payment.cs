using System;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public CashDesk CashDesk { get; set; }
        public DateTime PaymentDateTime { get; set; }
        public PaymentType PaymentType { get; set; }
        public Employee Employee { get; set; }
        #pragma warning disable CS8618
        protected Payment() { }
        #pragma warning restore CS8618
        public Payment(CashDesk cashDesk, DateTime paymentDateTime, PaymentType paymentType, Employee employee)
        {
            CashDesk = cashDesk;
            PaymentDateTime = paymentDateTime;
            PaymentType = paymentType;
            Employee = employee;
        }
    }
}