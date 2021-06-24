using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBS.Models;
using OBS.Services;
using Stripe;

namespace OBS.Controllers
{
    [Authorize(Roles = "OBSuser")]
    
    //[HandleError(View = "OError")]

    public class BooksController : Controller
    {
        private readonly IUserBookService _userBookService;
        private readonly IBookService _bookService;
        private readonly IMyBookService _mybookService;

        public BooksController(IBookService bookService,IUserBookService userBookService, IMyBookService mybookService)
        {
            _bookService = bookService;

            _userBookService = userBookService;
            _mybookService = mybookService;
        }
        public async Task<IActionResult> Charge(string stripeEmail,string stripeToken)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();
            var customer = customers.Create(new CustomerCreateOptions { Email = stripeEmail ,Source=stripeToken});//feh token ml2nhash

            string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";
            var mylist =await _userBookService.GetAll(UID);
            long total = 0;
            foreach(var item in mylist)
            {
                total += await _userBookService.PricePerBook(item.BookId, UID);
            }

            Trace.WriteLine(total);
            
            if(total!=0)
            { 
            var charge = charges.Create(new ChargeCreateOptions { Amount = total*100, Description = "Test", Currency = "usd" ,
                Customer=customer.Id, ReceiptEmail=stripeEmail,Metadata=new Dictionary<string, string>(){
                    { "OrderID","112"},{"Postcode","22" } }
            });// custidmsh mwgod
            if(charge.Status=="succeeded")
            {
                string B = charge.BalanceTransactionId;
                await Checkout();
                return RedirectToAction("myBooks", "Books");

            }
            }
            else
            {
                await Checkout();
                return RedirectToAction("myBooks", "Books");
            }

            return RedirectToAction("index", "Books");
        }

        //ghalebn mlhash lzmaa 
       

            public async Task<ActionResult> myBooks()
        {
            string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";

            return View(await _mybookService.GetAll(UID));

        }
        //[HttpPost]
        //[AcceptVerbs("Get","Post")]
        //[HttpPost]
        private async Task Checkout()
        {

            string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";

            var bookInCart = await  _userBookService.GetAll(UID);
            
            foreach(var book in bookInCart)
            {
                MyBooks mbook = new MyBooks();
                mbook.UserId = book.UserId;
                mbook.BookId = book.BookId;
                mbook.Bought =book.Bought;
               // mbook.Book = book.Book;
                mbook.Rating = book.Rating;
                //MyBooks currentbook =  _mybookService.GetAll(UID).Result.FirstOrDefault(UB => UB.BookId == book.BookId && UB.UserId == UID);
                MyBooks currentbook = await _mybookService.GetDetails(book.BookId, UID);
                if (currentbook == null)
                {
                  await  _mybookService.Insert(mbook);

                }
                else
                {
                    currentbook.Bought += mbook.Bought;
                    await _mybookService.Update(currentbook.BookId, UID, currentbook);
                    //Trace.WriteLine(Userbook.Bought);

                }
                //_mybookService.Insert(mbook);

            }
            await _userBookService.ClearCart(UID);
        }

        // GET: Books
        //public async Task<IActionResult> Index()
        //{
        //    return View( await _bookService.GetAll());
        //}
        [HttpPost]
        public async Task<IActionResult> Index(Book book, int min, int max,string Freebooks)
        {
            List<Book> books = _bookService.GetAll().Result;
            bool free = false;

            if(Freebooks !=null )
            {
                books = await Task.Run(() => _bookService.GetAll().Result.Where(U => U.Price==0).ToList());
                free = true;
            }

            if (book.Title != null) { 

                 books = await Task.Run(() => _bookService.GetAll().Result.Where(U => U.Title.Contains(book.Title)).ToList());

             }
            if(book.Author!=null)
            {
                 books = await Task.Run(() => books.Where(u => u.Author.Contains(book.Author)).ToList());

            }
            if(book.Genre!=Genre.ChooseGenre)
            {
                books = await Task.Run(() => books.Where(u => u.Genre.Equals(book.Genre)).ToList());

            }
            if(book.PubDate!=null)
            {
                 books = await Task.Run(() => books.Where(u => u.PubDate.Equals(book.PubDate)).ToList());

            }
            if( min>0&&min<max&&!free)
            {
                books = await Task.Run(() => books.Where(u => u.Price > min && u.Price < max).ToList());

            }

            return View( books);
        }

        public async Task<IActionResult> Index(string item,string order)
        {
            if (item == null && order == null)
            {
                return View(await _bookService.GetAll());

            }
            else
            {
                List<Book> listbooks = _bookService.GetAll().Result;
                switch (item)
                {
                    case "title":
                        if (order == "asc")
                        {
                            listbooks = listbooks.OrderBy(o => o.Title).ToList();
                        }
                        else
                        {
                            listbooks = listbooks.OrderByDescending(o => o.Title).ToList();

                        }



                        break;
                    case "author":
                        if (order == "asc")
                        {
                            listbooks = listbooks.OrderBy(o => o.Author).ToList();
                        }
                        else
                        {
                            listbooks = listbooks.OrderByDescending(o => o.Author).ToList();

                        }



                        break;
                    case "price":
                        if (order == "asc")
                        {
                            listbooks = listbooks.OrderBy(o => o.Price).ToList();
                        }
                        else
                        {
                            listbooks = listbooks.OrderByDescending(o => o.Price).ToList();

                        }



                        break;
                    case "pubdate":
                        if (order == "asc")
                        {
                            listbooks = listbooks.OrderBy(o => o.PubDate).ToList();
                        }
                        else
                        {
                            listbooks = listbooks.OrderByDescending(o => o.PubDate).ToList();

                        }



                        break;

                }
                ViewBag.item = item;
                ViewBag.order= order;


                return View(listbooks);
            }

        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetDetails(id);

            //if (book == null)
            //{
            //    return NotFound();
            //}

            return View(book);
        }
        public async Task<ActionResult> Buy(int id)
        {

            // Book book = _context.Books.FirstOrDefault(b => b.Id == id);
            Book book = await _bookService.GetDetails(id);
            return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> Buy(Book book)
        {
            string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";
            // UserBook Userbook = _userBookService.GetAll(UID).Result.FirstOrDefault(UB => UB.BookId == book.Id && UB.UserId == UID);
            UserBook Userbook =await _userBookService.GetDetails(book.Id, UID);
            if(Userbook == null)
            {
               await _userBookService.Insert(new UserBook() { BookId = book.Id, UserId = UID, Rating = null, Bought=1 });

            }
            else
            {
                Userbook.Bought++;
                await _userBookService.Update(book.Id,UID, Userbook);
                //Trace.WriteLine(Userbook.Bought);

            }
      
            return RedirectToAction("Index", "UserBooks");
        }
        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Title,Author,Price,PubDate,Genre")] Book book)
        {
            if (ModelState.IsValid)
            {
                _bookService.Insert(book);
            
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        //// GET: Books/Edit/5
        //public async Task< IActionResult> Edit(int id)
        //{

        //    var book = await _bookService.GetDetails(id);
        //    if (book == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(book);
        //}

        //// POST: Books/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Price,PubDate,Genre")] Book book)
        //{
        //    if (id != book.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //           await Task.Run(()=> _bookService.UpdateBook(id,book));
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BookExists(book.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(book);
        //}

        public async Task<IActionResult> Edit(int id)
        {
            string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";


            var userBook = await _mybookService.GetDetails(id, UID);
            if (userBook == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_mybookService.GetAll(UID).Result, "Id", "Author", userBook.BookId);
            return View(userBook);
        }

        // POST: UserBooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,UserId,Rating")] MyBooks userBook)
        {
            string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";

            if (id != userBook.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _mybookService.Update(id, UID, userBook);
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                    
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookId"] = new SelectList(_mybookService.GetAll(UID).Result, "Id", "Author", userBook.BookId);
            return View(userBook);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            
             

            var book =await  _bookService.GetDetails(id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  async Task<IActionResult> DeleteConfirmed(int id)
        {
          
            await Task.Run(()=> _bookService.DeleteBook(id));
            
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _bookService.GetAll().Result.Any(e => e.Id == id);
        }
    }
}
