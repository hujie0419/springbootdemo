using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class PagerModel
    {
        public PagerModel()
      : this(1, 20)
        {
        }

        public PagerModel(int currentPage)
          : this(currentPage, 20)
        {
        }

        public PagerModel(int currentPage, int pageSize)
        {
            this.CurrentPage = currentPage < 1 ? 1 : currentPage;
            this.PageSize = pageSize < 1 ? 20 : pageSize;
        }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalItem { get; set; }

        public int TotalPage
        {
            get
            {
                return (this.TotalItem + this.PageSize - 1) / this.PageSize;
            }
        }
    }
}