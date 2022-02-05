
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogGo.Repositories;
using DogGo.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace DogGo.Controllers
{
    public class DogsController : Controller
    {
        // GET: Dogs
       [Authorize]
        public ActionResult Index()
        {
            int ownerId = GetCurrentUserId();

            List<Dog> dogs = _dogRepo.GetDogsByOwnerId(ownerId);

            return View(dogs);
        }

        // GET: OwnersController/Details/5
        // GET: Owners/Details/5
        public ActionResult Details(int id)
        {
            Dog dog = _dogRepo.GetDogById(id);

            if (dog == null)
            {
                return NotFound();
            }

            return View(dog);
        }

        //// GET: OwnersController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //// POST: OwnersController/Create
       [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Dog dog)
        {
          try
          {
        // update the dogs OwnerId to the current user's Id 
                dog.OwnerId = GetCurrentUserId();

                _dogRepo.AddDog(dog);

                 return RedirectToAction("Index");
            }
              catch (Exception ex)
                 {
                     return View(dog);
                 }
          }

        // GET: OwnersController/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
           
            Dog dog = _dogRepo.GetDogById(id);

            if(dog.OwnerId != GetCurrentUserId())
            {
                return NotFound();
            }
            else if (dog == null)
            {
                return NotFound();
            }

            return View(dog);
        }

        // POST: Owners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Dog dog)
        {
            try
            {
                dog.OwnerId = GetCurrentUserId();
                _dogRepo.UpdateDog(dog);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return NotFound();
                //return View(dog);
            }
        }

        // GET: OwnersController/Delete/5
        // GET: Owners/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
           Dog dog = _dogRepo.GetDogById(id);

            return View(dog);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Dog dog)
        {
            try
            {
                _dogRepo.DeleteDog(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(dog);
            }
        }

        private readonly IDogRepository _dogRepo;

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public DogsController(IDogRepository dogRepository)
        {
            _dogRepo = dogRepository;
        }

        private int GetCurrentUserId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
