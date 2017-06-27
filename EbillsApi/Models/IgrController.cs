using EbillsApi.Models;
using EbillsApi.Models.Repository;
using NIBBSRestServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EbillsApi.Controllers
{
    public class IgrController : ApiController
    {
        private ValidationRequest vResponse;

        //getting list of IGR
        [HttpGet]
        public IHttpActionResult GetIgr()
        {
            IgrRepository pp = new IgrRepository();

            return Ok(pp.GetIgrs());

        }

        //Posting request from Nibss
        [HttpPost]
        public IHttpActionResult PostRequest([FromBody]EbillsRequest value)
        {
            vResponse = value.vRequest;
            return GetResponse(vResponse);
        }

        //priavte class to get response
        private IHttpActionResult GetResponse(ValidationRequest vResponse)
        {
            vResponse.Step = vResponse.Step;
            vResponse.ProductName = vResponse.ProductName;
            vResponse.BillerID = vResponse.BillerID;
            vResponse.BillerName = vResponse.BillerName;
            vResponse.ProductName = vResponse.ProductName;
            vResponse.Param = vResponse.Param;

            return Ok(vResponse);
        }
    }
}
