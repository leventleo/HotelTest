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
        }
        public RoomTypes RoomType { get; set; }
        public double RoomPrice { get; set; }
    }
}
