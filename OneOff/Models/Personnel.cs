using System;

namespace OneOff.Models
{
    public class Personnel
    {
        public DateTime Created { get; set; }

        public PersonnelStatus PersonnelStatus { get; set; }

        public DateTime StartDate { get; set; }

        public string Title { get; set; }

        public string Username { get; set; }
    }
}
