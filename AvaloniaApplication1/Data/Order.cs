using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Data
{
    // Упрощенная модель Order (без OrderItems)
    public partial class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Измените на UTC
        public string Status { get; set; } = "Completed";

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

