using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFirstWebAPIGoGo.Models
{
    public class Home
    {
        public string AddressID { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
}