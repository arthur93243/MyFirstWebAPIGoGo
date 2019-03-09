using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;


namespace MyFirstWebAPIGoGo.Controllers
{
    public class HomeController : ApiController
    {
        public IHttpActionResult GetHome()
        {
            string str = "SELECT TOP 100 AddressID, AddressLine1, City, PostalCode FROM Person.Address";
            DataTable dttemp = (new DataCon()).GetDataTable(str);

            List<Models.Home> lsOBJ = new List<Models.Home>();
            lsOBJ = (new CommTolols()).TableConvertToObject<Models.Home>(dttemp);

            if (lsOBJ.Count == 0)
                return StatusCode(HttpStatusCode.NoContent);
            else
                return Ok(lsOBJ);
        }
    }
}
