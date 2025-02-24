namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        #pragma warning disable CS8618
        protected Address() { }
        #pragma warning restore CS8618
        public Address(string street, string city, string zip)
        {
            Street = street;
            City = city;
            Zip = zip;
        }
    }
}