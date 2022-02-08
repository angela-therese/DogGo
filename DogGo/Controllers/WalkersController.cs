using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogGo.Repositories;
using DogGo.Models;
using DogGo.Models.ViewModels;
using System.Security.Claims;

namespace DogGo.Controllers
{
    public class WalkersController : Controller
    {
        // GET: Walkers
        public ActionResult Index()
        {
            int currentOwnerId = GetCurrentUserId(); //Get ID of currently logged-in user
            List<Walker> walkers = new List<Walker>(); //Instantiate new list
            if (currentOwnerId != 0) //If the userId is not 0
            {
                Owner currentOwner = _ownerRepo.GetOwnerById(currentOwnerId); //Gets the current owner object using the currentownerid
                walkers = _walkerRepo.GetWalkersInNeighborhood(currentOwner.NeighborhoodId); //Gets walkers based on the owner's neigbhorhood Id 
            }
            else //if the userId is 0
            {
                walkers = _walkerRepo.GetAllWalkers(); // Gets all walkers if there is no current user id
            }

            return View(walkers);
        }

        // GET: WalkersController/Details/5
        // GET: Walkers/Details/5
        public ActionResult Details(int id)
        {
            Walker walker = _walkerRepo.GetWalkerById(id);
            List<Walk> walks = walker.Walks;
           

            WalkerProfileViewModel vm = new WalkerProfileViewModel()
            {
                Walker = walker,
                Walks = walks,
               
            };
            return View(vm);

           
        }

        // GET: WalkersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WalkersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: WalkersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: WalkersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: WalkersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private readonly IWalkerRepository _walkerRepo;
        private readonly IDogRepository _dogRepo;
        private readonly IOwnerRepository _ownerRepo;
        private readonly INeighborhoodRepository _neighborhoodRepo;
       
       

        // ASP.NET will give us an instance of our Walker Repository. This is called "Dependency Injection"
        public WalkersController(IWalkerRepository walkerRepository, IDogRepository dogRepository, IOwnerRepository ownerRepository, INeighborhoodRepository neighborhoodRepository )
        {
            _walkerRepo = walkerRepository;
            _dogRepo = dogRepository;
            _ownerRepo = ownerRepository;
            _neighborhoodRepo = neighborhoodRepository;
            
        }

        private int GetCurrentUserId()
        {
            string id= User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != null)
            { 
                return int.Parse(id); 
            }
           else
            {
                return 0;  
            }
        }
    }
}
