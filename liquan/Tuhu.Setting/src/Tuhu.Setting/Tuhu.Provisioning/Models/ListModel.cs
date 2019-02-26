using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class ListModel<T> : IEnumerable<T>, IEnumerable
    {
        public IEnumerable<T> Source { get; set; }

        public PagerModel Pager { get; set; }

        public ListModel()
        {
        }

        public ListModel(PagerModel pager)
        {
            this.Pager = pager;
        }

        public ListModel(IEnumerable<T> source)
        {
            this.Source = source;
        }

        public ListModel(PagerModel pager, IEnumerable<T> source)
        {
            this.Pager = pager;
            this.Source = source;
        }

        public ListModel(IEnumerable<T> source, PagerModel pager)
        {
            this.Source = source;
            this.Pager = pager;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Source.GetEnumerator();
        }
    }
}