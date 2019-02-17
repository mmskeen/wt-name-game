using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTNameGame
{
    public class Profile
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Slug { get; set; }

        public string JobTitle { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Headshot Headshot { get; set; }

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

    }
}
