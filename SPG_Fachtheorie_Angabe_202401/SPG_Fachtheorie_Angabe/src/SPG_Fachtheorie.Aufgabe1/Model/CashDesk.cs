using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class CashDesk
    {
        [Key]
        public int Number { get; private set; }

        protected CashDesk() { }
        public CashDesk(int number) 
        {
            Number = number;
        }
    }
}