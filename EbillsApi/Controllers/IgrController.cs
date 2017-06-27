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
        private Field FieldItem;
        private Igr igr;
        private IgrRepository p;

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
            //checking if step is 1
            if (vResponse.Step.Equals(1))
            {
                ParamToArray(vResponse.Param);

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(payerid))
                {
                    return GetHttpMsg(vResponse, "Name or PayerId field can not be empty");
                }

                sResponse = GetResponse(vResponse, 2);
            }

            //checking if step is 2
            if (vResponse.Step.Equals(2))
            {
                ParamToArray(vResponse.Param);
                if (string.IsNullOrEmpty(mda) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(payerid) || string.IsNullOrEmpty(ercasBillerId))
                {
                    return GetHttpMsg(vResponse, "Parameter Missing");
                }


            }

            //checking if step is 3

            return GetHttpMsg(vResponse);

        }


        //return field
        //passing the biller id
        public Field GetField(String xtring)
        {
            FieldItem = new Field();
            p = new IgrRepository();
            igr = p.getIgr(xtring);

            if (igr.Igr_abbre.Equals("ERCAS_BAUCHI"))
            {
                FieldItem.Name = "Lga";
            }
            else
            {
                FieldItem.Name = "Mda";
            }

            FieldItem.Type = "List";
            FieldItem.Required = false;
            FieldItem.Readonly = false;
            FieldItem.MaxLength = 0;
            FieldItem.Order = 0;
            FieldItem.RequiredInNextStep = true;
            FieldItem.AmountField = false;
            FieldItem.Item = p.ListMda(xtring);

            return FieldItem;

        }


        //priavte class to get response
        private ValidationResponse GetResponse(ValidationRequest vResponse, int num)
        {
            sResponse.BillerName = vResponse.BillerName;
            sResponse.BillerID = vResponse.BillerID;
            sResponse.ProductName = vResponse.ProductName;
            sResponse.NextStep = num;
            sResponse.ResponseCode = "00";
            sResponse.ResponseMessage = "Successful";
            sResponse.Param = vResponse.Param;
            sResponse.field = GetField(ercasBillerId);

            return sResponse;
        }

        //getting error response
        private ValidationResponse GetErrorResponse(ValidationRequest vResponse, int num, string msg, string code)
        {
            sResponse.ProductName = vResponse.ProductName;
            sResponse.NextStep = num;
            sResponse.ResponseCode = code;
            sResponse.ResponseMessage = msg;

            return sResponse;
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
                return Request.CreateResponse<ValidationResponse>(HttpStatusCode.BadRequest, GetErrorResponse(vResponse, vResponse.Step, msg, "400"));
                
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
            }
        }
    }
}
