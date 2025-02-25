namespace DOOR.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerContactInfo { get; set; }
        public string AdditionalProperties { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int DoorId { get; set; }
        public Door Door { get; set; }
    }
}
