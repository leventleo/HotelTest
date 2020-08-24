using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Entity;
using Hotel.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
             

            return View();
        }

        [HttpPost]
        public ReservationViewModel ReservationInput(Reservations resevations)
        {
            var model = new ReservationViewModel();
            var rooms = _roomTypesRepository.TableNoTracking.Include(x=>x.RoomPrice).Include(x=>x.RoomPictures);
            var dayCount = resevations.CheckInDate.Subtract(resevations.CheckOutDate).TotalDays;
            var weekends = GetDaysBetween(resevations.CheckInDate, resevations.CheckOutDate).Where(d => d.DayOfWeek == DayOfWeek.Friday || d.DayOfWeek == DayOfWeek.Saturday);
            var normalDayCount = dayCount - weekends.Count();
            if(dayCount==normalDayCount)
            {
                foreach (var item in rooms)
                {
                    var room = new RoomViewModel
                    {
                        RoomPrice = Convert.ToDouble(((double)item.RoomPrice.LastOrDefault().RoomPrice1) * dayCount),
                        RoomType = item
                    };
                    model.RoomTypes.Add(room);
                }
            }
            else
            {
                foreach (var item in rooms)
                { var roomprice= ((double)item.RoomPrice.LastOrDefault().RoomPrice1);
                    var totalNormalDaysPrice = Convert.ToDouble(roomprice * normalDayCount);
                    var totalWeekendDaysPrice= Convert.ToDouble((roomprice*1.3) * weekends.Count());
                    var room = new RoomViewModel
                    {
                        RoomPrice = totalNormalDaysPrice+ totalWeekendDaysPrice,
                        RoomType = item
                    };
                    model.RoomTypes.Add(room);
                }
            }
                
            return model;
        }

        private IEnumerable<DateTime> GetDaysBetween(DateTime checkInDate, DateTime checkOutDate)          {
             
                for (DateTime i = checkInDate; i < checkOutDate; i = i.AddDays(1))
                {
                    yield return i;
                }            
        }
    }
}
