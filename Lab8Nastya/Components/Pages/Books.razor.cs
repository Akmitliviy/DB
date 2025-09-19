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
    public partial class Books
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

        protected IEnumerable<Lab8Nastya.Models.BookStore.Book> books;

        protected RadzenDataGrid<Lab8Nastya.Models.BookStore.Book> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            books = await BookStoreService.GetBooks(new Query { Filter = $@"i => i.Title.Contains(@0) || i.Description.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Author,Category,Publisher" });
        }
        protected override async Task OnInitializedAsync()
        {
            books = await BookStoreService.GetBooks(new Query { Filter = $@"i => i.Title.Contains(@0) || i.Description.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Author,Category,Publisher" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddBook>("Add Book", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<Lab8Nastya.Models.BookStore.Book> args)
        {
            await DialogService.OpenAsync<EditBook>("Edit Book", new Dictionary<string, object> { {"BookId", args.Data.BookId} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Lab8Nastya.Models.BookStore.Book book)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await BookStoreService.DeleteBook(book.BookId);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Book"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await BookStoreService.ExportBooksToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Author,Category,Publisher",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Books");
            }

            if (args == null || args.Value == "xlsx")
            {
                await BookStoreService.ExportBooksToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Author,Category,Publisher",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Books");
            }
        }
    }
}