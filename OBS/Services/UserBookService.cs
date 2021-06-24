using Microsoft.EntityFrameworkCore;
using OBS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBS.Services
{
    public class UserBookService : IUserBookService
    {
        private readonly BooksContext context;
        public UserBookService(BooksContext _context) {
            context = _context;
        }
        public async Task ClearCart( string id2)
        {
            var Book = await Task.Run(()=> context.UserBooks.Where(UB=>UB.UserId == id2).ToList());
              context.UserBooks.RemoveRange(Book);
            await context.SaveChangesAsync();
           
        }
        public async Task<long> PricePerBook(int bookID,string id2)
        {
           
            //var total =await context.UserBooks.FromSqlRaw("select s.Price* u.Bought as total from Books s , UserBooks u where u.BookId = s.Id and s.Id = {0} and  u.UserId={1}", bookID,id2).FirstOrDefaultAsync();
            var x = from o in context.UserBooks
                    join od in context.Books on o.BookId equals od.Id
                    where o.BookId == bookID && o.UserId == id2
                    select od.Price * o.Bought;
            decimal y = x.FirstOrDefault().Value;
            
            //var toal = context.UserBooks.FromSqlRaw("SELECT sum( From Authors Where AuthorId = {0}", id).FirstOrDefault();
            return Convert.ToInt64(y);
        }



        public async Task Delete(int id, string id2)
        {
            var Book =await context.UserBooks.Include(U => U.Book).FirstOrDefaultAsync(UB => UB.BookId == id && UB.UserId == id2);

            if (Book.Bought == 1)
            {
               //var book = await context.UserBooks.Include(U => U.Book).FirstOrDefaultAsync(UB => UB.BookId == id && UB.UserId == id2);
                context.Remove(Book);
               await context.SaveChangesAsync();
            }
            else
            {
                Book.Bought = Book.Bought - 1;
             await  Update(id, id2, Book);
                await context.SaveChangesAsync();

            }
        }

        public async Task<List<UserBook>> GetAll(string id)
        {
            List<UserBook> UserB = await  context.UserBooks.Include(UB => UB.Book).Where(u=>u.UserId==id).ToListAsync();
            //context.SaveChanges();
            return UserB;
        }

        public async Task<UserBook> GetDetails(int id, string id2)
        {
           return await context.UserBooks.Include(U => U.Book).FirstOrDefaultAsync(UB => UB.BookId == id && UB.UserId == id2);
        }

        public  async Task Insert(UserBook userbook)
        {
          await context.UserBooks.AddAsync(userbook);
           await context.SaveChangesAsync();

        }

        public async Task  Update(int id, string id2, UserBook userbook)
        {
            UserBook userbookupdated =await context.UserBooks.FirstOrDefaultAsync(UB => UB.BookId == id && UB.UserId == id2);
            if (userbook.Bought != null)
            {
                userbookupdated.Bought = userbook.Bought;
            }
            if (userbook.Rating != null)
            {
                userbookupdated.Rating = userbook.Rating;
            }
              await  context.SaveChangesAsync();
            
        }
    }
}
