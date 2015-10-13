using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Xml;
using System.Xml.Xsl;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.Threading.Tasks;

namespace ScanByBookApp
{
    public class HomePageModel
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["ScanConn"].ConnectionString);

        public async Task<string> GetBookDetails(int iSearch, string strValue)
        {
                    try
                    {
                        switch (iSearch)
                        {
                            case 1:
                                return await GetXmlFromServer("https://www.googleapis.com/books/v1/volumes?q=inauthor:" + strValue + "&key=AIzaSyAOLyj_HgNnyCRy23VHLQibgOVu5lVwWQQ&maxResults=20&orderBy=newest");
                            case 2:
                                return await GetXmlFromServer("https://www.googleapis.com/books/v1/volumes?q=isbn:" + strValue + "&key=AIzaSyAOLyj_HgNnyCRy23VHLQibgOVu5lVwWQQ&maxResults=20&orderBy=newest");
                            case 3:
                                return await GetXmlFromServer("https://www.googleapis.com/books/v1/volumes?q=intitle:" + strValue + "&key=AIzaSyAOLyj_HgNnyCRy23VHLQibgOVu5lVwWQQ&maxResults=20&orderBy=newest");
                            case 4:
                                return await GetXmlFromServer("https://www.googleapis.com/books/v1/volumes?q=" + strValue + "&key=AIzaSyAOLyj_HgNnyCRy23VHLQibgOVu5lVwWQQ&maxResults=20&orderBy=newest");
                            default: return "";
                        }
                    }
                    catch (Exception ex)
                    {
                        //log to be created
                        return "";
                    }
        }

        private async Task<string> GetXmlFromServer(String dblpUrl)
        {
            try
            {
                WebRequest wr = WebRequest.Create(dblpUrl);
                WebResponse resp = await wr.GetResponseAsync();
                Stream dataStream = resp.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                String strVal = ConvertToXML(responseFromServer);
                reader.Close();
                dataStream.Close();
                resp.Close();
                return strVal;
            }
            catch (Exception ex)
            {
                //log to be created
                return "";
            }
        }

        private String ConvertToXML(String json)
        {
            try
            {
                XmlDocument doc = JsonConvert.DeserializeXmlNode(json, "root");
                string output;
                ServicePointManager.Expect100Continue = true;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                using (StringReader sReader = new StringReader(doc.InnerXml))
                using (XmlReader xReader = XmlReader.Create(sReader))
                using (StringWriter sWriter = new StringWriter())
                using (XmlWriter xWriter = XmlWriter.Create(sWriter))
                {
                    XslCompiledTransform xslt = new XslCompiledTransform();
                    xslt.Load("https://web.njit.edu/~ts336/GoogleBook.xslt");
                    xslt.Transform(xReader, xWriter);
                    output = sWriter.ToString();
                }
                return output;
            }
            catch (Exception ex)
            {
                //log to be created
                return "";
            }
        }

        internal void SaveBooksToDB(String strUserId, String strXmlBookDet, bool IsRead)
        {
            try
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("usp_InsertBookDetails", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@BookDet", strXmlBookDet));
                cmd.Parameters.Add(new SqlParameter("@UserId", strUserId));
                cmd.Parameters.Add(new SqlParameter("@IsRead", IsRead ? 1 : 0));
                cmd.ExecuteNonQueryAsync();
                cnn.Close();
            }
            catch (Exception e)
            {
                //log to be createds
            }
        }

        internal BookNotesEntity SeekAndGetBookDet(String strTxtVal, int iSearchCondn, string strUserId)
        {
            try
            {
                cnn.Open();
                SqlParameter[] spParameter = new SqlParameter[3];

                spParameter[0] = new SqlParameter("@SearchFor", SqlDbType.VarChar);
                spParameter[0].Direction = ParameterDirection.Input;
                spParameter[0].Value = strTxtVal;

                spParameter[1] = new SqlParameter("@SearchCondn", SqlDbType.VarChar);
                spParameter[1].Direction = ParameterDirection.Input;
                spParameter[1].Value = iSearchCondn;

                spParameter[2] = new SqlParameter("@UserId", SqlDbType.VarChar);
                spParameter[2].Direction = ParameterDirection.Input;
                spParameter[2].Value = strUserId;

                DataSet ds = SqlHelper.ExecuteDataset(cnn, CommandType.StoredProcedure, "usp_SeekAndGetBookDet", spParameter);
                cnn.Close();
                List<BookEntity> lstBooks = ds.Tables[0].AsEnumerable().Select(dataRow => new BookEntity
                {

                    ISBNType = dataRow.Field<string>("ISBNType"),
                    ISBNNo = dataRow.Field<string>("ISBN"),
                    etag = dataRow.Field<string>("ETag"),
                    title = dataRow.Field<string>("Title"),
                    authors = dataRow.Field<string>("Author"),
                    publisher = dataRow.Field<string>("Publisher"),
                    publishedDate = dataRow.Field<string>("PublishDate"),
                    Image = dataRow.Field<string>("ImageURL"),
                    preview = dataRow.Field<string>("Preview"),
                    pageCount = dataRow.Field<string>("PageCount"),
                    textSnippet = dataRow.Field<string>("Snippet"),
                    UserRead = dataRow.Field<string>("UserRead")

                }).ToList();

                List<NotesEntity> lstNotes = ds.Tables[1].AsEnumerable().Select(dataRow => new NotesEntity
                                {

                                    UserId = dataRow.Field<string>("UserId"),
                                    Notes = dataRow.Field<string>("NoteDetails"),
                                    ISBNNo = dataRow.Field<string>("ISBNNo")

                                }).ToList();

                BookNotesEntity bneCollection = new BookNotesEntity();
                bneCollection.lstBook = lstBooks;
                bneCollection.lstNote = lstNotes;
                return bneCollection;
            }
            catch (Exception e)
            {
                //log to be created
                return null;
            }
        }

        internal async Task<bool> SaveNotesToDB(string strNotes, string strUser, string strISBN)
        {
            try
            {
                cnn.Open();
                SqlCommand scQuery = new SqlCommand("INSERT INTO TBLNOTEDETAILS VALUES('" + strUser + "','" + strNotes + "','" + Guid.NewGuid() + "','" + strISBN + "')", cnn);
                int result =await scQuery.ExecuteNonQueryAsync();
                cnn.Close();
                if (result >= 0)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                //log to be created
                return false;
            }
        }
    }
}