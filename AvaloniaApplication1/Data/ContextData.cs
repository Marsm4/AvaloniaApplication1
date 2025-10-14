using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Data
{
    internal class ContextData
    {
        public static User? CurrentUser { get; set; }
        public static Movie? CurrentMovie { get; set; }
        public static User? CurrentLoggedInUser { get; set; }
        public static Basket? CurrentBasketItem { get; set; }
        public static Category? CurrentCategory { get; set; }   
        
    }
}
