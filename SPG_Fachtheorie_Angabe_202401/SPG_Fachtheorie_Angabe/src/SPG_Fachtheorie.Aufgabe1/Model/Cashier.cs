namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Cashier : Employee
    {
        public string JobSerialization { get; set; }
        #pragma warning disable CS8618
        protected Cashier() { }
        #pragma warning restore CS8618
        public Cashier(int registrationNumber, string jobSerialization, string firstName, string lastName, Address address) 
            : base(registrationNumber, firstName, lastName, address)
        {
            JobSerialization = jobSerialization;
        }
    }
}