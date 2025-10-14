using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1.Views
{
    public partial class BasketEditView : UserControl
    {
        private Basket _basketItem;

        public BasketEditView()
        {
            InitializeComponent();

            if (ContextData.CurrentBasketItem != null)
            {
                _basketItem = ContextData.CurrentBasketItem;
                tbQuantity.Text = _basketItem.Quantity.ToString();
            }
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_basketItem != null && int.TryParse(tbQuantity.Text, out int quantity) && quantity > 0)
            {
                _basketItem.Quantity = quantity;
                App.dbContext.Baskets.Update(_basketItem);
                App.dbContext.SaveChanges();
            }

            CloseWindow();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            var window = (Window)this.VisualRoot;
            window?.Close();
        }
    }
}