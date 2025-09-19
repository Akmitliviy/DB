using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Lab8Nastya.Components.Pages
{
    public partial class AddOrder
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        public BookStoreService BookStoreService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            order = new Lab8Nastya.Models.BookStore.Order();

            clientsForClientId = await BookStoreService.GetClients();

            locationsForLocationId = await BookStoreService.GetLocations();
        }
        protected bool errorVisible;
        protected Lab8Nastya.Models.BookStore.Order order;

        protected IEnumerable<Lab8Nastya.Models.BookStore.Client> clientsForClientId;

        protected IEnumerable<Lab8Nastya.Models.BookStore.Location> locationsForLocationId;

        protected async Task FormSubmit()
        {
            try
            {
                await BookStoreService.CreateOrder(order);
                DialogService.Close(order);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}