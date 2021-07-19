using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace EInqWebDTNonTxnClass
{
    public class EInqWebDTNonTxn
    {
        //MODIFIES: XML Response, pbpartExistenceIndicator, pbsuccess
        //EFFECTS: connects to SQL Server to provide part details
        public XmlNode Fetch_Parts(string pspartNo, int piqty, string psConnStr,
                                   ref bool pbpartExistenceIndicator, ref bool pbsuccess)
        {
            // Connect to Sql Server
            SqlConnection lsqlconn = new SqlConnection(psConnStr);
            // Creating an XmlNode to store data from dataset
            XmlDocument lxmldoc = new XmlDocument();
            XmlNode lxmlnode = lxmldoc.CreateNode(XmlNodeType.Element,
                              "Details", "http://tempuri.org/");


            // Creating Stored Procedure command
            SqlCommand lsqlcmd = new SqlCommand("DTPartSelection", lsqlconn);
            lsqlcmd.CommandType = CommandType.StoredProcedure;

            // Add Parameters to Stored Procedure command
            lsqlcmd.Parameters.AddWithValue("@Franc", "52");
            lsqlcmd.Parameters.AddWithValue("@Part_no", pspartNo);
            lsqlcmd.Parameters.AddWithValue("@PrType", 2);
            lsqlcmd.Parameters.AddWithValue("@PrValue", "RP");
            lsqlcmd.Parameters.AddWithValue("@SlMADtype", "MAD");
            lsqlcmd.Parameters.AddWithValue("@SlSOHtype", "SOH");
            lsqlcmd.Parameters.AddWithValue("@Cust", "9999");

            try
            {  // Calling Stored procedure and obtaining data in a dataset
                DataSet lds = new DataSet();
                SqlDataAdapter lsqlda = new SqlDataAdapter();
                lsqlda.SelectCommand = lsqlcmd;
                lsqlda.Fill(lds);

                // If part number does not exist, setting existenceIndicator to false
                if (Is_Empty(lds))
                {
                    pbpartExistenceIndicator = false;
                    return lxmlnode;
                }

                else
                {
                    // Writing data from dataset to XmlNode
                    for (int i = 0; i < lds.Tables.Count; i++)
                    {
                        for (int j = 0; j < lds.Tables[i].Rows.Count; j++)
                        {

                            XmlNode lxmlpartNo = lxmldoc.CreateNode(XmlNodeType.Element,
                                                                   "Part", "http://tempuri.org/");
                            lxmlnode.AppendChild(lxmlpartNo);

                            Add_Node_From_DataSet(i, j, lds, lxmldoc, lxmlpartNo, "Part", "Part");

                            XmlNode lxmlInqPart = lxmldoc.CreateNode(XmlNodeType.Element,
                                                                     "InqPart", "http://tempuri.org/");
                            XmlText lxmlInqPartText = lxmldoc.CreateTextNode(pspartNo.ToString());
                            lxmlInqPart.AppendChild(lxmlInqPartText);
                            lxmlpartNo.AppendChild(lxmlInqPart);

                            Add_Node_From_DataSet(i, j, lds, lxmldoc, lxmlpartNo, "MfgPart", "Suppart");
                            Add_Node_From_DataSet(i, j, lds, lxmldoc, lxmlpartNo, "BrdDesc", "BrdDesc");

                            XmlNode lxmlReqQty = lxmldoc.CreateNode(XmlNodeType.Element,
                                                                     "ReqQty", "http://tempuri.org/");
                            XmlText lxmlReqQtyText = lxmldoc.CreateTextNode(piqty.ToString());
                            lxmlReqQty.AppendChild(lxmlReqQtyText);
                            lxmlpartNo.AppendChild(lxmlReqQty);

                            Add_Node_From_DataSet(i, j, lds, lxmldoc, lxmlpartNo, "CrtnQty", "CrtnQty");
                            Add_Node_From_DataSet(i, j, lds, lxmldoc, lxmlpartNo, "Rate", "Rate");
                            Add_Node_From_DataSet(i, j, lds, lxmldoc, lxmlpartNo, "SohCpd", "SohCpd");
                            Add_Node_From_DataSet(i, j, lds, lxmldoc, lxmlpartNo, "SohBrch", "SohBrch");
                            Add_Node_From_DataSet(i, j, lds, lxmldoc, lxmlpartNo, "Brand", "Brand");
                            Add_Node_From_DataSet(i, j, lds, lxmldoc, lxmlpartNo, "Desc", "PartDesc");

                        }
                    }
                }

                return lxmlnode;
            }
            catch (Exception lex)
            {
                pbsuccess = false;
                return lxmlnode;
            }
        }


        //MODIFIES: XML Response 
        //EFFECTS: creates and adds a node to an existing node from a given dataset with the given details
        void Add_Node_From_DataSet(int pitableNo, int pirowNo, DataSet pds,
            XmlDocument pxmldoc, XmlNode pxmlpartNo, string psnodeName, string pselementName)
        {
            XmlNode lxmlnewNode = pxmldoc.CreateNode(XmlNodeType.Element,
                                       psnodeName, "http://tempuri.org/");
            XmlText lxmlnewNodeText = pxmldoc.CreateTextNode(pds.Tables[pitableNo]
                                     .Rows[pirowNo][pselementName].ToString());
            lxmlnewNode.AppendChild(lxmlnewNodeText);
            pxmlpartNo.AppendChild(lxmlnewNode);
        }


        //MODIFIES: XML Response , pbisCartEmpty, pbsuccess
        //EFFECTS: displays existing parts in PATCART 
        public XmlNode Display_Cart(string psusername, string psConnStr, ref bool pbisCartEmpty, ref bool pbsuccess)
        {
            // Connect to Sql Server
            SqlConnection lsqlconn = new SqlConnection(psConnStr);
            // Creating an XmlNode to store data from dataset
            XmlDocument lxmldoc = new XmlDocument();
            XmlNode lxmlnode = lxmldoc.CreateNode(XmlNodeType.Element,
                              "Details", "http://tempuri.org/");

            try {
                lsqlconn.Open();

                // Creating Stored Procedure command
                SqlCommand lsqlcmd = new SqlCommand("DisplayCart", lsqlconn);
                lsqlcmd.CommandType = CommandType.StoredProcedure;

                // Add Parameters to Stored Procedure command
                lsqlcmd.Parameters.AddWithValue("@Username", psusername);

                // Calling Stored procedure and obtaining data in a dataset
                DataSet lds = new DataSet();
                SqlDataAdapter lsqlda = new SqlDataAdapter();
                lsqlda.SelectCommand = lsqlcmd;
                lsqlda.Fill(lds);

                if (Is_Empty(lds))
                {
                    pbisCartEmpty = true;
                    return lxmlnode;

                }
                else
                {
                    for (int j = 0; j < lds.Tables[0].Rows.Count; j++)
                    {

                        XmlNode lxmlpartNo = lxmldoc.CreateNode(XmlNodeType.Element,
                                                               "Part", "http://tempuri.org/");
                        lxmlnode.AppendChild(lxmlpartNo);

                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "Part", "Part");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "MfgPart", "Suppart");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "BrdDesc", "BrdDesc");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "CrtnQty", "CrtnQty");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "ConfQty", "PCCONFQTY");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "Rate", "Rate");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "SohCpd", "SohCpd");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "SohBrch", "SohBrch");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "Brand", "Brand");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "Desc", "PartDesc");
                        Add_Node_From_DataSet(0, j, lds, lxmldoc, lxmlpartNo, "Remarks", "PCREMARKS");

                    }
                    return lxmlnode;
                }
            }
            catch (Exception lex)
            {
                pbsuccess = false;
                return lxmlnode;
            }
       }


        //EFFECTS: returns true if pds is empty and false if not
        bool Is_Empty(DataSet pds)
        {

            // Counting total number rows from each table
            int linumRows = 0;
            for (int i = 0; i < pds.Tables.Count; i++)
            {
                linumRows += pds.Tables[i].Rows.Count;
            }

            // If part number does not exist, setting existenceIndicator to false
            if (linumRows == 0)
            {
                return true;
            }
            return false;
        }


        //MODIFIES: XML Response , pbisEmpty, pbsuccessIndicator
        //EFFECTS: displays file last uploaded by the user in PATUSERFILES
        public XmlNode View_Uploaded_File(string psConnStr, ref bool pbsuccessIndicator, 
                                          ref bool pbisEmpty, ref Byte[] fileData)
        {
            // Connect to Sql Server
            SqlConnection lsqlconn = new SqlConnection(psConnStr);
            // Creating an XmlNode to store data from dataset
            XmlDocument lxmldoc = new XmlDocument();
            XmlNode lxmlnode = lxmldoc.CreateNode(XmlNodeType.Element,
                              "Details", "http://tempuri.org/");
            SqlCommand lsqlcmd = new SqlCommand();
            string lscmd = "SELECT TOP 1 *FROM PATUSERFILES ORDER BY UFFILEID";
            lsqlcmd.CommandText = lscmd;
            lsqlcmd.Connection = lsqlconn;


            try
            {
                lsqlconn.Open();
                // Obtaining data in a dataset
                SqlDataReader lsqldr = lsqlcmd.ExecuteReader();
                if(lsqldr.Read())
                {
                    XmlNode lxmlfileNode = lxmldoc.CreateNode(XmlNodeType.Element,
                              "File", "http://tempuri.org/");
                    fileData = (byte[])lsqldr["UFDATA"];
                    Add_Node_From_String(lxmldoc, lxmlfileNode, "Filename", lsqldr["UFNAME"].ToString());
                    Add_Node_From_String(lxmldoc, lxmlfileNode, "Type", lsqldr["UFTYPE"].ToString());
                    lxmlnode.AppendChild(lxmlfileNode);
                }

            }
            catch (Exception ex)
            {
                pbsuccessIndicator = false;
            }

            return lxmlnode;

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


    }
 
}
