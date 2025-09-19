using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using Lab8Nastya.Data;

namespace Lab8Nastya
{
    public partial class BookStoreService
    {
        private readonly IDbContextFactory<BookStoreContext> contextFactory;
        private readonly NavigationManager navigationManager;

        public BookStoreService(IDbContextFactory<BookStoreContext> contextFactory, NavigationManager navigationManager)
        {
            this.contextFactory = contextFactory;
            this.navigationManager = navigationManager;
        }

        public void Reset()
        {
            using var context = contextFactory.CreateDbContext();
            contextFactory.CreateDbContext().ChangeTracker.Entries()
                .Where(e => e.Entity != null)
                .ToList()
                .ForEach(e => e.State = EntityState.Detached);
        }

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportAuthorsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/authors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/authors/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAuthorsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/authors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/authors/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAuthorsRead(ref IQueryable<Lab8Nastya.Models.BookStore.Author> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.Author>> GetAuthors(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Authors.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAuthorsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAuthorGet(Lab8Nastya.Models.BookStore.Author item);
        partial void OnGetAuthorByAuthorId(ref IQueryable<Lab8Nastya.Models.BookStore.Author> items);


        public async Task<Lab8Nastya.Models.BookStore.Author> GetAuthorByAuthorId(int authorid)
        {
            var items = contextFactory.CreateDbContext().Authors
                              .AsNoTracking()
                              .Where(i => i.AuthorId == authorid);

 
            OnGetAuthorByAuthorId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAuthorGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAuthorCreated(Lab8Nastya.Models.BookStore.Author item);
        partial void OnAfterAuthorCreated(Lab8Nastya.Models.BookStore.Author item);

        public async Task<Lab8Nastya.Models.BookStore.Author> CreateAuthor(Lab8Nastya.Models.BookStore.Author author)
        {
            OnAuthorCreated(author);

            var existingItem = contextFactory.CreateDbContext().Authors
                              .Where(i => i.AuthorId == author.AuthorId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Authors.Add(author);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(author).State = EntityState.Detached;
                throw;
            }

            OnAfterAuthorCreated(author);

            return author;
        }

        public async Task<Lab8Nastya.Models.BookStore.Author> CancelAuthorChanges(Lab8Nastya.Models.BookStore.Author item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAuthorUpdated(Lab8Nastya.Models.BookStore.Author item);
        partial void OnAfterAuthorUpdated(Lab8Nastya.Models.BookStore.Author item);

        /*public async Task<Lab8Nastya.Models.BookStore.Author> UpdateAuthor(int authorid, Lab8Nastya.Models.BookStore.Author author)
        {
            OnAuthorUpdated(author);

            var itemToUpdate = contextFactory.CreateDbContext().Authors
                              .Where(i => i.AuthorId == author.AuthorId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(author);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterAuthorUpdated(author);

            return author;
        }*/
 
        public async Task<Lab8Nastya.Models.BookStore.Author> UpdateAuthor(int authorId, Lab8Nastya.Models.BookStore.Author author)
        {
            using var context = contextFactory.CreateDbContext();
            using var transaction = await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            //Console.WriteLine($"Transaction Isolation Level: {transaction.getDbTransaction().IsolationLevel}");
            try
            {
                OnAuthorUpdated(author);

                var itemToUpdate = context.Authors
                    .Where(i => i.AuthorId == author.AuthorId)
                    .FirstOrDefault();

                if (itemToUpdate == null)
                {
                    throw new Exception("Item no longer available");
                }

                // Оновлення значень
                context.Entry(itemToUpdate).CurrentValues.SetValues(author);
                context.Entry(itemToUpdate).Property("RowVersion").OriginalValue = author.RowVersion;

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            

                OnAfterAuthorUpdated(author);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Concurrency error: the record was modified by another user.");
                Console.WriteLine(ex.Message);

                throw new Exception("Concurrency conflict occurred. The changes were not saved.", ex);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("An error occurred while updating the author.");
                Console.WriteLine(ex.Message);
                throw;
            }

            return author;
}


       
        partial void OnAuthorDeleted(Lab8Nastya.Models.BookStore.Author item);
        partial void OnAfterAuthorDeleted(Lab8Nastya.Models.BookStore.Author item);

        public async Task<Lab8Nastya.Models.BookStore.Author> DeleteAuthor(int authorid)
        {
            var itemToDelete = contextFactory.CreateDbContext().Authors
                              .Where(i => i.AuthorId == authorid)
                              .Include(i => i.Books)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAuthorDeleted(itemToDelete);


            contextFactory.CreateDbContext().Authors.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAuthorDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportBookAmountLocationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/bookamountlocations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/bookamountlocations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportBookAmountLocationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/bookamountlocations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/bookamountlocations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnBookAmountLocationsRead(ref IQueryable<Lab8Nastya.Models.BookStore.BookAmountLocation> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.BookAmountLocation>> GetBookAmountLocations(Query query = null)
        {
            var items = contextFactory.CreateDbContext().BookAmountLocations.AsQueryable();

            items = items.Include(i => i.Book);
            items = items.Include(i => i.Location);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnBookAmountLocationsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnBookAmountLocationGet(Lab8Nastya.Models.BookStore.BookAmountLocation item);
        partial void OnGetBookAmountLocationByRecordId(ref IQueryable<Lab8Nastya.Models.BookStore.BookAmountLocation> items);


        public async Task<Lab8Nastya.Models.BookStore.BookAmountLocation> GetBookAmountLocationByRecordId(int recordid)
        {
            var items = contextFactory.CreateDbContext().BookAmountLocations
                              .AsNoTracking()
                              .Where(i => i.RecordId == recordid);

            items = items.Include(i => i.Book);
            items = items.Include(i => i.Location);
 
            OnGetBookAmountLocationByRecordId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnBookAmountLocationGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnBookAmountLocationCreated(Lab8Nastya.Models.BookStore.BookAmountLocation item);
        partial void OnAfterBookAmountLocationCreated(Lab8Nastya.Models.BookStore.BookAmountLocation item);

        public async Task<Lab8Nastya.Models.BookStore.BookAmountLocation> CreateBookAmountLocation(Lab8Nastya.Models.BookStore.BookAmountLocation bookamountlocation)
        {
            OnBookAmountLocationCreated(bookamountlocation);

            var existingItem = contextFactory.CreateDbContext().BookAmountLocations
                              .Where(i => i.RecordId == bookamountlocation.RecordId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().BookAmountLocations.Add(bookamountlocation);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(bookamountlocation).State = EntityState.Detached;
                throw;
            }

            OnAfterBookAmountLocationCreated(bookamountlocation);

            return bookamountlocation;
        }

        public async Task<Lab8Nastya.Models.BookStore.BookAmountLocation> CancelBookAmountLocationChanges(Lab8Nastya.Models.BookStore.BookAmountLocation item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnBookAmountLocationUpdated(Lab8Nastya.Models.BookStore.BookAmountLocation item);
        partial void OnAfterBookAmountLocationUpdated(Lab8Nastya.Models.BookStore.BookAmountLocation item);

        public async Task<Lab8Nastya.Models.BookStore.BookAmountLocation> UpdateBookAmountLocation(int recordid, Lab8Nastya.Models.BookStore.BookAmountLocation bookamountlocation)
        {
            OnBookAmountLocationUpdated(bookamountlocation);

            var itemToUpdate = contextFactory.CreateDbContext().BookAmountLocations
                              .Where(i => i.RecordId == bookamountlocation.RecordId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(bookamountlocation);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterBookAmountLocationUpdated(bookamountlocation);

            return bookamountlocation;
        }

        partial void OnBookAmountLocationDeleted(Lab8Nastya.Models.BookStore.BookAmountLocation item);
        partial void OnAfterBookAmountLocationDeleted(Lab8Nastya.Models.BookStore.BookAmountLocation item);

        public async Task<Lab8Nastya.Models.BookStore.BookAmountLocation> DeleteBookAmountLocation(int recordid)
        {
            var itemToDelete = contextFactory.CreateDbContext().BookAmountLocations
                              .Where(i => i.RecordId == recordid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnBookAmountLocationDeleted(itemToDelete);


            contextFactory.CreateDbContext().BookAmountLocations.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterBookAmountLocationDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportBookAmountOrdersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/bookamountorders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/bookamountorders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportBookAmountOrdersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/bookamountorders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/bookamountorders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnBookAmountOrdersRead(ref IQueryable<Lab8Nastya.Models.BookStore.BookAmountOrder> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.BookAmountOrder>> GetBookAmountOrders(Query query = null)
        {
            var items = contextFactory.CreateDbContext().BookAmountOrders.AsQueryable();

            items = items.Include(i => i.Book);
            items = items.Include(i => i.Order);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnBookAmountOrdersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnBookAmountOrderGet(Lab8Nastya.Models.BookStore.BookAmountOrder item);
        partial void OnGetBookAmountOrderByRecordId(ref IQueryable<Lab8Nastya.Models.BookStore.BookAmountOrder> items);


        public async Task<Lab8Nastya.Models.BookStore.BookAmountOrder> GetBookAmountOrderByRecordId(int recordid)
        {
            var items = contextFactory.CreateDbContext().BookAmountOrders
                              .AsNoTracking()
                              .Where(i => i.RecordId == recordid);

            items = items.Include(i => i.Book);
            items = items.Include(i => i.Order);
 
            OnGetBookAmountOrderByRecordId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnBookAmountOrderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnBookAmountOrderCreated(Lab8Nastya.Models.BookStore.BookAmountOrder item);
        partial void OnAfterBookAmountOrderCreated(Lab8Nastya.Models.BookStore.BookAmountOrder item);

        public async Task<Lab8Nastya.Models.BookStore.BookAmountOrder> CreateBookAmountOrder(Lab8Nastya.Models.BookStore.BookAmountOrder bookamountorder)
        {
            OnBookAmountOrderCreated(bookamountorder);

            var existingItem = contextFactory.CreateDbContext().BookAmountOrders
                              .Where(i => i.RecordId == bookamountorder.RecordId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().BookAmountOrders.Add(bookamountorder);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(bookamountorder).State = EntityState.Detached;
                throw;
            }

            OnAfterBookAmountOrderCreated(bookamountorder);

            return bookamountorder;
        }

        public async Task<Lab8Nastya.Models.BookStore.BookAmountOrder> CancelBookAmountOrderChanges(Lab8Nastya.Models.BookStore.BookAmountOrder item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnBookAmountOrderUpdated(Lab8Nastya.Models.BookStore.BookAmountOrder item);
        partial void OnAfterBookAmountOrderUpdated(Lab8Nastya.Models.BookStore.BookAmountOrder item);

        public async Task<Lab8Nastya.Models.BookStore.BookAmountOrder> UpdateBookAmountOrder(int recordid, Lab8Nastya.Models.BookStore.BookAmountOrder bookamountorder)
        {
            OnBookAmountOrderUpdated(bookamountorder);

            var itemToUpdate = contextFactory.CreateDbContext().BookAmountOrders
                              .Where(i => i.RecordId == bookamountorder.RecordId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(bookamountorder);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterBookAmountOrderUpdated(bookamountorder);

            return bookamountorder;
        }

        partial void OnBookAmountOrderDeleted(Lab8Nastya.Models.BookStore.BookAmountOrder item);
        partial void OnAfterBookAmountOrderDeleted(Lab8Nastya.Models.BookStore.BookAmountOrder item);

        public async Task<Lab8Nastya.Models.BookStore.BookAmountOrder> DeleteBookAmountOrder(int recordid)
        {
            var itemToDelete = contextFactory.CreateDbContext().BookAmountOrders
                              .Where(i => i.RecordId == recordid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnBookAmountOrderDeleted(itemToDelete);


            contextFactory.CreateDbContext().BookAmountOrders.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterBookAmountOrderDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportBooksToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/books/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/books/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportBooksToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/books/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/books/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnBooksRead(ref IQueryable<Lab8Nastya.Models.BookStore.Book> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.Book>> GetBooks(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Books.AsQueryable();

            items = items.Include(i => i.Author);
            items = items.Include(i => i.Category);
            items = items.Include(i => i.Publisher);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnBooksRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnBookGet(Lab8Nastya.Models.BookStore.Book item);
        partial void OnGetBookByBookId(ref IQueryable<Lab8Nastya.Models.BookStore.Book> items);


        public async Task<Lab8Nastya.Models.BookStore.Book> GetBookByBookId(int bookid)
        {
            var items = contextFactory.CreateDbContext().Books
                              .AsNoTracking()
                              .Where(i => i.BookId == bookid);

            items = items.Include(i => i.Author);
            items = items.Include(i => i.Category);
            items = items.Include(i => i.Publisher);
 
            OnGetBookByBookId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnBookGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnBookCreated(Lab8Nastya.Models.BookStore.Book item);
        partial void OnAfterBookCreated(Lab8Nastya.Models.BookStore.Book item);

        public async Task<Lab8Nastya.Models.BookStore.Book> CreateBook(Lab8Nastya.Models.BookStore.Book book)
        {
            OnBookCreated(book);

            var existingItem = contextFactory.CreateDbContext().Books
                              .Where(i => i.BookId == book.BookId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Books.Add(book);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(book).State = EntityState.Detached;
                throw;
            }

            OnAfterBookCreated(book);

            return book;
        }

        public async Task<Lab8Nastya.Models.BookStore.Book> CancelBookChanges(Lab8Nastya.Models.BookStore.Book item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnBookUpdated(Lab8Nastya.Models.BookStore.Book item);
        partial void OnAfterBookUpdated(Lab8Nastya.Models.BookStore.Book item);

        public async Task<Lab8Nastya.Models.BookStore.Book> UpdateBook(int bookid, Lab8Nastya.Models.BookStore.Book book)
        {
            OnBookUpdated(book);

            var itemToUpdate = contextFactory.CreateDbContext().Books
                              .Where(i => i.BookId == book.BookId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(book);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterBookUpdated(book);

            return book;
        }

        partial void OnBookDeleted(Lab8Nastya.Models.BookStore.Book item);
        partial void OnAfterBookDeleted(Lab8Nastya.Models.BookStore.Book item);

        public async Task<Lab8Nastya.Models.BookStore.Book> DeleteBook(int bookid)
        {
            var itemToDelete = contextFactory.CreateDbContext().Books
                              .Where(i => i.BookId == bookid)
                              .Include(i => i.BookAmountLocations)
                              .Include(i => i.BookAmountOrders)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnBookDeleted(itemToDelete);


            contextFactory.CreateDbContext().Books.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterBookDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportCategoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/categories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/categories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCategoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/categories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/categories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCategoriesRead(ref IQueryable<Lab8Nastya.Models.BookStore.Category> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.Category>> GetCategories(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Categories.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCategoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCategoryGet(Lab8Nastya.Models.BookStore.Category item);
        partial void OnGetCategoryByCategoryId(ref IQueryable<Lab8Nastya.Models.BookStore.Category> items);


        public async Task<Lab8Nastya.Models.BookStore.Category> GetCategoryByCategoryId(int categoryid)
        {
            var items = contextFactory.CreateDbContext().Categories
                              .AsNoTracking()
                              .Where(i => i.CategoryId == categoryid);

 
            OnGetCategoryByCategoryId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCategoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCategoryCreated(Lab8Nastya.Models.BookStore.Category item);
        partial void OnAfterCategoryCreated(Lab8Nastya.Models.BookStore.Category item);

        public async Task<Lab8Nastya.Models.BookStore.Category> CreateCategory(Lab8Nastya.Models.BookStore.Category category)
        {
            OnCategoryCreated(category);

            var existingItem = contextFactory.CreateDbContext().Categories
                              .Where(i => i.CategoryId == category.CategoryId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Categories.Add(category);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(category).State = EntityState.Detached;
                throw;
            }

            OnAfterCategoryCreated(category);

            return category;
        }

        public async Task<Lab8Nastya.Models.BookStore.Category> CancelCategoryChanges(Lab8Nastya.Models.BookStore.Category item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCategoryUpdated(Lab8Nastya.Models.BookStore.Category item);
        partial void OnAfterCategoryUpdated(Lab8Nastya.Models.BookStore.Category item);

        public async Task<Lab8Nastya.Models.BookStore.Category> UpdateCategory(int categoryid, Lab8Nastya.Models.BookStore.Category category)
        {
            OnCategoryUpdated(category);

            var itemToUpdate = contextFactory.CreateDbContext().Categories
                              .Where(i => i.CategoryId == category.CategoryId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(category);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterCategoryUpdated(category);

            return category;
        }

        partial void OnCategoryDeleted(Lab8Nastya.Models.BookStore.Category item);
        partial void OnAfterCategoryDeleted(Lab8Nastya.Models.BookStore.Category item);

        public async Task<Lab8Nastya.Models.BookStore.Category> DeleteCategory(int categoryid)
        {
            var itemToDelete = contextFactory.CreateDbContext().Categories
                              .Where(i => i.CategoryId == categoryid)
                              .Include(i => i.Books)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCategoryDeleted(itemToDelete);


            contextFactory.CreateDbContext().Categories.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCategoryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportClientsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/clients/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/clients/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportClientsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/clients/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/clients/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnClientsRead(ref IQueryable<Lab8Nastya.Models.BookStore.Client> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.Client>> GetClients(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Clients.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnClientsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnClientGet(Lab8Nastya.Models.BookStore.Client item);
        partial void OnGetClientByClientId(ref IQueryable<Lab8Nastya.Models.BookStore.Client> items);


        public async Task<Lab8Nastya.Models.BookStore.Client> GetClientByClientId(int clientid)
        {
            var items = contextFactory.CreateDbContext().Clients
                              .AsNoTracking()
                              .Where(i => i.ClientId == clientid);

 
            OnGetClientByClientId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnClientGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnClientCreated(Lab8Nastya.Models.BookStore.Client item);
        partial void OnAfterClientCreated(Lab8Nastya.Models.BookStore.Client item);

        public async Task<Lab8Nastya.Models.BookStore.Client> CreateClient(Lab8Nastya.Models.BookStore.Client client)
        {
            OnClientCreated(client);

            var existingItem = contextFactory.CreateDbContext().Clients
                              .Where(i => i.ClientId == client.ClientId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Clients.Add(client);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(client).State = EntityState.Detached;
                throw;
            }

            OnAfterClientCreated(client);

            return client;
        }

        public async Task<Lab8Nastya.Models.BookStore.Client> CancelClientChanges(Lab8Nastya.Models.BookStore.Client item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnClientUpdated(Lab8Nastya.Models.BookStore.Client item);
        partial void OnAfterClientUpdated(Lab8Nastya.Models.BookStore.Client item);

        public async Task<Lab8Nastya.Models.BookStore.Client> UpdateClient(int clientid, Lab8Nastya.Models.BookStore.Client client)
        {
            OnClientUpdated(client);

            var itemToUpdate = contextFactory.CreateDbContext().Clients
                              .Where(i => i.ClientId == client.ClientId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(client);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterClientUpdated(client);

            return client;
        }

        partial void OnClientDeleted(Lab8Nastya.Models.BookStore.Client item);
        partial void OnAfterClientDeleted(Lab8Nastya.Models.BookStore.Client item);

        public async Task<Lab8Nastya.Models.BookStore.Client> DeleteClient(int clientid)
        {
            var itemToDelete = contextFactory.CreateDbContext().Clients
                              .Where(i => i.ClientId == clientid)
                              .Include(i => i.Orders)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnClientDeleted(itemToDelete);


            contextFactory.CreateDbContext().Clients.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterClientDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportLocationsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/locations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/locations/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportLocationsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/locations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/locations/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnLocationsRead(ref IQueryable<Lab8Nastya.Models.BookStore.Location> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.Location>> GetLocations(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Locations.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnLocationsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnLocationGet(Lab8Nastya.Models.BookStore.Location item);
        partial void OnGetLocationByLocationId(ref IQueryable<Lab8Nastya.Models.BookStore.Location> items);


        public async Task<Lab8Nastya.Models.BookStore.Location> GetLocationByLocationId(int locationid)
        {
            var items = contextFactory.CreateDbContext().Locations
                              .AsNoTracking()
                              .Where(i => i.LocationId == locationid);

 
            OnGetLocationByLocationId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnLocationGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnLocationCreated(Lab8Nastya.Models.BookStore.Location item);
        partial void OnAfterLocationCreated(Lab8Nastya.Models.BookStore.Location item);

        public async Task<Lab8Nastya.Models.BookStore.Location> CreateLocation(Lab8Nastya.Models.BookStore.Location location)
        {
            OnLocationCreated(location);

            var existingItem = contextFactory.CreateDbContext().Locations
                              .Where(i => i.LocationId == location.LocationId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Locations.Add(location);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(location).State = EntityState.Detached;
                throw;
            }

            OnAfterLocationCreated(location);

            return location;
        }

        public async Task<Lab8Nastya.Models.BookStore.Location> CancelLocationChanges(Lab8Nastya.Models.BookStore.Location item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnLocationUpdated(Lab8Nastya.Models.BookStore.Location item);
        partial void OnAfterLocationUpdated(Lab8Nastya.Models.BookStore.Location item);

        public async Task<Lab8Nastya.Models.BookStore.Location> UpdateLocation(int locationid, Lab8Nastya.Models.BookStore.Location location)
        {
            OnLocationUpdated(location);

            var itemToUpdate = contextFactory.CreateDbContext().Locations
                              .Where(i => i.LocationId == location.LocationId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(location);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterLocationUpdated(location);

            return location;
        }

        partial void OnLocationDeleted(Lab8Nastya.Models.BookStore.Location item);
        partial void OnAfterLocationDeleted(Lab8Nastya.Models.BookStore.Location item);

        public async Task<Lab8Nastya.Models.BookStore.Location> DeleteLocation(int locationid)
        {
            var itemToDelete = contextFactory.CreateDbContext().Locations
                              .Where(i => i.LocationId == locationid)
                              .Include(i => i.BookAmountLocations)
                              .Include(i => i.Orders)
                              .Include(i => i.WorkingHours)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnLocationDeleted(itemToDelete);


            contextFactory.CreateDbContext().Locations.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterLocationDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOrdersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/orders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/orders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOrdersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/orders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/orders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOrdersRead(ref IQueryable<Lab8Nastya.Models.BookStore.Order> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.Order>> GetOrders(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Orders.AsQueryable();

            items = items.Include(i => i.Client);
            items = items.Include(i => i.Location);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnOrdersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOrderGet(Lab8Nastya.Models.BookStore.Order item);
        partial void OnGetOrderByOrderId(ref IQueryable<Lab8Nastya.Models.BookStore.Order> items);


        public async Task<Lab8Nastya.Models.BookStore.Order> GetOrderByOrderId(int orderid)
        {
            var items = contextFactory.CreateDbContext().Orders
                              .AsNoTracking()
                              .Where(i => i.OrderId == orderid);

            items = items.Include(i => i.Client);
            items = items.Include(i => i.Location);
 
            OnGetOrderByOrderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOrderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOrderCreated(Lab8Nastya.Models.BookStore.Order item);
        partial void OnAfterOrderCreated(Lab8Nastya.Models.BookStore.Order item);

        public async Task<Lab8Nastya.Models.BookStore.Order> CreateOrder(Lab8Nastya.Models.BookStore.Order order)
        {
            OnOrderCreated(order);

            var existingItem = contextFactory.CreateDbContext().Orders
                              .Where(i => i.OrderId == order.OrderId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Orders.Add(order);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(order).State = EntityState.Detached;
                throw;
            }

            OnAfterOrderCreated(order);

            return order;
        }

        public async Task<Lab8Nastya.Models.BookStore.Order> CancelOrderChanges(Lab8Nastya.Models.BookStore.Order item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOrderUpdated(Lab8Nastya.Models.BookStore.Order item);
        partial void OnAfterOrderUpdated(Lab8Nastya.Models.BookStore.Order item);

        public async Task<Lab8Nastya.Models.BookStore.Order> UpdateOrder(int orderid, Lab8Nastya.Models.BookStore.Order order)
        {
            OnOrderUpdated(order);

            var itemToUpdate = contextFactory.CreateDbContext().Orders
                              .Where(i => i.OrderId == order.OrderId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(order);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterOrderUpdated(order);

            return order;
        }

        partial void OnOrderDeleted(Lab8Nastya.Models.BookStore.Order item);
        partial void OnAfterOrderDeleted(Lab8Nastya.Models.BookStore.Order item);

        public async Task<Lab8Nastya.Models.BookStore.Order> DeleteOrder(int orderid)
        {
            var itemToDelete = contextFactory.CreateDbContext().Orders
                              .Where(i => i.OrderId == orderid)
                              .Include(i => i.BookAmountOrders)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOrderDeleted(itemToDelete);


            contextFactory.CreateDbContext().Orders.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOrderDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPublishersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/publishers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/publishers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPublishersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/publishers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/publishers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPublishersRead(ref IQueryable<Lab8Nastya.Models.BookStore.Publisher> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.Publisher>> GetPublishers(Query query = null)
        {
            var items = contextFactory.CreateDbContext().Publishers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnPublishersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPublisherGet(Lab8Nastya.Models.BookStore.Publisher item);
        partial void OnGetPublisherByPublisherId(ref IQueryable<Lab8Nastya.Models.BookStore.Publisher> items);


        public async Task<Lab8Nastya.Models.BookStore.Publisher> GetPublisherByPublisherId(int publisherid)
        {
            var items = contextFactory.CreateDbContext().Publishers
                              .AsNoTracking()
                              .Where(i => i.PublisherId == publisherid);

 
            OnGetPublisherByPublisherId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnPublisherGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPublisherCreated(Lab8Nastya.Models.BookStore.Publisher item);
        partial void OnAfterPublisherCreated(Lab8Nastya.Models.BookStore.Publisher item);

        public async Task<Lab8Nastya.Models.BookStore.Publisher> CreatePublisher(Lab8Nastya.Models.BookStore.Publisher publisher)
        {
            OnPublisherCreated(publisher);

            var existingItem = contextFactory.CreateDbContext().Publishers
                              .Where(i => i.PublisherId == publisher.PublisherId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().Publishers.Add(publisher);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(publisher).State = EntityState.Detached;
                throw;
            }

            OnAfterPublisherCreated(publisher);

            return publisher;
        }

        public async Task<Lab8Nastya.Models.BookStore.Publisher> CancelPublisherChanges(Lab8Nastya.Models.BookStore.Publisher item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPublisherUpdated(Lab8Nastya.Models.BookStore.Publisher item);
        partial void OnAfterPublisherUpdated(Lab8Nastya.Models.BookStore.Publisher item);

        public async Task<Lab8Nastya.Models.BookStore.Publisher> UpdatePublisher(int publisherid, Lab8Nastya.Models.BookStore.Publisher publisher)
        {
            OnPublisherUpdated(publisher);

            var itemToUpdate = contextFactory.CreateDbContext().Publishers
                              .Where(i => i.PublisherId == publisher.PublisherId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(publisher);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterPublisherUpdated(publisher);

            return publisher;
        }

        partial void OnPublisherDeleted(Lab8Nastya.Models.BookStore.Publisher item);
        partial void OnAfterPublisherDeleted(Lab8Nastya.Models.BookStore.Publisher item);

        public async Task<Lab8Nastya.Models.BookStore.Publisher> DeletePublisher(int publisherid)
        {
            var itemToDelete = contextFactory.CreateDbContext().Publishers
                              .Where(i => i.PublisherId == publisherid)
                              .Include(i => i.Books)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPublisherDeleted(itemToDelete);


            contextFactory.CreateDbContext().Publishers.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPublisherDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportWorkingHoursToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/workinghours/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/workinghours/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportWorkingHoursToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/workinghours/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/workinghours/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnWorkingHoursRead(ref IQueryable<Lab8Nastya.Models.BookStore.WorkingHour> items);

        public async Task<IQueryable<Lab8Nastya.Models.BookStore.WorkingHour>> GetWorkingHours(Query query = null)
        {
            var items = contextFactory.CreateDbContext().WorkingHours.AsQueryable();

            items = items.Include(i => i.Location);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnWorkingHoursRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnWorkingHourGet(Lab8Nastya.Models.BookStore.WorkingHour item);
        partial void OnGetWorkingHourByLocationId(ref IQueryable<Lab8Nastya.Models.BookStore.WorkingHour> items);


        public async Task<Lab8Nastya.Models.BookStore.WorkingHour> GetWorkingHourByLocationId(int locationid)
        {
            var items = contextFactory.CreateDbContext().WorkingHours
                              .AsNoTracking()
                              .Where(i => i.LocationId == locationid);

            items = items.Include(i => i.Location);
 
            OnGetWorkingHourByLocationId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnWorkingHourGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnWorkingHourCreated(Lab8Nastya.Models.BookStore.WorkingHour item);
        partial void OnAfterWorkingHourCreated(Lab8Nastya.Models.BookStore.WorkingHour item);

        public async Task<Lab8Nastya.Models.BookStore.WorkingHour> CreateWorkingHour(Lab8Nastya.Models.BookStore.WorkingHour workinghour)
        {
            OnWorkingHourCreated(workinghour);

            var existingItem = contextFactory.CreateDbContext().WorkingHours
                              .Where(i => i.LocationId == workinghour.LocationId)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                contextFactory.CreateDbContext().WorkingHours.Add(workinghour);
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(workinghour).State = EntityState.Detached;
                throw;
            }

            OnAfterWorkingHourCreated(workinghour);

            return workinghour;
        }

        public async Task<Lab8Nastya.Models.BookStore.WorkingHour> CancelWorkingHourChanges(Lab8Nastya.Models.BookStore.WorkingHour item)
        {
            var entityToCancel = contextFactory.CreateDbContext().Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnWorkingHourUpdated(Lab8Nastya.Models.BookStore.WorkingHour item);
        partial void OnAfterWorkingHourUpdated(Lab8Nastya.Models.BookStore.WorkingHour item);

        public async Task<Lab8Nastya.Models.BookStore.WorkingHour> UpdateWorkingHour(int locationid, Lab8Nastya.Models.BookStore.WorkingHour workinghour)
        {
            OnWorkingHourUpdated(workinghour);

            var itemToUpdate = contextFactory.CreateDbContext().WorkingHours
                              .Where(i => i.LocationId == workinghour.LocationId)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = contextFactory.CreateDbContext().Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(workinghour);
            entryToUpdate.State = EntityState.Modified;

            contextFactory.CreateDbContext().SaveChanges();

            OnAfterWorkingHourUpdated(workinghour);

            return workinghour;
        }

        partial void OnWorkingHourDeleted(Lab8Nastya.Models.BookStore.WorkingHour item);
        partial void OnAfterWorkingHourDeleted(Lab8Nastya.Models.BookStore.WorkingHour item);

        public async Task<Lab8Nastya.Models.BookStore.WorkingHour> DeleteWorkingHour(int locationid)
        {
            var itemToDelete = contextFactory.CreateDbContext().WorkingHours
                              .Where(i => i.LocationId == locationid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnWorkingHourDeleted(itemToDelete);


            contextFactory.CreateDbContext().WorkingHours.Remove(itemToDelete);

            try
            {
                contextFactory.CreateDbContext().SaveChanges();
            }
            catch
            {
                contextFactory.CreateDbContext().Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterWorkingHourDeleted(itemToDelete);

            return itemToDelete;
        }
    
      public async Task ExportGetClientsByOrderStatusesToExcel(Query query = null, string fileName = null)
      {
          navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/getclientsbyorderstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/getclientsbyorderstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
      }

      public async Task ExportGetClientsByOrderStatusesToCSV(Query query = null, string fileName = null)
      {
          navigationManager.NavigateTo(query != null ? query.ToUrl($"export/bookstore/getclientsbyorderstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/bookstore/getclientsbyorderstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
      }

      public async Task<IQueryable<Lab8Nastya.Models.BookStore.GetClientsByOrderStatus>> GetGetClientsByOrderStatuses(Query query = null)
      {
          OnGetClientsByOrderStatusesDefaultParams();

          var items = contextFactory.CreateDbContext().GetClientsByOrderStatuses.FromSqlInterpolated($"EXEC [dbo].[GetClientsByOrderStatus] ").ToList().AsQueryable();

          if (query != null)
          {
              if (!string.IsNullOrEmpty(query.Expand))
              {
                  var propertiesToExpand = query.Expand.Split(',');
                  foreach(var p in propertiesToExpand)
                  {
                      items = items.Include(p.Trim());
                  }
              }

              ApplyQuery(ref items, query);
          }
          
          OnGetClientsByOrderStatusesInvoke(ref items);

          return await Task.FromResult(items);
      }

      partial void OnGetClientsByOrderStatusesDefaultParams();

      partial void OnGetClientsByOrderStatusesInvoke(ref IQueryable<Lab8Nastya.Models.BookStore.GetClientsByOrderStatus> items);  
    }
}