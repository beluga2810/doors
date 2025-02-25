using System.ComponentModel.DataAnnotations;

namespace DOOR.Models
{
    public class Door
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Material { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public List<Service> Services { get; set; }

        public List<Order> Orders { get; set; }
    }
}
