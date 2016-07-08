using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aliseeks.Domain;

namespace Aliseeks.Models
{
    public class SearchModel
    {
        public SearchCriteria Criteria { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
}