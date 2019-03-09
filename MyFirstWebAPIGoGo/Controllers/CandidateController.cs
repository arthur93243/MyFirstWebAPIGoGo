using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyFirstWebAPIGoGo.Controllers
{
    public class CandidateController : ApiController
    {
        Models.Candidate[] Candidates = new Models.Candidate[] {
            new Models.Candidate { Name="peter", Id="a000000000", Age=30, Email="peter@gmail.com" },
            new Models.Candidate { Name="justin", Id="a11111111", Age=28, Email="justin@gmail.com" },
            new Models.Candidate { Name="terry", Id="a222222222", Age=32, Email="terry@gmail.com" }
        };

        //取得所有應徵者的資料清單
        public IEnumerable<Models.Candidate> GetAllCandidates()
        {
            return Candidates;
        }

        //取得特定名稱應徵者的資料
        public IHttpActionResult GetCandidate(string id)
        {
            var myCandidate = Candidates.FirstOrDefault((c) => c.Name == id);
            if (myCandidate == null)
                return StatusCode(HttpStatusCode.NoContent);
            else
                return Ok(myCandidate);
        }

    }
}
