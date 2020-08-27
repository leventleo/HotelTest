using Hotel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.WebUI.Models
{
    public class ReservationViewModel
    {
        public ReservationViewModel()
        {
            RoomTypes = new List<RoomViewModel>();
        }
        public List<RoomViewModel> RoomTypes { get; set; }
    }

    public class RoomViewModel
    {
        public RoomViewModel()
        {
            RoomType = new RoomTypes();
            ReservationDays = new List<ReservationDays>();
           
             
        }
        public RoomTypes RoomType { get; set; }
        public List<ReservationDays>  ReservationDays { get; set; }
        public int  Person { get; set; }
        public int  ExtraBed { get; set; }
    }

    public class ReservationDays
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }

    public partial class RoomSearchInput
    {
        
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Client { get; set; }
        public int Bed { get; set; }
       
    }
}
