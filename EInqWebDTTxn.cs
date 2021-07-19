using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace EInqWebDTTxnClass
{
    public class EInqWebDTTxn
    {
        //REQUIRES: Valid XmlNode
        //MODIFIES: XML response, pbsuccessIndicator, pbrollBackSuccessIndicator
        //EFFECTS: stores parts added to cart in patcart on SQL Server
        public XmlNode Add_To_Cart(XmlNode pxmlpartDetails, ref bool pbsuccessIndicator,
                                    ref bool pbrollBackSuccessIndicator, string psConnStr, string psschema)

        {
            // Creating transaction object
            SqlTransaction lsqlTxnUpdate = null;
            // Connecting to Sql Server for updating rows (transactional connection)
            SqlConnection lsqlconnTxn = new SqlConnection(psConnStr);
            int linumNodes = pxmlpartDetails.ChildNodes.Count;
            int recordsAffected = 0;

            XmlDocument lxmldoc = new XmlDocument();
            XmlNode lxmlnodedetails = lxmldoc.CreateNode(XmlNodeType.Element,
                                  "Details", "http://tempuri.org/");

            try
            {
                // Attempt to connect to SQL server
                lsqlconnTxn.Open();
                SqlCommand lsqlcmd = new SqlCommand("AddToCart", lsqlconnTxn);
                lsqlcmd.CommandType = CommandType.StoredProcedure;
                lsqlTxnUpdate = lsqlconnTxn.BeginTransaction();
                for (int i = 0; i < linumNodes; i++)

                {
                    // Take the first childNode (part)
                    XmlNode childNode = pxmlpartDetails.ChildNodes[i];
                    lsqlcmd.Parameters.Add("@pcschema", SqlDbType.Char, 10).Value = psschema;
                    lsqlcmd.Parameters.Add("@Username", SqlDbType.Char, 10).Value = childNode["Username"].InnerXml;
                    lsqlcmd.Parameters.Add("@Part", SqlDbType.VarChar, 10).Value = childNode["Part"].InnerXml;
                    lsqlcmd.Parameters.Add("@InqPart", SqlDbType.VarChar, 10).Value = childNode["InqPart"].InnerXml;
                    lsqlcmd.Parameters.AddWithValue("@Rate", childNode["Rate"].InnerXml);
                    lsqlcmd.Parameters.Add("@Brand", SqlDbType.VarChar, 10).Value = childNode["Brand"].InnerXml;
                    lsqlcmd.Parameters.Add("@ConfQty", SqlDbType.Int, 4).Value = childNode["ConfQty"].InnerXml;
                    lsqlcmd.Parameters.Add("@Remarks", SqlDbType.VarChar, 10).Value = childNode["Remarks"].InnerXml;
                    lsqlcmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime, 8).Value = childNode["CreatedDate"].InnerXml;
                    lsqlcmd.Parameters.Add("@CreatedTime", SqlDbType.DateTime, 8).Value = childNode["CreatedTime"].InnerXml;
                    lsqlcmd.Transaction = lsqlTxnUpdate;
                    recordsAffected += lsqlcmd.ExecuteNonQuery();
                    // Clear existing parameters to add new ones
                    lsqlcmd.Parameters.Clear();
                }
                // Attempt to commit transaction
                lsqlTxnUpdate.Commit();
            }

            catch (Exception lex)
            {
                pbsuccessIndicator = false;
                if (recordsAffected > 0)
                {
                    try
                    {
                        // Attempt to roll back transaction
                        lsqlTxnUpdate.Rollback();
                    }
                    catch (Exception ex)
                    {
                        pbrollBackSuccessIndicator = false;
                    }
                }
            }

            if (pbsuccessIndicator)
            {
                // Creating Success Message
                XmlNode lxmlnodestatus = lxmldoc.CreateNode(XmlNodeType.Element,
                                      "Status", "http://tempuri.org/");
                XmlText lxmlStatusText = lxmldoc.CreateTextNode(recordsAffected.ToString() + " records affected");
                lxmlnodestatus.AppendChild(lxmlStatusText);
                lxmlnodedetails.AppendChild(lxmlnodestatus);
                return lxmlnodedetails;
            }

            return lxmlnodedetails;

        }



        //REQUIRES: Valid XmlNode
        //MODIFIES: XML response, pbsuccessIndicator, pbrollBackSuccessIndicator
        //EFFECTS: deletes parts from patcart on SQL Server
        public XmlNode Delete_From_Cart(XmlNode pxmlpartDetails, string psConnStr,
                        ref bool pbsuccessIndicator, ref bool pbrollBackSuccessIndicator)
        {
            // Creating transaction object
            SqlTransaction lsqlTxnDelete = null;
            // Connecting to Sql Server for deleting rows (transactional connection)
            SqlConnection lsqlconnTxn = new SqlConnection(psConnStr);
            int linumNodes = pxmlpartDetails.ChildNodes.Count;
            int lirecordsAffected = 0;

            XmlDocument lxmldoc = new XmlDocument();
            XmlNode lxmlnodedetails = lxmldoc.CreateNode(XmlNodeType.Element,
                                  "Details", "http://tempuri.org/");

            string lsdeletecmd = "DELETE FROM PATCART WHERE PCPART = @PartNo AND PCBRAND = @Brand AND PCUSER = @Username";
            SqlCommand lsqlcmdDelete = new SqlCommand();
            lsqlcmdDelete.CommandText = lsdeletecmd;
            lsqlcmdDelete.Connection = lsqlconnTxn;

              try
              {
                // Attempt to connect to SQL server
                lsqlconnTxn.Open();
                lsqlTxnDelete = lsqlconnTxn.BeginTransaction();
                for (int i = 0; i < linumNodes; i++)

                {
                    // Take the first childNode (part)
                    XmlNode childNode = pxmlpartDetails.ChildNodes[i];
                    lsqlcmdDelete.Parameters.Add("@PartNo", SqlDbType.VarChar, 10).Value = childNode["Part"].InnerXml;
                    lsqlcmdDelete.Parameters.Add("@Brand", SqlDbType.VarChar, 10).Value = childNode["Brand"].InnerXml;
                    lsqlcmdDelete.Parameters.Add("@Username", SqlDbType.VarChar, 10).Value = childNode["Username"].InnerXml;
                    lsqlcmdDelete.Transaction = lsqlTxnDelete;
                    lirecordsAffected += lsqlcmdDelete.ExecuteNonQuery();
                    // Clear existing parameters to add new ones
                    lsqlcmdDelete.Parameters.Clear();
                }
                // Attempt to commit transaction
                lsqlTxnDelete.Commit();

                // Creating Success Message
                XmlNode lxmlnodestatus = lxmldoc.CreateNode(XmlNodeType.Element,
                                      "Status", "http://tempuri.org/");
                XmlText lxmlStatusText = lxmldoc.CreateTextNode(lirecordsAffected.ToString() + " records affected");
                lxmlnodestatus.AppendChild(lxmlStatusText);
                lxmlnodedetails.AppendChild(lxmlnodestatus);
            }
             catch (Exception lex)
             {
                pbsuccessIndicator = false;
                if (lirecordsAffected > 0)
                {
                    try
                    {
                        // Attempt to roll back transaction
                        lsqlTxnDelete.Rollback();
                    }
                    catch (Exception ex)
                    {
                        pbrollBackSuccessIndicator = false;
                    }
                }
            }

            return lxmlnodedetails;
        }



        //REQUIRES: Valid XmlNode
        //MODIFIES: XML response, pbsuccessIndicator
        //EFFECTS: uploads files to sql table PATUSERFILES
        public XmlNode Upload_File(XmlNode pxmlfileDetails, string psConnStr,
                         string psschema, ref bool pbsuccessIndicator, ref Byte[] fileData)
        {
            // Connecting to Sql Server for uploading the file (transactional connection)
            SqlConnection lsqlconnTxn = new SqlConnection(psConnStr);
            int lirecordsAffected = 0;

            XmlDocument lxmldoc = new XmlDocument();
            XmlNode lxmlnodedetails = lxmldoc.CreateNode(XmlNodeType.Element,
                                  "Details", "http://tempuri.org/");

            string lsuploadcmd = "INSERT INTO PATUSERFILES (UFAPPL, UFUSER, UFNAME, UFTYPE, UFDATA) " +  
                                 "VALUES (@Schema, @Username, @FileName, @Type, @Data)";
            SqlCommand lsqlcmdUpload = new SqlCommand();
            lsqlcmdUpload.CommandText = lsuploadcmd;
            lsqlcmdUpload.Connection = lsqlconnTxn;
            XmlNode childNode = pxmlfileDetails.ChildNodes[0];

            try
            {
                lsqlconnTxn.Open();
                lsqlcmdUpload.Parameters.Add("@Schema", SqlDbType.Char, 10).Value = psschema;
                lsqlcmdUpload.Parameters.Add("@Username", SqlDbType.Char, 10).Value = childNode["Username"].InnerXml;
                lsqlcmdUpload.Parameters.Add("@FileName", SqlDbType.VarChar, 50).Value = childNode["Name"].InnerXml;
                lsqlcmdUpload.Parameters.Add("@Type", SqlDbType.VarChar, 50).Value = childNode["Type"].InnerXml;
                lsqlcmdUpload.Parameters.Add("@Data", SqlDbType.VarBinary).Value = fileData;
                lirecordsAffected += lsqlcmdUpload.ExecuteNonQuery();
                // Creating Success Message
                XmlNode lxmlnodestatus = lxmldoc.CreateNode(XmlNodeType.Element,
                                      "Status", "http://tempuri.org/");
                XmlText lxmlStatusText = lxmldoc.CreateTextNode(lirecordsAffected.ToString() + " records affected");
                lxmlnodestatus.AppendChild(lxmlStatusText);
                lxmlnodedetails.AppendChild(lxmlnodestatus);

            }
            catch (Exception lex)
            {
                pbsuccessIndicator = false;

            }

            return lxmlnodedetails;
       }


    }

}

