using System;
using System.Collections.Generic;
using System.Text;

namespace Trading.DAO
{
    public class Courier
    {
        public int Id { get; set; }

        public string AWBNumber { get; set; }

        public DateTime ReceivedFrom { get; set; }

        public DateTime ReceivedOn { get; set; }

        public string Subject { get; set; } = " - ";

        public string Status { get; set; } = " - ";
    }
}
