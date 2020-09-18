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
        private readonly IRepository<Reservations> _reservationsRepository;

        public HomeController(IRepository<RoomTypes> roomTypesRepository, IRepository<Reservations> reservationsRepository)
        {
            _roomTypesRepository = roomTypesRepository;
            _reservationsRepository = reservationsRepository;
        }
        public IActionResult Index()
        {


            return View();
        }

        public IActionResult GetReservations()
        {


            return View();
        }

        [HttpPost]
        public JsonResult ReservationInput(RoomSearchInput resevations)
        {
            var checkinDate = resevations.CheckInDate;
            var checkoutDate = Helpers.ChangeTime(resevations.CheckOutDate, 14, 0, 0, 0);
            var model = CalculateRooms(checkinDate, checkoutDate, resevations.Client, resevations.Bed);
            return Json(model);
        }

        [HttpPost]
        public JsonResult Reserved(RoomSearchInput resevations, Client client)
        {


            decimal TotalPrice = CalculateSingleRooms(resevations);
            if (resevations.CheckInDate.Year == 1)
                resevations.CheckInDate = DateTime.Now;
            if (resevations.CheckOutDate.Year == 1)
                resevations.CheckOutDate = Helpers.ChangeTime(resevations.CheckInDate.AddDays(1), 14, 0, 0, 0);
            var reserveEntity = new Reservations
            {
                FranchPad = resevations.Bed,
                CheckInDate = resevations.CheckInDate,
                CheckOutDate = resevations.CheckOutDate,
                ClientFullName = client.Name + " " + client.Surname,
                ClientNationalNumber = "11111111111",
                Person = resevations.Client,
                RoomTypeId = resevations.RoomType,
                TotalPrice = TotalPrice,
                CreateDate = DateTime.Now
            };

            _reservationsRepository.Insert(reserveEntity);

            return Json(true);
        }
        private ReservationViewModel CalculateRooms(DateTime checkinDate, DateTime checkoutDate, int person, int bed)
        {
            if (checkinDate.Year == 1)
                checkinDate = DateTime.Now;
            if (checkoutDate.Year == 1)
                checkoutDate = Helpers.ChangeTime(checkinDate.AddDays(1), 14, 0, 0, 0);

            var model = new ReservationViewModel();
            var rooms = _roomTypesRepository.TableNoTracking.Include(x => x.RoomPrice).Include(x => x.RoomPictures);
            int dayCount = (int)Math.Floor(checkoutDate.Subtract(checkinDate).TotalHours / 24);
            var days = GetDaysBetween(checkinDate, checkoutDate);
            int normalDayCount = dayCount - days.Count();
            foreach (var item in rooms)
            {
                var room = new RoomViewModel();
                room.RoomType = item;
                room.Person = person;
                room.ExtraBed = person > 0 ? bed : 0;

                foreach (var day in days.Take(days.Count() - 1).ToList())
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
                            if (bed == 0)
                            {
                                roomprice = roomprice * Convert.ToDecimal(1.3);
                            }
                            else
                            {
                                roomprice = (roomprice * Convert.ToDecimal(1.3)) * Convert.ToDecimal(1.2);
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
                            if (bed > 0)
                            {
                                roomprice = roomprice * Convert.ToDecimal(1.2);
                            }
                        }

                        room.ReservationDays.Add(new ReservationDays { Date = day, Price = roomprice });
                    }

                }
                room.TotalPrice = room.ReservationDays.Sum(s => s.Price);
                model.RoomTypes.Add(room);

            }
            model.RoomTypes = model.RoomTypes.OrderBy(o => o.TotalPrice).ToList();
            return model;
        }

        private decimal CalculateSingleRooms(RoomSearchInput roomSearch)
        {
            if (roomSearch.CheckInDate.Year == 1)
                roomSearch.CheckInDate = DateTime.Now;
            if (roomSearch.CheckOutDate.Year == 1)
                roomSearch.CheckOutDate = Helpers.ChangeTime(roomSearch.CheckInDate.AddDays(1), 14, 0, 0, 0);

            var model = new ReservationViewModel();
            var rooms = _roomTypesRepository.TableNoTracking.Include(x => x.RoomPrice).ToList();
            int dayCount = (int)Math.Floor(roomSearch.CheckOutDate.Subtract(roomSearch.CheckInDate).TotalHours / 24);
            var days = GetDaysBetween(roomSearch.CheckInDate, roomSearch.CheckOutDate);
            int normalDayCount = dayCount - days.Count();
            foreach (var item in rooms.Where(x => x.id == roomSearch.RoomType).ToList())
            {
                var room = new RoomViewModel();
                room.RoomType = item;
                room.Person = roomSearch.Client;
                room.ExtraBed = roomSearch.Client > 0 ? roomSearch.Bed : 0;

                foreach (var day in days.Take(days.Count() - 1).ToList())
                {

                    var roomprice = item.RoomPrice.LastOrDefault().RoomPrice1;
                    if (day.DayOfWeek == DayOfWeek.Friday || day.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (roomSearch.Client == 1)
                        {
                            roomprice = roomprice * Convert.ToDecimal(1.3);
                            roomprice = roomprice - (roomprice * Convert.ToDecimal(0.3));
                        }
                        else
                        {
                            if (roomSearch.Bed == 0)
                            {
                                roomprice = roomprice * Convert.ToDecimal(1.3);
                            }
                            else
                            {
                                roomprice = (roomprice * Convert.ToDecimal(1.3)) * Convert.ToDecimal(1.2);
                            }

                        }

                        room.ReservationDays.Add(new ReservationDays { Date = day, Price = roomprice });
                    }
                    else
                    {
                        if (roomSearch.Client == 1)
                        {
                            roomprice = roomprice - (roomprice * Convert.ToDecimal(0.3));
                        }
                        else
                        {
                            if (roomSearch.Bed > 0)
                            {
                                roomprice = roomprice * Convert.ToDecimal(1.2);
                            }
                        }

                        room.ReservationDays.Add(new ReservationDays { Date = day, Price = roomprice });
                    }

                }
                room.TotalPrice = room.ReservationDays.Sum(s => s.Price);
                model.RoomTypes.Add(room);

            }

            return model.RoomTypes.FirstOrDefault().TotalPrice;
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
