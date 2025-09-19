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
    public partial class EditBookAmountOrder
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

        [Parameter]
        public int RecordId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            bookAmountOrder = await BookStoreService.GetBookAmountOrderByRecordId(RecordId);

            ordersForOrderId = await BookStoreService.GetOrders();

            booksForBookId = await BookStoreService.GetBooks();
        }
        protected bool errorVisible;
        protected Lab8Nastya.Models.BookStore.BookAmountOrder bookAmountOrder;

        protected IEnumerable<Lab8Nastya.Models.BookStore.Order> ordersForOrderId;

        protected IEnumerable<Lab8Nastya.Models.BookStore.Book> booksForBookId;

        protected async Task FormSubmit()
        {
            try
            {
                await BookStoreService.UpdateBookAmountOrder(RecordId, bookAmountOrder);
                DialogService.Close(bookAmountOrder);
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