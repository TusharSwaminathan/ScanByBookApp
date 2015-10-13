using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;

namespace ScanByBookApp
{
    public partial class HomePage : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                HomePageControl hpcSeek = new HomePageControl();
                List<BookEntity> lstBooksData = new List<BookEntity>();
                BookNotesEntity bneBookNotes = new BookNotesEntity();
                bneBookNotes = hpcSeek.SeekAndGetBooks(txtSearch.Value, GetSearchCriteria(), hvfEmail.Value);
                lstBooksData = bneBookNotes.lstBook;
                if (lstBooksData.Count.Equals(0))
                    GetBooksFromGoogle();
                else
                {
                    ViewState["NoteDet"] = bneBookNotes.lstNote;
                    BindGridData(lstBooksData);
                }
            }
            catch(Exception ex)
            {
                //log to be created
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Apologies", "alert('We are facing a technical difficulty, Please try again later :-( ')", true);
            }
        }

        private int GetSearchCriteria()
        {
            if (rbAuthor.Checked)
                return 1;
            else if (rbISBN.Checked)
                return 2;
            else if (rbTitle.Checked)
                return 3;
            else
                return 4;
        }

        private async void GetBooksFromGoogle()
        {
            try
            {
            HomePageControl hpcGetGoogleBooks = new HomePageControl();
            List<BookEntity> lstBooksData = new List<BookEntity>();
            lstBooksData =await hpcGetGoogleBooks.GetBookDetails(GetSearchCriteria(), txtSearch.Value, hvfEmail.Value);
            lstBooksData.Select(x => x.ISBNNo).Distinct().ToList();
            BindGridData(lstBooksData);
            }
            catch (Exception ex)
            {
                //log to be created
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Apologies", "alert('We are facing a technical difficulty, Please try again later :-(')", true);
            }
        }

        private void BindGridData(List<BookEntity> lstBooksData)
        {
            if (lstBooksData.Count.Equals(0))
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Apologies", "alert('I am sorry we were unable to find your search request !!!')", true);
       
                ViewState["BookDet"] = lstBooksData;
                gvBookDetails.DataSource = lstBooksData;
                gvBookDetails.DataBind();
            
        }

        protected void btnRead_Click(object sender, EventArgs e)
        {
            try
            {
                HomePageControl hpcSaveBooks = new HomePageControl();
                List<BookEntity> lstBooksData = new List<BookEntity>();
                lstBooksData = (List<BookEntity>)ViewState["BookDet"];
                Button b = new Button();
                b = (Button)sender;
                if (lstBooksData[Convert.ToInt32(b.CommandArgument)].UserRead.Equals("Unread"))
                {
                    lstBooksData[Convert.ToInt32(b.CommandArgument)].UserRead = "Read";
                    hpcSaveBooks.SaveBooksToDB(hvfEmail.Value, lstBooksData[Convert.ToInt32(b.CommandArgument)], false);
                }
                else
                {
                    hpcSaveBooks.SaveBooksToDB(hvfEmail.Value, lstBooksData[Convert.ToInt32(b.CommandArgument)], true);
                    lstBooksData[Convert.ToInt32(b.CommandArgument)].UserRead = "Unread";
                }
                BindGridData(lstBooksData);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Apologies", "alert('We are facing a technical difficulty please try again later!')", true);
            }
        }

        protected void gvBookDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBookDetails.DataSource = (List<BookEntity>)ViewState["BookDet"];
            gvBookDetails.PageIndex = e.NewPageIndex;
            gvBookDetails.DataBind();

        }

        protected void logoutText_Click(object sender, EventArgs e)
        {
            ViewState["BookDet"] = null;
            ViewState["NoteDet"] = null;
            gvBookDetails.DataSource = null;
            gvBookDetails.DataBind();
            hvfEmail.Value = "";
            txtSearch.Value = "";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Logged Out Successfully')", true);
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (ViewState["NoteDet"] != null)
                {
                    string customerId = gvBookDetails.DataKeys[e.Row.RowIndex].Value.ToString();
                    GridView gvOrders = e.Row.FindControl("gvOrders") as GridView;
                    List<NotesEntity> lstGridNote = new List<NotesEntity>();
                    List<NotesEntity> lstFullNote = (List<NotesEntity>)ViewState["NoteDet"];
                    lstFullNote.ForEach(x => { if (x.ISBNNo.Equals(customerId))lstGridNote.Add(x); });

                    gvOrders.DataSource = lstGridNote;
                    gvOrders.DataBind();
                }
            }
        }

        protected async void btnGridNote_Click(object sender, EventArgs e)
        {
            HomePageControl hpcSaveNotes = new HomePageControl();
            List<BookEntity> lstBooksData = new List<BookEntity>();
            lstBooksData = (List<BookEntity>)ViewState["BookDet"];

            Button b = new Button();
            b = (Button)sender;
            int iRow = Convert.ToInt32(b.CommandArgument) % 5;
            TextBox tbNotes = (TextBox)gvBookDetails.Rows[iRow].FindControl("txtNotes");
            bool val = await hpcSaveNotes.SaveNotesToDB(tbNotes.Text.Substring(1), hvfEmail.Value, lstBooksData[Convert.ToInt32(b.CommandArgument)].ISBNNo);

            if (val)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Thanks for your Note ! ')", true);
                NotesEntity neNewNote = new NotesEntity();
                List<NotesEntity> lstNotes = new List<NotesEntity>();
                neNewNote.ISBNNo = lstBooksData[Convert.ToInt32(b.CommandArgument)].ISBNNo;
                neNewNote.Notes = tbNotes.Text.Substring(1);
                neNewNote.UserId = hvfEmail.Value;
                if (ViewState["NoteDet"] != null)
                    lstNotes = (List<NotesEntity>)ViewState["NoteDet"];
                lstNotes.Add(neNewNote);
                ViewState["NoteDet"] = lstNotes;
                BindGridData(lstBooksData);
            }
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Apologies", "alert('We are facing a technical difficulty please try again later! ')", true);

        }
    }
}
