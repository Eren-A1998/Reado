using Microsoft.EntityFrameworkCore;
using OBS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBS.Services
{
    public class MyBookService :IMyBookService
    {
        private readonly BooksContext context;
        public MyBookService(BooksContext _context)
        {
            context = _context;
        }

        public async Task<List<MyBooks>> GetAll(string id)
        {
            List<MyBooks> UserB = await context.MyBooks.Include(UB => UB.Book).Where(u => u.UserId == id).ToListAsync();
            //context.SaveChanges();
            return UserB;
        }

        public async Task<MyBooks> GetDetails(int id, string id2)
        {
            return await context.MyBooks.Include(U => U.Book).FirstOrDefaultAsync(UB => UB.BookId == id && UB.UserId == id2);
        }

        public async Task Insert(MyBooks book)
        {


           await context.MyBooks.AddAsync(book);
          await  context.SaveChangesAsync();

        }
        public async Task Update(int id, string id2, MyBooks userbook)
        {
            MyBooks bookupdated = await context.MyBooks.FirstOrDefaultAsync(UB => UB.BookId == id && UB.UserId == id2);
            if (userbook.Bought != null)
            {
                bookupdated.Bought = userbook.Bought;
            }
            if (userbook.Rating != null)
            {
                bookupdated.Rating = userbook.Rating;
            }
           await context.SaveChangesAsync();

        }


    }
}
