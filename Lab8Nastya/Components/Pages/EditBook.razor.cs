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
    public partial class EditBook
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
        public int BookId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            book = await BookStoreService.GetBookByBookId(BookId);

            authorsForAuthorId = await BookStoreService.GetAuthors();

            categoriesForCategoryId = await BookStoreService.GetCategories();

            publishersForPublisherId = await BookStoreService.GetPublishers();
        }
        protected bool errorVisible;
        protected Lab8Nastya.Models.BookStore.Book book;

        protected IEnumerable<Lab8Nastya.Models.BookStore.Author> authorsForAuthorId;

        protected IEnumerable<Lab8Nastya.Models.BookStore.Category> categoriesForCategoryId;

        protected IEnumerable<Lab8Nastya.Models.BookStore.Publisher> publishersForPublisherId;

        protected async Task FormSubmit()
        {
            try
            {
                await BookStoreService.UpdateBook(BookId, book);
                DialogService.Close(book);
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