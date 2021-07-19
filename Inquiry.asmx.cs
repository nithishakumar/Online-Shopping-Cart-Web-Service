using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Net;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using BusinessLogicLayerFunctions;

namespace TruckCarPartsWebService
{
    /// <summary>
    /// Contains Web Methods for Parts Inquiry
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]


    public class MessageHeader : SoapHeader
    {
        public string ResponseId;
        public string ResponseCode;
        public string ResponseDate;
        public string Username;
        public string Password;
        public void SetData(string psResponseId, string psResponseCode, string psResponseDate, 
                            string psUserName, string psPassWord)
        {
            ResponseId = psResponseId;
            ResponseCode = psResponseCode;
            ResponseDate = psResponseDate;
            Username = psUserName;
            Password = psPassWord;
        }

    }

    public class AuthHeader : SoapHeader {
        public string Username;
        public string Password;
    }


    public class WebService1 : System.Web.Services.WebService
    {
        public MessageHeader header = new MessageHeader();
        public AuthHeader AuthHeader = new AuthHeader();

        // Reading Username, Password, Connection String, and Schema from Web.config   
        public string csUsername = System.Configuration.ConfigurationManager.AppSettings["Username"];
        public string csPassword = System.Configuration.ConfigurationManager.AppSettings["Password"];
        public string csConnStr = System.Configuration.ConfigurationManager.AppSettings["ConnStr"];
        public string csschema = System.Configuration.ConfigurationManager.AppSettings["Schema"];

        public bool Authentication()
        {

            // Authentication:
            if (AuthHeader.Username != csUsername || AuthHeader.Password != csPassword)
            {
                header.SetData("FAIL", "401", DateTime.Now.ToString(), csUsername, csPassword);
                return false;

            }

            return true;
        }

        //MODIFIES: XML response
        //EFFECTS: Throws an exception with the given error message and code
        public XmlNode Throw_Exception(string pserrorDescription, int pserrorCode)

        {
            try
            {
                if (pserrorCode == 400)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, pserrorDescription);
                }
                else if (pserrorCode == 401)
                {
                    throw new HttpException((int)HttpStatusCode.Unauthorized, pserrorDescription);
                }
                else if (pserrorCode == 404)
                {
                    throw new HttpException((int)HttpStatusCode.NotFound, pserrorDescription);
                }
                else
                {
                    throw new HttpException((int)HttpStatusCode.InternalServerError, pserrorDescription);
                }

            }
            catch (HttpException lex)
            {
                Context.Response.StatusCode = lex.GetHttpCode();

                // Creating Soap Fault Message
                XmlDocument lxmldoc = new XmlDocument();
                XmlNode lxmlnode = lxmldoc.CreateNode(XmlNodeType.Element,
                                      "soap:Fault", "http://tempuri.org/");
                XmlNode lxmlfaultCode = lxmldoc.CreateNode(XmlNodeType.Element,
                                        "faultcode", "http://tempuri.org/");
                XmlText lxmlfaultCodeText = lxmldoc.CreateTextNode(pserrorCode.ToString());
                lxmlfaultCode.AppendChild(lxmlfaultCodeText);
                XmlNode lxmlfaultString = lxmldoc.CreateNode(XmlNodeType.Element,
                                       "faultstring", "http://tempuri.org/");
                XmlText lxmlfaultStringText = lxmldoc.CreateTextNode(pserrorDescription);
                lxmlfaultString.AppendChild(lxmlfaultStringText);

                lxmlnode.AppendChild(lxmlfaultCode);
                lxmlnode.AppendChild(lxmlfaultString);
                return lxmlnode;

            }
        }

        [WebMethod]
        [SoapHeader("header", Direction = SoapHeaderDirection.Out)]
        [SoapHeader("AuthHeader", Direction = SoapHeaderDirection.In, Required = true)]
        //REQUIRES: quantity to be an integer greater than 0
        //MODIFIES: XML response
        //EFFECTS: sends inquired part data in XML format
        public XmlNode Fetch_Parts(string pspartNo, string psqty)

        {
            if (!Authentication())
            {
                header.SetData("FAIL", "401", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Invalid Username or Password", 401);
            }

            bool lbpartExistenceIndicator = true;
            bool lbNum = true;
            bool lbsuccess = true;

            // Data Validation:
            int licheck;
            lbNum = int.TryParse(psqty, out licheck);
            if (psqty == "" || lbNum == false)
            {
                header.SetData("FAIL", "400", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Quantity has to be a positive integer", 400);
            }
            else if (licheck <= 0)
            {
                header.SetData("FAIL", "400", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Quantity should be greater than 0", 400);
            }
            else if (pspartNo == "")
            {
                header.SetData("FAIL", "400", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Part number cannot be blank", 400);
            }

            // Calling function from Business Logic Layer
            BLL lobj = new BLL();
            XmlNode lxmlnode = lobj.Fetch_Parts(pspartNo, licheck, csConnStr,
                                                     ref lbpartExistenceIndicator, ref lbsuccess);

            // If part number doesn't exist, throw an exception
            if (lbpartExistenceIndicator == false)
            {
                header.SetData("FAIL", "404", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Part number doesn't exist", 404);
            }

            if (!lbsuccess)
            {
                header.SetData("FAIL", "500", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Unable to fetch parts", 500);
            }


            header.SetData("SUCCESS", "200", DateTime.Now.ToString(), csUsername, csPassword);
            return lxmlnode;
        }


        [WebMethod]
        [SoapHeader("header", Direction = SoapHeaderDirection.Out)]
        [SoapHeader("AuthHeader", Direction = SoapHeaderDirection.In, Required = true)]
        //REQUIRES: Valid XmlNode
        //MODIFIES: XML response
        //EFFECTS: stores parts added to cart in patcart on SQL Server
        public XmlNode Add_To_Cart(XmlNode pxmlpartDetails, ref bool pbrollBackSuccessIndicator, ref bool pbsuccessIndicator)
        {
            if (!Authentication())
            {
                header.SetData("FAIL", "401", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Invalid Username or Password", 401);
            }

            // Calling function from Business Logic Layer
            BLL lobj = new BLL();
            XmlNode lxmlnode = lobj.Add_To_Cart(pxmlpartDetails, ref pbsuccessIndicator,
                                    ref pbrollBackSuccessIndicator, csConnStr, csschema);

            // If parts were added/updated successfully
            if (pbsuccessIndicator)
            {
                header.SetData("SUCCESS", "200", DateTime.Now.ToString(), csUsername, csPassword);
                return lxmlnode;
            }
            // If parts were not added/updated successfully but records were rolled back successfully
            else if (!pbsuccessIndicator && pbrollBackSuccessIndicator)

            {
                header.SetData("FAIL", "500", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Failed to add parts to cart but successfully rolled back all records", 500);
            }
            else
            {
                header.SetData("FAIL", "500", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Failed to add parts to cart and failed to roll back all records", 500);
            }

        }


        [WebMethod]
        [SoapHeader("header", Direction = SoapHeaderDirection.Out)]
        [SoapHeader("AuthHeader", Direction = SoapHeaderDirection.In, Required = true)]
        //MODIFIES: XML response, pbisCartEmpty
        //EFFECTS: displays parts in the cart
        public XmlNode Display_Cart(ref bool pbisCartEmpty)
        {
            if (!Authentication())
            {
                header.SetData("FAIL", "401", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Invalid Username or Password", 401);
            }

            bool lbsuccess = true;

            // Calling function from Business Logic Layer
            BLL lobj = new BLL();
            XmlNode lxmlnode = lobj.Display_Cart(AuthHeader.Username, csConnStr, ref pbisCartEmpty, ref lbsuccess);

            if (!lbsuccess)
            {
                header.SetData("FAIL", "500", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Unable to display cart", 500);
            }

            header.SetData("SUCCESS", "200", DateTime.Now.ToString(), csUsername, csPassword);
            return lxmlnode;
        }


        [WebMethod]
        [SoapHeader("header", Direction = SoapHeaderDirection.Out)]
        [SoapHeader("AuthHeader", Direction = SoapHeaderDirection.In, Required = true)]
        //REQUIRES: Valid XmlNode
        //MODIFIES: XML response
        //EFFECTS: deletes parts from cart
        public XmlNode Delete_From_Cart(XmlNode pxmlpartDetails, ref bool pbrollBackSuccessIndicator,
                                        ref bool pbsuccessIndicator)
        {
            if (!Authentication())
            {
                header.SetData("FAIL", "401", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Invalid Username or Password", 401);
            }

            // Calling function from Business Logic Layer
            BLL lobj = new BLL();
            XmlNode lxmlnode = lobj.Delete_From_Cart(pxmlpartDetails, csConnStr,
                        ref pbsuccessIndicator, ref pbrollBackSuccessIndicator);

            // If parts were added/updated successfully
            if (pbsuccessIndicator)
            {
                header.SetData("SUCCESS", "200", DateTime.Now.ToString(), csUsername, csPassword);
                return lxmlnode;
            }
            // If parts were not added/updated successfully but records were rolled back successfully
            else if (!pbsuccessIndicator && pbrollBackSuccessIndicator)

            {
                header.SetData("FAIL", "500", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Failed to delete parts from cart but successfully rolled back all records", 500);
            }
            else
            {
                header.SetData("FAIL", "500", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Failed to delete parts from cart and failed to roll back all records", 500);
            }

        }

        [WebMethod]
        [SoapHeader("header", Direction = SoapHeaderDirection.Out)]
        [SoapHeader("AuthHeader", Direction = SoapHeaderDirection.In, Required = true)]
        //REQUIRES: Valid XmlNode
        //MODIFIES: XML response
        //EFFECTS: uploads files to PATUSERFILES on SQL Server
        public XmlNode Upload_File(XmlNode pxmlfileDetails, ref Byte[] fileData)
        {
            if (!Authentication())
            {
                header.SetData("FAIL", "401", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Invalid Username or Password", 401);
            }

            bool lbsuccessIndicator = true;

            // Calling function from Business Logic Layer
            BLL lobj = new BLL();
            XmlNode lxmlnode = lobj.Upload_File(pxmlfileDetails, csConnStr, csschema, ref lbsuccessIndicator, ref fileData);

            if(!lbsuccessIndicator)
            {
                header.SetData("FAIL", "500", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Unable to upload file", 500);
            }

            header.SetData("SUCCESS", "200", DateTime.Now.ToString(), csUsername, csPassword);
            return lxmlnode;
        }

        [WebMethod]
        [SoapHeader("header", Direction = SoapHeaderDirection.Out)]
        [SoapHeader("AuthHeader", Direction = SoapHeaderDirection.In, Required = true)]
        //REQUIRES: Valid XmlNode
        //MODIFIES: XML response
        //EFFECTS: gets the file last uploaded from PATUSERFILES
        public XmlNode View_Uploaded_File(ref Byte[] fileData)
        {
            if (!Authentication())
            {
                header.SetData("FAIL", "401", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("Invalid Username or Password", 401);
            }

            bool lbsuccessIndicator = true;
            bool lbisEmpty = true;

            // Calling function from Business Logic Layer
            BLL lobj = new BLL();
            XmlNode lxmlnode = lobj.View_Uploaded_File(csConnStr, ref lbsuccessIndicator, ref lbisEmpty, ref fileData);

            if (!lbsuccessIndicator)
            {
                header.SetData("FAIL", "500", DateTime.Now.ToString(), csUsername, csPassword);
                return Throw_Exception("No files uploaded", 500);
            }

            header.SetData("SUCCESS", "200", DateTime.Now.ToString(), csUsername, csPassword);
            return lxmlnode;
        }


    }
}
