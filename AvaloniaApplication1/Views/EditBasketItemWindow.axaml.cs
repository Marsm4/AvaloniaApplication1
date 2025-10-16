using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1
{
    public partial class EditBasketItemWindow : Window
    {
        private Basket _basketItem;

        public EditBasketItemWindow(Basket basketItem)
        {
            InitializeComponent();
            _basketItem = basketItem;
            tbQuantity.Text = _basketItem.Quantity.ToString();
        }

        private void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbQuantity.Text, out int quantity) && quantity > 0)
            {
                _basketItem.Quantity = quantity;
                using var context = new AppDbContext();
                context.Baskets.Update(_basketItem);
                context.SaveChanges();
                this.Close();
            }
        }

        private void btnCancel_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}