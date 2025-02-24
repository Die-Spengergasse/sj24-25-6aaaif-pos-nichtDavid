namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Manager : Employee
    {
        public string CarType { get; set; }
        #pragma warning disable CS8618
        protected Manager() { }
        #pragma warning restore CS8618
        public Manager(string carType) 
        {
            CarType = carType;
        }
    }
}