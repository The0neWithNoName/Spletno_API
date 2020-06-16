using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SpletnoProject.Controllers
{
    public class WebappController : ApiController
    {

        public string Get()
        {
            return "Welcome To this Api for Web App purposes";
        }


        public string Get(string command)
        {
           

            return "Error";
        }

    }
}
