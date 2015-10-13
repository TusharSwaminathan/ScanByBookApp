using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ScanByBookApp
{
    public class HomePageControl
    {
        public async Task<List<BookEntity>> GetBookDetails(int iSearch, string strValue, string strUser)
        {
            HomePageModel hpmGetBookDetails = new HomePageModel();
            String strVal =await hpmGetBookDetails.GetBookDetails(iSearch, strValue);
            return PopulateBookDetails(strVal, strUser);
        }

        private List<BookEntity> PopulateBookDetails(String strVal, string strUser)
        {

            XmlDocument xm = new XmlDocument();
            xm.LoadXml(strVal);
            List<BookEntity> lstBookDet = new List<BookEntity>();
            foreach (XmlNode xndNod in xm)
                foreach (XmlNode xndNode in xndNod)
                {

                    BookEntity beBookDet = new BookEntity();
                    beBookDet.id = xndNode["id"].InnerText;
                    beBookDet.ISBNType = xndNode["ISBNType"].InnerText;
                    beBookDet.ISBNNo = xndNode["ISBNNo"].InnerText;
                    beBookDet.etag = xndNode["etag"].InnerText;
                    beBookDet.title = xndNode["title"].InnerText;
                    beBookDet.authors = xndNode["authors"].InnerText;
                    beBookDet.publisher = xndNode["publisher"].InnerText;
                    beBookDet.publishedDate = xndNode["publishedDate"].InnerText;
                    beBookDet.Image = xndNode["Image"].InnerText;
                    beBookDet.preview = xndNode["preview"].InnerText;
                    beBookDet.pageCount = xndNode["pageCount"].InnerText;
                    beBookDet.textSnippet = xndNode["textSnippet"].InnerText;
                    beBookDet.UserRead = "Read";
                    lstBookDet.Add(beBookDet);
                }
            return lstBookDet;
        }

        internal async Task<bool> SaveNotesToDB(string strNotes,string strUser,string strISBN)
        {
            HomePageModel hpmSaveToDB = new HomePageModel();
            return await hpmSaveToDB.SaveNotesToDB(strNotes, strUser, strISBN);
        }

        internal void SaveBooksToDB(string strUserId,BookEntity Bookdata,bool IsRead)
        {
            HomePageModel hpmSaveToDB = new HomePageModel();
            List<BookEntity> lstBooks = new List<BookEntity>();
            lstBooks.Add(Bookdata);
            XmlSerializer serializer = new XmlSerializer(typeof(List<BookEntity>));
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            serializer.Serialize(xmlWriter, lstBooks);
            hpmSaveToDB.SaveBooksToDB(strUserId,stringWriter.ToString(),IsRead);
        }

        internal BookNotesEntity SeekAndGetBooks(string strTxtValue, int iSrchCondn, string strUserId)
        {
            HomePageModel hpmSeekAndGet = new HomePageModel();
            return hpmSeekAndGet.SeekAndGetBookDet(strTxtValue, iSrchCondn, strUserId);
        
        }
    }
}