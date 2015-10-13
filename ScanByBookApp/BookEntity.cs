using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScanByBookApp
{
    [Serializable]
    public class BookEntity
    {
        public String id { get; set; }
        public String ISBNType { get; set; }
        public String ISBNNo { get; set; }
        public String etag { get; set; }
        public String title { get; set; }
        public String authors { get; set; }
        public String publisher { get; set; }
        public String publishedDate { get; set; }
        public String description { get; set; }
        public String Image { get; set; }
        public String preview { get; set; }
        public String pageCount { get; set; }
        public String textSnippet { get; set; }
        public String UserRead { get; set; }
    }

    [Serializable]
    public class NotesEntity
    {
        public String ISBNNo { get; set; }
        public String UserId { get; set; }
        public String Notes { get; set; }
    }

    [Serializable]
    public class BookNotesEntity
    {
        public List<BookEntity> lstBook { get; set; }
        public List<NotesEntity> lstNote { get; set; }
    }
}