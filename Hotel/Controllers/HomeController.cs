using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.DataAccessLayer.ExtensionMetod;
using Hotel.Entity;
using Hotel.WebUI.Models;
using Microsoft.AspNetCore.Identity;
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
        public JsonResult ReservationInput(RoomSearchInput resevations)
        {
            var checkinDate = resevations.CheckInDate;
            var checkoutDate =Helpers.ChangeTime(resevations.CheckOutDate,14,0,0,0);
            var model = CalculateRooms(checkinDate, checkoutDate, resevations.Client,resevations.Bed);
            return Json(model);
        }

        private ReservationViewModel CalculateRooms(DateTime checkinDate, DateTime checkoutDate, int person,int bed)
        {
            if (checkinDate.Year == 1)
                checkinDate =  DateTime.Now;
            if (checkoutDate.Year == 1)
                checkoutDate =Helpers.ChangeTime(checkinDate.AddDays(1),14,0,0,0);

            var model = new ReservationViewModel();
            var rooms = _roomTypesRepository.TableNoTracking.Include(x => x.RoomPrice).Include(x => x.RoomPictures);
            int  dayCount =(int)Math.Floor(checkoutDate.Subtract(checkinDate).TotalHours /24);
            var days = GetDaysBetween(checkinDate, checkoutDate);            
            int normalDayCount = dayCount - days.Count();
            foreach (var item in rooms)
            { 
                var room = new RoomViewModel();               
                room.RoomType = item;
                room.Person = person;
                room.ExtraBed = person>0 ?bed:0;

                foreach (var day in days.Take(days.Count()-1).ToList())
                {
                   
                       var roomprice = item.RoomPrice.LastOrDefault().RoomPrice1;
                    if (day.DayOfWeek == DayOfWeek.Friday || day.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (person == 1)
                        {
                            roomprice = roomprice * Convert.ToDecimal(1.3);
                            roomprice = roomprice - (roomprice * Convert.ToDecimal(0.3));
                        }
                        else
                        {
                            if(bed==0)
                            {
                                roomprice = roomprice * Convert.ToDecimal(1.3);
                            }                            
                            else
                            {
                                roomprice = (roomprice * Convert.ToDecimal(1.3))*Convert.ToDecimal(1.2);                                
                            }

                        }

                        room.ReservationDays.Add(new ReservationDays { Date = day, Price = roomprice });
                    }
                    else
                    {
                        if (person == 1)
                        {
                            roomprice = roomprice - (roomprice * Convert.ToDecimal(0.3));
                        }
                        else
                        {
                            if (bed>0)
                            {
                                roomprice = roomprice * Convert.ToDecimal(1.2);
                            }                             
                        }

                        room.ReservationDays.Add(new ReservationDays { Date = day, Price = roomprice });
                    }

                }
               
                model.RoomTypes.Add(room);
            }

            return model;
        }

       

        private IEnumerable<DateTime> GetDaysBetween(DateTime checkInDate, DateTime checkOutDate)
        {

            for (DateTime i = checkInDate; i < checkOutDate; i = i.AddDays(1))
            { 
                yield return i;
            }
        }
    }
}
