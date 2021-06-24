using OBS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBS.Services
{
    public class BookService : IBookService
    {
        private readonly BooksContext context;
        public BookService(BooksContext context)
        {
            this.context = context;
        }
        public  void DeleteBook(int id)
        {
             context.Remove(context.Books.Find(id));
            context.SaveChanges();
        }

        public async Task<List<Book>> GetAll()
        {

            //context.SaveChanges();
            return await Task.Run(() => context.Books.ToList());
           
        }

        public async Task< Book> GetDetails(int id)
        {
            return await context.Books.FindAsync(id);
        }

        public async void Insert(Book book)
        {
           await context.Books.AddAsync(book);
            context.SaveChanges();
        }

        public  void UpdateBook(int id, Book book)
        {

            Book bookUpdated =  context.Books.Find(id);
            bookUpdated.Title = book.Title;
            bookUpdated.Author = book.Author;
            bookUpdated.Price = book.Price;
            bookUpdated.Description = book.Description;
            bookUpdated.PubDate = book.PubDate;
            bookUpdated.Genre = book.Genre;
            bookUpdated.UserBook = book.UserBook;

            


            context.SaveChanges();
        }
    }
}
