using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class PaymentItem
    {
        [Key]
        public int Id { get; set; }
        public string ArticleName { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public Payment Payment { get; set; }
        #pragma warning disable CS8618
        protected PaymentItem() { }
        #pragma warning restore CS8618
        public PaymentItem(string articleName, int amount, decimal price, Payment payment)
        {
            ArticleName = articleName;
            Amount = amount;
            Price = price;
            Payment = payment;
        }
    }
}