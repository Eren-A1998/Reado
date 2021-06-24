using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OBS.Models;
using OBS.Services;

namespace OBS.Controllers
{
    //[Authorize(Roles = "Manager")]
    [Authorize(Roles = "OBSuser")]


    public class UserBooksController : Controller
    {
        private readonly IUserBookService bs;

        public UserBooksController(IUserBookService bookService)
        {
            bs = bookService;
        }

        // GET: UserBooks
        public async  Task<IActionResult> Index()
        {
            string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";

            return View( await Task.Run(()=> bs.GetAll(UID)));
        }

        // GET: UserBooks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            
            string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";

            var userBook = await bs.GetDetails(id, UID);
            if (userBook == null)
            {
                return NotFound();
            }

            return View(userBook);
        }

        //// GET: UserBooks/Create
        //public IActionResult Create()
        //{
        //    ////////////////////////Edit Here Tomorrow ////////////////////////////
        //    // ViewData["BookId"] = new SelectList(_context.Books, "Id", "Author");
        //    // ViewData["BookId"] = new SelectList(bs., "Id", "Author");
        //    return View(new UserBook());
        //}

        //// POST: UserBooks/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("BookId,UserId,Rating")] UserBook userBook)
        //{
        //   // ViewBag.Tracks = trackRepositiory.GetAll();
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await Task.Run(()=>bs.Insert(userBook));
        //            return RedirectToAction("Index");
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError("", ex.Message);

        //            return View(userBook);
        //        }
        //    }
        //    return View(userBook);
        //}

        //GET: UserBooks/Edit/5
        //public async Task<IActionResult> Edit(int id)
        //{
        //    string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";


        //    var userBook = await bs.GetDetails(id,UID);
        //    if (userBook == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["BookId"] = new SelectList(bs.GetAll(UID).Result, "Id", "Author", userBook.BookId);
        //    return View(userBook);
        //}

        //// POST: UserBooks/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("BookId,UserId,Rating")] UserBook userBook)
        //{
        //    string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";

        //    if (id != userBook.BookId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            bs.Update(id,UID,userBook);
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UserBookExists(userBook.BookId,UID))
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
        //    ViewData["BookId"] = new SelectList(bs.GetAll(UID).Result, "Id", "Author", userBook.BookId);
        //    return View(userBook);
        //}

        //// GET: UserBooks/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
               string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";


            var userBook = await bs.GetDetails(id, UID);
               
            if (userBook == null)
            {
                return NotFound();
            }

            return View(userBook);
        }

        // POST: UserBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string UID = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "NAN";

            bs.Delete(id,UID);
            return RedirectToAction(nameof(Index));
        }

        private bool UserBookExists(int id,string id2)
        {
            if (bs.GetDetails(id, id2) == null)
                return false;
            return true;
        }
    }
}
