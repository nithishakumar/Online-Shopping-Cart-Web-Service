using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Net;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using System.Text;
using System.Drawing;



namespace TruckCarPartsWebPage
{

    public partial class Inquiry : System.Web.UI.Page
    {
        public DataTable cdtpartsInquiry = new DataTable();
        public string cspartNo, csqty;
        public XmlNode cxmlnode;
        public localhost.AuthHeader cAuthHeaderCredentials = new localhost.AuthHeader();
        public DataTable cdtpartsAdded = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Display items currently in the cart
            if (!IsPostBack)
            {
                Display_Cart();
            }
        }


        //MODIFIES: pwebServiceObject
        //EFFECTS: sets username and password for the given web service object
        public void Set_Credentials(ref localhost.WebService1 pwebServiceObject)
        {
            // Authentication:
            cAuthHeaderCredentials.Username = "ANTOZ02053";
            cAuthHeaderCredentials.Password = "Parts1";
            pwebServiceObject.AuthHeaderValue = cAuthHeaderCredentials;
        }


        // Check Button Click:
        protected void Button1_Click(object sender, EventArgs e)

        {

            // Storing quantity and partNo
            csqty = TextBox2.Text.ToString();
            cspartNo = TextBox1.Text.ToString();

            try
            {
                // Creating an object of the web service and setting username/pass
                localhost.WebService1 lwebgetData = new localhost.WebService1();
                Set_Credentials(ref lwebgetData);

                // Adding columns to the data table
                cdtpartsInquiry.Columns.Add("Part");
                cdtpartsInquiry.Columns.Add("Inq Part");
                cdtpartsInquiry.Columns.Add("Mfg Part");
                cdtpartsInquiry.Columns.Add("Brd desc");
                cdtpartsInquiry.Columns.Add("Req Qty");
                cdtpartsInquiry.Columns.Add("Crtn Qty");
                cdtpartsInquiry.Columns.Add("Rate");
                cdtpartsInquiry.Columns.Add("Soh Cpd");
                cdtpartsInquiry.Columns.Add("Soh Brch");
                cdtpartsInquiry.Columns.Add("Brand");
                cdtpartsInquiry.Columns.Add("Desc");
                XmlNode lxmlchildNode;
                cxmlnode = lwebgetData.Fetch_Parts(cspartNo, csqty);
                int linumNodes = cxmlnode.ChildNodes.Count;

                for (int i = 0; i < linumNodes; i++)
                {
                    // Reading from the web service's response to the data table
                    DataRow lrow = cdtpartsInquiry.NewRow();
                    lxmlchildNode = cxmlnode.ChildNodes[i];
                    lrow[0] = lxmlchildNode["Part"].InnerXml;
                    lrow[1] = lxmlchildNode["InqPart"].InnerXml;
                    lrow[2] = lxmlchildNode["MfgPart"].InnerXml;
                    lrow[3] = lxmlchildNode["BrdDesc"].InnerXml;
                    lrow[4] = lxmlchildNode["ReqQty"].InnerXml;
                    lrow[5] = lxmlchildNode["CrtnQty"].InnerXml;
                    lrow[6] = lxmlchildNode["Rate"].InnerXml;
                    lrow[7] = lxmlchildNode["SohCpd"].InnerXml;
                    lrow[8] = lxmlchildNode["SohBrch"].InnerXml;
                    lrow[9] = lxmlchildNode["Brand"].InnerXml;
                    lrow[10] = lxmlchildNode["Desc"].InnerXml;
                    cdtpartsInquiry.Rows.Add(lrow);

                }

                // Binding data to the grid view
                GridView1.DataSource = cdtpartsInquiry;
                GridView1.DataBind();
            }
            catch (Exception lex)
            { 
                if (lex.Message.ToString() == "Unable to connect to the remote server")
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                         "alert('Unable to connect to the web service')", true);
                    return;
                }
                else if (lex.Message.ToString() == "The request failed with HTTP status 401: Unauthorized.")
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                           "alert('Unauthorized Request')", true);
                    return;
                }

                // Checking if quantity is a positive integer
                int licheck;
                bool lbNum = int.TryParse(csqty, out licheck);

                if (csqty == "" || lbNum == false)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                            "alert('Quantity should be a positive integer')", true);
                    return;
                }
                else if (cspartNo == "")
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                            "alert('Part number cannot be blank')", true);
                    return;
                }
                else if (licheck <= 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                           "alert('Quantity should be greater than zero')", true);
                    return;
                }
                else if (lex.Message.ToString() == "The request failed with HTTP status 404: Not Found.")
                {

                    Label3.Visible = true;
                    Label3.Text = "Part number doesn't exist";
                    return;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                            "alert('Failed to fetch parts')", true);
                }
               
            }


        }

        // Add Button Click:
        protected void Button2_Click(object sender, EventArgs e)
        {
            // Creating an object of the web service and setting username/pass
            localhost.WebService1 lwebstoreData = new localhost.WebService1();
            Set_Credentials(ref lwebstoreData);

            bool lbrollBackSuccessIndicator = true, lbsuccessIndicator = true;
            int litotRecords = 0;

            // Creating an XmlNode to store data in the web service request
            XmlDocument lxmldoc = new XmlDocument();
            XmlNode lxmlnode = lxmldoc.CreateNode(XmlNodeType.Element, "Details", "http://tempuri.org/");

            for (int i = 0; i < GridView1.Rows.Count; i++)
            {
                CheckBox lcbselect = (CheckBox)GridView1.Rows[i].FindControl("Select");
                TextBox ltbConfQty = (TextBox)GridView1.Rows[i].FindControl("ConfQty");
                TextBox ltbRemarks = (TextBox)GridView1.Rows[i].FindControl("Remarks");

                // If at least one row was checked
                if (lcbselect.Checked && lcbselect != null)
                {
                    // Incrementing no. of records
                    litotRecords++;

                    // Checking if quantity is a positive integer
                    int licheck;
                    bool lbNum = int.TryParse(ltbConfQty.Text.ToString(), out licheck);

                    if (!lbNum)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                                 "alert('Quantity should be a positive integer')", true);
                        return;
                    }
                    else if (licheck <= 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                                "alert('Quantity should greater than 0')", true);
                        return;
                    }

                    // Creating cart node
                    XmlNode lxmlCart = lxmldoc.CreateNode(XmlNodeType.Element,
                                                         "Cart", "http://tempuri.org/");
                    lxmlnode.AppendChild(lxmlCart);

                    // Adding tags to the cart node
                    Add_Node_From_String(lxmldoc, lxmlCart, "Username", cAuthHeaderCredentials.Username);
                    Add_Node_From_String(lxmldoc, lxmlCart, "Part", GridView1.Rows[i].Cells[3].Text.ToString());
                    Add_Node_From_String(lxmldoc, lxmlCart, "InqPart", GridView1.Rows[i].Cells[4].Text.ToString());
                    Add_Node_From_String(lxmldoc, lxmlCart, "Rate", GridView1.Rows[i].Cells[9].Text.ToString());
                    Add_Node_From_String(lxmldoc, lxmlCart, "Brand", GridView1.Rows[i].Cells[12].Text.ToString());
                    Add_Node_From_String(lxmldoc, lxmlCart, "ConfQty", ltbConfQty.Text.ToString());
                    Add_Node_From_String(lxmldoc, lxmlCart, "Remarks", ltbRemarks.Text.ToString());
                    Add_Node_From_String(lxmldoc, lxmlCart, "CreatedDate", DateTime.Now.Date.ToString());
                    Add_Node_From_String(lxmldoc, lxmlCart, "CreatedTime", DateTime.Now.ToString());
                }
            }


            // If at least one row was checked
            if (litotRecords > 0)
            {
                try

                {
                    // Calling web method to store/update the parts checked
                    XmlNode lxmlnodereturned = lwebstoreData.Add_To_Cart(lxmlnode, ref lbrollBackSuccessIndicator, ref lbsuccessIndicator);
                    // display cart
                    Display_Cart();

                }
                 catch (Exception lex)
                 {
                     if (lex.Message.ToString() == "Unable to connect to the remote server")
                     {
                         ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                              "alert('Unable to connect to the web service')", true);
                         return;
                     }
                     else if (!lbrollBackSuccessIndicator && lbrollBackSuccessIndicator)
                     {
                         ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                     "alert('Failed to add parts successfully but rolled back all records')", true);
                         return;
                     }
                     else if (!lbrollBackSuccessIndicator && !lbsuccessIndicator)
                     {
                         ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                     "alert('Failed to add parts successfully and roll back all records')", true);
                     }
                     else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                     "alert('Failed to add parts successfully')", true);
                    }
                 }

             }
             else
             {
                 ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                     "alert('At least one part has to be selected')", true);
             }
             
            }
     

        //MODIFIES: XML Response 
        //EFFECTS: Creates and adds a node to an existing node with the given details
        public void Add_Node_From_String(XmlDocument pxmldoc, XmlNode pxmlOrder, string psnodeName, string text)
        {
            XmlNode lxmlnewNode = pxmldoc.CreateNode(XmlNodeType.Element,
                                       psnodeName, "http://tempuri.org/");
            XmlText lxmlnewNodeText = pxmldoc.CreateTextNode(text);
            lxmlnewNode.AppendChild(lxmlnewNodeText);
            pxmlOrder.AppendChild(lxmlnewNode);
        }

        //REQUIRES: a valid number in string format
        //EFFECTS: converts the number from string to float and returns it
        public float String_To_Float(string psconvert)
        {
            // Checking if quantity is a positive integer
            float licheck;
            bool lbNum = float.TryParse(psconvert, out licheck);

            return licheck;
        }


        //EFFECTS: displays current items in the cart
        public void Display_Cart()

        {
            try
            { 
                // Creating an object of the web service and setting username/pass
                localhost.WebService1 lwebgetCart = new localhost.WebService1();
                Set_Credentials(ref lwebgetCart);
                bool lbisCartEmpty = false;
                XmlNode lxmlnodeCart = lwebgetCart.Display_Cart(ref lbisCartEmpty);

                if (!lbisCartEmpty)
                {
                    float litotRate = 0;
                    int litotQuantity = 0, litotRecords = 0;

                    // Adding columns to the data table
                    cdtpartsAdded.Columns.Add("Part");
                    cdtpartsAdded.Columns.Add("Mfg Part");
                    cdtpartsAdded.Columns.Add("Brd desc");
                    cdtpartsAdded.Columns.Add("Crtn Qty");
                    cdtpartsAdded.Columns.Add("Rate");
                    cdtpartsAdded.Columns.Add("Soh Cpd");
                    cdtpartsAdded.Columns.Add("Soh Brch");
                    cdtpartsAdded.Columns.Add("Brand");
                    cdtpartsAdded.Columns.Add("Desc");
                    cdtpartsAdded.Columns.Add("Conf Qty");
                    cdtpartsAdded.Columns.Add("Remarks");
                    int linumNodes = lxmlnodeCart.ChildNodes.Count;

                    for (int i = 0; i < linumNodes; i++)
                    {
                        // Reading from the web service's response to the data table
                        DataRow lrow = cdtpartsAdded.NewRow();
                        XmlNode lxmlchildNode = lxmlnodeCart.ChildNodes[i];
                        lrow[0] = lxmlchildNode["Part"].InnerXml;
                        lrow[1] = lxmlchildNode["MfgPart"].InnerXml;
                        lrow[2] = lxmlchildNode["BrdDesc"].InnerXml;
                        lrow[3] = lxmlchildNode["CrtnQty"].InnerXml;
                        lrow[4] = lxmlchildNode["Rate"].InnerXml;
                        lrow[5] = lxmlchildNode["SohCpd"].InnerXml;
                        lrow[6] = lxmlchildNode["SohBrch"].InnerXml;
                        lrow[7] = lxmlchildNode["Brand"].InnerXml;
                        lrow[8] = lxmlchildNode["Desc"].InnerXml;
                        lrow[9] = lxmlchildNode["ConfQty"].InnerXml;
                        lrow[10] = lxmlchildNode["Remarks"].InnerXml;
                        cdtpartsAdded.Rows.Add(lrow);
                        litotRate += String_To_Float(lxmlchildNode["Rate"].InnerXml);
                        litotRecords++;
                        litotQuantity += (int)String_To_Float(lxmlchildNode["ConfQty"].InnerXml);

                    }

                    // Binding data to grid view
                    GridView2.DataSource = cdtpartsAdded;
                    GridView2.DataBind();
                    GridView2.Visible = true;

                    // Displaying other order details
                    OrderConfirmation.Visible = true;
                    RecordsAccumulated.Visible = true;
                    RecordsAccumulatedno.Text = litotRecords.ToString();
                    RecordsAccumulatedno.Visible = true;
                    TotQuantity.Visible = true;
                    totqtyno.Visible = true;
                    totqtyno.Text = litotQuantity.ToString();
                    totrate.Visible = true;
                    totrateno.Text = litotRate.ToString();
                    totrateno.Visible = true;
                    DeleteButton.Visible = true;

                }
            }
            catch (Exception lex)
            {
                if (lex.Message.ToString() == "Unable to connect to the remote server")
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                         "alert('Unable to connect to the web service')", true);
                    return;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                         "alert('Unable to display cart')", true);
                }

            }
         }

        protected void Upload_Click(object sender, EventArgs e)
        {
            View.Visible = false;
            Label4.Visible = false;
            try
           {
                string lsfilePath = FileUpload1.PostedFile.FileName;
                string lsfileName = Path.GetFileName(lsfilePath);
                string lsext = Path.GetExtension(lsfileName);
                string lstype;
                if(!FileUpload1.HasFile)
                {
                    Label4.Text = "Please select a file";
                    return;
                }
                if (lsext == ".pdf")
                {
                    lstype = "application/pdf";
                    // Creating an XmlNode to store data in the web service request
                    XmlDocument lxmldoc = new XmlDocument();
                    XmlNode lxmlnode = lxmldoc.CreateNode(XmlNodeType.Element, "Details", "http://tempuri.org/");


                    // Creating file node
                    XmlNode lxmlFile = lxmldoc.CreateNode(XmlNodeType.Element,
                                                         "File", "http://tempuri.org/");
                    lxmlnode.AppendChild(lxmlFile);

                    // Creating an object of the web service and setting username/pass
                    localhost.WebService1 lwebuploadFile = new localhost.WebService1();
                    Set_Credentials(ref lwebuploadFile);

                    // Adding file details to file node
                    Add_Node_From_String(lxmldoc, lxmlFile, "Username", cAuthHeaderCredentials.Username);
                    Add_Node_From_String(lxmldoc, lxmlFile, "Name", lsfileName);
                    Add_Node_From_String(lxmldoc, lxmlFile, "Type", lstype);

                    // Reading file as binary data
                    Stream lfileStream = FileUpload1.PostedFile.InputStream;
                    BinaryReader lbr = new BinaryReader(lfileStream);
                    Byte[] fileData = lbr.ReadBytes((Int32)lfileStream.Length);

                    // Calling function from the web service to upload file
                    XmlNode lxmlnodeFile = lwebuploadFile.Upload_File(lxmlnode, ref fileData);

                    Label4.Text = "File Uploaded Successfully!";
                    View.Visible = true;
                    Label4.Visible = true;

                }
                else
                {

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                         "alert('Only PDF files allowed')", true);
                    return;

                }
             }
            catch(Exception lex)
            {
                if (lex.Message.ToString() == "Unable to connect to the remote server")
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                         "alert('Unable to connect to the web service')", true);
                    return;
                }
                else
                {

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                         "alert('Unable to upload file')", true);
                    return;
                }
          }
          
      }

        protected void View_Click(object sender, EventArgs e)
        {
            try
            {
                // Creating an object of the web service and setting username/pass
                localhost.WebService1 lwebviewFile = new localhost.WebService1();
                Set_Credentials(ref lwebviewFile);
                Byte[] fileData = null;
                XmlNode lxmlnodeFile = lwebviewFile.View_Uploaded_File(ref fileData);
                XmlNode lxmlchildNode = lxmlnodeFile.ChildNodes[0];
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = lxmlchildNode["Type"].InnerXml;
                Response.AddHeader("content-disposition", "attachment;filename=" + lxmlchildNode["Filename"].InnerXml);
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                XmlNode lxmlDataNode = lxmlchildNode["Data"];
                Response.BinaryWrite(fileData);
                Response.End();
            }
            catch (Exception lex)
            {
                if (lex.Message.ToString() == "Unable to connect to the remote server")
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                         "alert('Unable to connect to the web service')", true);
                    return;
                }
                else if (lex.Message.ToString() == "The request failed with HTTP status 404: Not Found.")
                {

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                         "alert('No files uploaded')", true);
                    return;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                         "alert('Failed to view file')", true);
                }
            }
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            // Creating an object of the web service and setting username/pass
            localhost.WebService1 lwebsdeleteData = new localhost.WebService1();
            Set_Credentials(ref lwebsdeleteData);

            bool lbrollBackSuccessIndicator = true, lbsuccessIndicator = true;

            // Creating an XmlNode to store data in the web service request
            XmlDocument lxmldoc = new XmlDocument();
            XmlNode lxmlnode = lxmldoc.CreateNode(XmlNodeType.Element, "Details", "http://tempuri.org/");
            int litotRecords = 0;

            for (int i = 0; i < GridView2.Rows.Count; i++)
            {
                CheckBox lcbselect = (CheckBox)GridView2.Rows[i].FindControl("Select");
                // If at least one row was checked
                if (lcbselect.Checked && lcbselect != null)
                {
                    // Creating cart node
                    XmlNode lxmlCart = lxmldoc.CreateNode(XmlNodeType.Element,
                                                         "Cart", "http://tempuri.org/");
                    lxmlnode.AppendChild(lxmlCart);
                    // Adding tags to the cart node
                    Add_Node_From_String(lxmldoc, lxmlCart, "Username", cAuthHeaderCredentials.Username);
                    Add_Node_From_String(lxmldoc, lxmlCart, "Part", GridView2.Rows[i].Cells[1].Text.ToString());
                    Add_Node_From_String(lxmldoc, lxmlCart, "Rate", GridView2.Rows[i].Cells[5].Text.ToString());
                    Add_Node_From_String(lxmldoc, lxmlCart, "Brand", GridView2.Rows[i].Cells[8].Text.ToString());
                    litotRecords++;
                }

            }

            // If at least one row was checked
            if (litotRecords > 0)
            {
                try
                {
                    // Calling web method to store/update the parts checked
                    XmlNode lxmlnodereturned = lwebsdeleteData.Delete_From_Cart(lxmlnode, 
                                               ref lbrollBackSuccessIndicator, ref lbsuccessIndicator);
                    // If all records were deleted, don't display the cart
                    if (litotRecords == GridView2.Rows.Count)
                    {
                        OrderConfirmation.Visible = false;
                        RecordsAccumulated.Visible = false;
                        RecordsAccumulatedno.Visible = false;
                        TotQuantity.Visible = false;
                        totqtyno.Visible = false;
                        totrate.Visible = false;
                        totrateno.Visible = false;
                        DeleteButton.Visible = false;
                        GridView2.Visible = false;
                    }
                    else
                    {
                        // display cart
                        Display_Cart();
                    }
                }
                catch (Exception lex)
                {
                    if (lex.Message.ToString() == "Unable to connect to the remote server")
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                             "alert('Unable to connect to the web service')", true);
                        return;
                    }
                    else if (!lbrollBackSuccessIndicator && lbrollBackSuccessIndicator)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                    "alert('Failed to delete all parts successfully but rolled back all records')", true);
                        return;
                    }
                    else if (!lbrollBackSuccessIndicator && !lbsuccessIndicator)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                    "alert('Failed to delete parts successfully and roll back all records')", true);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                     "alert('Failed to delete parts successfully')", true);
                    }
                }

            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message",
                                                    "alert('At least one part has to be selected')", true);
            }

        }

    }
}
