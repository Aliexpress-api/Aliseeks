using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Aliseeks.Models;
using Aliseeks.Domain;

namespace Aliseeks.Controllers
{
    public class HomeController : Controller
    {
        SearchCriteria search
        {
            get
            {
                SearchCriteria c = (SearchCriteria)Session["criteria"];
                if(c == null)
                {
                    c = new SearchCriteria();
                    Session["criteria"] = c;
                }

                return c;
            }

            set
            {
                Session["criteria"] = value;
            }
        }

        // GET: Home
        public ActionResult Index()
        {
            return View(search);
        }

        public PartialViewResult SearchBar()
        {
            return PartialView(search);
        }

        public ViewResult SearchResults(int page)
        {
            SearchModel model = new SearchModel();
            model.Criteria = search;
            model.Criteria.Page = page;

            Aliseeks.Domain.AliexpressAPI api = new Domain.AliexpressAPI();
            model.Items = api.GetItems(model.Criteria);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(SearchCriteria s)
        {
            if (ModelState.IsValid)
            {
                search = s;
                return RedirectToAction("SearchResults", "Home", new { page = 1 });
            }
            else
            {
                return View(s);
            }
        }
    }
}