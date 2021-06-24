using OBS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OBS.Services
{
    public interface IMyBookService
    {
        public Task<List<MyBooks>> GetAll(string UID);
        public Task<MyBooks> GetDetails(int id,string UID);
        public Task Insert(MyBooks book);
        public Task Update(int id, string id2, MyBooks book);


    }
}
