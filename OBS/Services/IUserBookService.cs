using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBS.Models;

namespace OBS.Services
{
    public interface IUserBookService
    {
        public Task<List<UserBook>> GetAll(string id);
        public Task<UserBook> GetDetails(int id, string id2);
        public Task Insert(UserBook userbook);
        public Task Update(int id,string id2, UserBook userbook);
       public Task Delete(int id,string id2);
        public Task ClearCart(string id);
        public Task<long> PricePerBook(int bookID, string id2);

    }
}
