using EbillsApi.Models;
using EbillsApi.Models.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;

namespace EbillsApi.Controllers
{
    public class IgrController : ApiController
    {
        private ValidationRequest vResponse = new ValidationRequest();
        private ValidationResponse sResponse = new ValidationResponse();
        private string name;
        private string payerid;
        private string ercasBillerId;
        private string mda;
        private string amount;

        public UtilityClass utility;

        //getting list of IGR
        [HttpGet]
        public IHttpActionResult GetIgr()
        {
            IgrRepository pp = new IgrRepository();

            return Ok(pp.GetIgrs());

        }

        //Posting request from Nibss
        [HttpPost]
        public HttpResponseMessage PostRequest(HttpRequestMessage value)
        {
            var doc = new XmlDocument();
            doc.Load(value.Content.ReadAsStreamAsync().Result);

            var obj = JsonConvert.SerializeXmlNode(doc);
            vResponse = JObject.Parse(obj)["ValidationRequest"].ToObject<ValidationRequest>();

            utility = new UtilityClass(vResponse);

            //checking if the product is non-tax
            if (vResponse.ProductName.Equals("Non-Tax"))
                return GetNonTax(vResponse);

            //check if product is tax
            //if ()
                //sResponse = GetNonTax();

            //check if product is remittance
           // if ()
                //sResponse = GetNonTax();

            //check if product is invoice
            //if ()
                //sResponse = GetNonTax();

            //check if product is refcode
            //if ()
                //sResponse = GetNonTax();

            return GetHttpMsg(vResponse, "ProductName paramter missing");
        }

        //getting NonTax
        private HttpResponseMessage GetNonTax(ValidationRequest vResponse)
        {
            ParamToArray(vResponse.Param);

            //checking if step is 1
            if (vResponse.Step.Equals(1))
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(payerid))
                {
                    return GetHttpMsg(vResponse, "Name or PayerId field can not be empty");
                }

                sResponse = utility.GetMdaResponse(vResponse, 2, ercasBillerId);
            }

            //checking if step is 2
            if (vResponse.Step.Equals(2))
            {
                if (string.IsNullOrEmpty(mda) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(payerid) || string.IsNullOrEmpty(ercasBillerId))
                {
                    return GetHttpMsg(vResponse, "Parameter Missing");
                }

                sResponse = utility.GetSubheadResponse(vResponse, 3, mda);

            }

            //checking if step is 3
            if (vResponse.Step.Equals(3))
            {
                if (string.IsNullOrEmpty(amount))
                {
                    return GetHttpMsg(vResponse, "Amount field can not be empty");
                }

                sResponse = utility.GetResponse(vResponse, 4);
            }

            return GetHttpMsg(vResponse);

        }




        //get HTTPerrorMessage
        private HttpResponseMessage GetHttpMsg(ValidationRequest vResponse, string msg=null)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return Request.CreateResponse<ValidationResponse>(HttpStatusCode.OK, sResponse);
            }
            else
            {
                return Request.CreateResponse<ValidationResponse>(HttpStatusCode.BadRequest, utility.GetErrorResponse(vResponse, vResponse.Step, msg, "400"));
                
            }
            
        }

        //converting param to array
        private void ParamToArray(IList<Param> sList)
        {
            for (int i = 0; i < sList.Count; i++)
            {
                if (sList[i].key.Equals("name"))
                {
                    name = sList[i].value;
                }

                if (sList[i].key.Equals("payerid"))
                {
                    payerid = sList[i].value;
                }

                if (sList[i].key.Equals("ercasBillerId")) 
                {
                    ercasBillerId = sList[i].value;
                }

                if (sList[i].key.Equals("mda")) 
                {
                    mda = sList[i].value;
                }

                if (sList[i].key.Equals("amount")) 
                {
                    amount = sList[i].value;
                }
            }
        }
    }
}
