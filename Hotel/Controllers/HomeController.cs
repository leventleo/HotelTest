using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Entity;
using Hotel.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<RoomTypes> _roomTypesRepository;

        public HomeController(IRepository<RoomTypes> roomTypesRepository)
        {
            _roomTypesRepository = roomTypesRepository;
        }
        public IActionResult Index()
        {
            var model = new ReservationViewModel
            {
                RoomTypes = _roomTypesRepository.TableNoTracking.ToList()
            };


            return View(model);
        }

        [HttpPost]
        public IActionResult ReservationInput(Reservations resevations)
        {
            return View();
        }

    }
}
