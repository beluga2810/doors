namespace DOOR.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public List<Employee> employees { get; set; }
    }
}
