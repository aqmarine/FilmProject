using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FilmProject.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;

namespace FilmProject.Controllers
{

    public class HomeController : Controller
    {
        private DBEntities db = new DBEntities();

        // GET: Home
        public ActionResult Index()
        {
            return View(db.Films.ToList());
        }

        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Film film = db.Films.Find(id);
            if (film == null)
            {
                return HttpNotFound();
            }
            return View(film);
        }

        // GET: Home/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "FilmId,Name,Description,Year,Director,Picture")] Film film,
                                            HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {
                if (image.ContentLength > 0 && image != null)
                {
                    film.Picture = new byte[image.ContentLength];
                    image.InputStream.Read(film.Picture, 0, image.ContentLength);
                }
                film.UserId = User.Identity.GetUserId();
                db.Films.Add(film);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(film);
        }

        // GET: Home/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Film film = db.Films.Find(id);
            if (User.Identity.GetUserId() != film.UserId)
            {
                return RedirectToAction("Index"); ;
            }
            if (film == null)
            {
                return HttpNotFound();
            }
            return View(film);
        }

        // POST: Home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "FilmId,Name,Description,Year,Director,Picture")] Film newEditFilm, 
                                        HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    
                    newEditFilm.Picture = new byte[image.ContentLength];
                    image.InputStream.Read(newEditFilm.Picture, 0, image.ContentLength);
                }
                else
                {
                    var editFilm = db.Films.Find(newEditFilm.FilmId);
                    newEditFilm.Picture = editFilm.Picture;
                }
                newEditFilm.UserId = User.Identity.GetUserId();
                db.Set<Film>().AddOrUpdate(newEditFilm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newEditFilm);
        }

        // GET: Home/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Film film = db.Films.Find(id);
            if (film == null)
            {
                return HttpNotFound();
            }
            return View(film);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            Film film = db.Films.Find(id);
            db.Films.Remove(film);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
