namespace DOOR.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int DoorId { get; set; }
        public Door Door { get; set; }
    }
}
