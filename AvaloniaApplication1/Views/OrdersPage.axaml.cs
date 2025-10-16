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
    public partial class OrdersPage : UserControl
    {
        public OrdersPage()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            var currentUser = Data.ContextData.CurrentLoggedInUser;
            if (currentUser == null) return;

            using var context = new AppDbContext();

            var orders = context.Orders
                .Where(o => o.UserId == currentUser.Id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Movie)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var ordersData = orders.Select(order => new
            {
                Id = order.Id,
                OrderDate = order.OrderDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                Status = order.Status,
                TotalItems = order.OrderItems.Sum(oi => oi.Quantity),
                ItemsCount = order.OrderItems.Count
            }).ToList();

            dgOrders.ItemsSource = ordersData;
        }

        private async void btnViewDetails_Click(object? sender, RoutedEventArgs e)
        {
            var orderData = (sender as Button).Tag;
            var idProperty = orderData.GetType().GetProperty("Id");
            if (idProperty == null) return;

            var orderId = (int)idProperty.GetValue(orderData);

            using var context = new AppDbContext();
            var order = context.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order != null)
            {
                var detailsWindow = new OrderDetailsWindow(order);
                var parent = this.VisualRoot as Window;
                await detailsWindow.ShowDialog(parent);
            }
        }

        private void btnRefresh_Click(object? sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}