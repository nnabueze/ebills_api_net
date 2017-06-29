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

namespace EbillsApi.Models
{
    public class UtilityClass : ApiController
    {
        private ValidationRequest vResponse = new ValidationRequest();
        private ValidationResponse sResponse = new ValidationResponse();
        private Mda mda;
        private Field FieldItem;
        private Igr igr = new Igr();
        private IgrRepository p = new IgrRepository();

        public UtilityClass(ValidationRequest xRequest)
        {
            vResponse = xRequest;
        }


         //return field
        //passing the biller id
        public Field GetMdaField(String xtring)
        {
            FieldItem = new Field();
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

        public Field GetSubheadField(String xtring)
        {
            FieldItem = new Field();
            mda = p.GetMda(xtring);
            

            FieldItem.Name = "Subhead";
            FieldItem.Type = "List";
            FieldItem.Required = false;
            FieldItem.Readonly = false;
            FieldItem.MaxLength = 0;
            FieldItem.Order = 0;
            FieldItem.RequiredInNextStep = true;
            FieldItem.AmountField = false;
            FieldItem.Item = p.ListSubhead(mda.Id);

            return FieldItem;

        }

        //priavte class to get response
        public ValidationResponse GetMdaResponse(ValidationRequest vResponse, int num, string billerid)
        {
            sResponse.BillerName = vResponse.BillerName;
            sResponse.BillerID = vResponse.BillerID;
            sResponse.ProductName = vResponse.ProductName;
            sResponse.NextStep = num;
            sResponse.ResponseCode = "00";
            sResponse.ResponseMessage = "Successful";
            sResponse.Param = vResponse.Param;
            sResponse.field = GetMdaField(billerid);

            return sResponse;
        }

        //priavte class to get response
        public ValidationResponse GetSubheadResponse(ValidationRequest vResponse, int num, string billerid)
        {
            sResponse.BillerName = vResponse.BillerName;
            sResponse.BillerID = vResponse.BillerID;
            sResponse.ProductName = vResponse.ProductName;
            sResponse.NextStep = num;
            sResponse.ResponseCode = "00";
            sResponse.ResponseMessage = "Successful";
            sResponse.Param = vResponse.Param;
            sResponse.field = GetSubheadField(billerid);

            return sResponse;
        }

        public ValidationResponse GetResponse(ValidationRequest vResponse, int num)
        {
            sResponse.BillerName = vResponse.BillerName;
            sResponse.BillerID = vResponse.BillerID;
            sResponse.ProductName = vResponse.ProductName;
            sResponse.NextStep = num;
            sResponse.ResponseCode = "00";
            sResponse.ResponseMessage = "Successful";
            sResponse.Param = vResponse.Param;

            return sResponse;
        }

        //getting error response
        public ValidationResponse GetErrorResponse(ValidationRequest vResponse, int num, string msg, string code)
        {
            sResponse.ProductName = vResponse.ProductName;
            sResponse.NextStep = num;
            sResponse.ResponseCode = code;
            sResponse.ResponseMessage = msg;

            return sResponse;
        }

        //get HTTPerrorMessage
        public HttpResponseMessage GetHttpMsg(ValidationRequest vResponse, string msg=null)
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


    }
}