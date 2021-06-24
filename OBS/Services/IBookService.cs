using OBS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBS.Services
{
    public interface IBookService
    {
        public Task<List<Book>> GetAll();
        public Task<Book> GetDetails(int id);
        public void Insert(Book book);
        public void UpdateBook(int id, Book book);
        public void DeleteBook(int id);
    }
}
