using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace AvaloniaApplication1
{
    public partial class BasketPage : UserControl
    {
        public BasketPage()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            var currentUser = Data.ContextData.CurrentLoggedInUser;
            if (currentUser == null) return;

            using var context = new AppDbContext();

            var basketItems = context.Baskets
                .Where(b => b.UserId == currentUser.Id)
                .Include(b => b.Movie)
                .ToList();

            var basketData = basketItems.Select(basket =>
            {
                var localDate = basket.AddedDate.ToLocalTime();
                return new
                {
                    Id = basket.Id,
                    MovieTitle = basket.Movie?.Title ?? "Неизвестный фильм",
                    MovieGenre = basket.Movie?.Genre ?? "Не указан",
                    MovieDirector = basket.Movie?.Director ?? "Не указан",
                    Quantity = basket.Quantity,
                    AddedDate = localDate.ToString("dd.MM.yyyy HH:mm")
                };
            }).ToList();

            dgBasket.ItemsSource = basketData;

            var totalItems = basketItems.Count;
            var totalQuantity = basketItems.Sum(b => b.Quantity);
            tbTotalInfo.Text = $"Всего: {totalItems} позиций, {totalQuantity} шт.";
        }

        private async void btnDeleteBasketItem_Click(object? sender, RoutedEventArgs e)
        {
            var basketItemData = (sender as Button).Tag;
            var idProperty = basketItemData.GetType().GetProperty("Id");
            if (idProperty == null) return;

            var basketId = (int)idProperty.GetValue(basketItemData);

            var confirmWindow = new ConfirmDialog("Удалить товар из корзины?");
            var parent = this.VisualRoot as Window;
            var result = await confirmWindow.ShowDialog<bool>(parent);

            if (result)
            {
                using var context = new AppDbContext();
                var basketItem = context.Baskets.FirstOrDefault(b => b.Id == basketId);
                if (basketItem != null)
                {
                    context.Baskets.Remove(basketItem);
                    context.SaveChanges();
                }
                Refresh();
            }
        }

        private async void btnEditBasketItem_Click(object? sender, RoutedEventArgs e)
        {
            var basketItemData = (sender as Button).Tag;
            var idProperty = basketItemData.GetType().GetProperty("Id");
            if (idProperty == null) return;

            var basketId = (int)idProperty.GetValue(basketItemData);

            using var context = new AppDbContext();
            var basketItem = context.Baskets.FirstOrDefault(b => b.Id == basketId);

            if (basketItem != null)
            {
                var editWindow = new EditBasketItemWindow(basketItem);
                var parent = this.VisualRoot as Window;
                await editWindow.ShowDialog(parent);
                Refresh();
            }
        }

        private async void btnClearBasket_Click(object? sender, RoutedEventArgs e)
        {
            var confirmWindow = new ConfirmDialog("Очистить всю корзину?");
            var parent = this.VisualRoot as Window;
            var result = await confirmWindow.ShowDialog<bool>(parent);

            if (result)
            {
                var currentUser = Data.ContextData.CurrentLoggedInUser;
                if (currentUser == null) return;

                using var context = new AppDbContext();
                var userBasketItems = context.Baskets
                    .Where(b => b.UserId == currentUser.Id)
                    .ToList();

                context.Baskets.RemoveRange(userBasketItems);
                context.SaveChanges();
                Refresh();
            }
        }

        private async void btnCheckout_Click(object? sender, RoutedEventArgs e)
        {
            var currentUser = Data.ContextData.CurrentLoggedInUser;
            if (currentUser == null) return;

            using var context = new AppDbContext();

            var basketItems = context.Baskets
                .Where(b => b.UserId == currentUser.Id)
                .Include(b => b.Movie)
                .ToList();

            if (!basketItems.Any())
            {
                ShowMessage("Корзина пуста");
                return;
            }

            try
            {
                var order = new Order
                {
                    UserId = currentUser.Id,
                    OrderDate = DateTime.UtcNow,
                    Status = "Completed"
                };

                context.Orders.Add(order);
                context.SaveChanges();

                foreach (var basketItem in basketItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        MovieId = basketItem.MovieId,
                        Quantity = basketItem.Quantity
                    };
                    context.OrderItems.Add(orderItem);
                }

                context.Baskets.RemoveRange(basketItems);
                context.SaveChanges();

                ShowMessage($"Заказ №{order.Id} успешно оформлен! Количество позиций: {basketItems.Count}");
                Refresh();
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при оформлении заказа: {ex.Message}");
            }
        }

        private void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}