using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFirstWebAPIGoGo.Models
{
    public class Candidate
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }
}