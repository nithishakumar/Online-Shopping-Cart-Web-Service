using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using EInqWebDTNonTxnClass;
using EInqWebDTTxnClass;


namespace BusinessLogicLayerFunctions
{
    public class BLL

    {
        //MODIFIES: XML Response , pbpartExistenceIndicator, pbsuccess
        //EFFECTS: connects to SQL Server to provide part details
        public XmlNode Fetch_Parts(string lspartNo, int liqty, string lsConnStr, 
                                       ref bool pbpartExistenceIndicator, ref bool pbsuccess)
        {
            // Validated data is now passed to Data Access Layer
            EInqWebDTNonTxn lobj = new EInqWebDTNonTxn();
            return lobj.Fetch_Parts(lspartNo, liqty, lsConnStr, ref pbpartExistenceIndicator, ref pbsuccess);
        }

        //MODIFIES: XML response, pbrollBackSuccessIndicator, pbsuccessIndicator
        //EFFECTS: stores parts added to cart in patcart on SQL Server
        public XmlNode Add_To_Cart(XmlNode pxmlpartDetails, ref bool pbsuccessIndiactor, 
                        ref bool pbrollBackSuccessIndicator, string psConnStr, string psschema)
        {
            // XmlNode is now passed to Data Access Layer
            EInqWebDTTxn lobj = new EInqWebDTTxn();
            return lobj.Add_To_Cart(pxmlpartDetails, ref pbsuccessIndiactor,
                                    ref pbrollBackSuccessIndicator, psConnStr, psschema);
        }

     
        //MODIFIES: XML response, pbIsCartEmpty, pbsuccess
        //EFFECTS: displays existing parts in the cart
        public XmlNode Display_Cart(string psusername, string psConnStr, ref bool pbisCartEmpty, ref bool pbsuccess)
        {
            // XmlNode is now passed to Data Access Layer
            EInqWebDTNonTxn lobj = new EInqWebDTNonTxn();
            return lobj.Display_Cart(psusername, psConnStr, ref pbisCartEmpty, ref pbsuccess);
        }


        //MODIFIES: XML response, pbrollBackSuccessIndicator, pbsuccessIndicator
        //EFFECTS: deletes parts from patcart on SQL Server
        public XmlNode Delete_From_Cart(XmlNode pxmlpartDetails, string psConnStr,
                        ref bool pbsuccessIndicator, ref bool pbrollBackSuccessIndicator)
        {
            // XmlNode is now passed to Data Access Layer
            EInqWebDTTxn lobj = new EInqWebDTTxn();
            return lobj.Delete_From_Cart(pxmlpartDetails, psConnStr,
                        ref pbsuccessIndicator, ref pbrollBackSuccessIndicator);
        }


        //MODIFIES: XML response, pbsuccessIndicator
        //EFFECTS: uploads files to PATUSERFILES on SQL Server
        public XmlNode Upload_File(XmlNode pxmlfileDetails, string psConnStr, string psschema, ref bool pbsuccessIndicator, ref Byte[] fileData)
        {
            // XmlNode is now passed to Data Access Layer
            EInqWebDTTxn lobj = new EInqWebDTTxn();
            return lobj.Upload_File(pxmlfileDetails, psConnStr,
                       psschema, ref pbsuccessIndicator, ref fileData);
        }


        //MODIFIES: XML response, pbsuccessIndicator
        //EFFECTS: gets the last uploaded file from PATUSERFILES
        public XmlNode View_Uploaded_File(string psConnStr, ref bool pbsuccessIndicator, ref bool isEmpty, ref Byte[] fileData)
        {
            // XmlNode is now passed to Data Access Layer
            EInqWebDTNonTxn lobj = new EInqWebDTNonTxn();
            return lobj.View_Uploaded_File(psConnStr, ref pbsuccessIndicator, ref isEmpty, ref fileData);
        }




    }
}
