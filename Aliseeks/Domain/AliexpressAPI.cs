using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Threading.Tasks;

using Aliseeks.Models;
using Aliseeks.Infrastructure;
using HtmlAgilityPack;

namespace Aliseeks.Domain
{
    public enum SortTypeAli
    {
        Price, Quantity
    }

    public class AliexpressAPI
    {
        const string baseURL = @"http://www.aliexpress.com/wholesale?";

        const string seperator = "&";
        const string searchText = "SearchText=";
        const string minPrice = "minPrice=";
        const string maxPrice = "maxPrice=";
        const string sortType = "SortType=";
        const string freeShip = "isFreeShip=";
        const string onSale = "isOnSale=";
        const string onePiece = "isRtl=";
        const string appOnly = "isMobileExclusive=";
        const string fromCountry = "shipFromCountry=";
        const string toCountry = "shipCountry=";
        const string page = "page=";
        const string unitPrice = "isUnitPrice=";
        const string needQuery = "needQuery=";
        const string grid = "g=";

        public IEnumerable<Item> GetItems(SearchCriteria criteria)
        {
            List<Item> items = new List<Item>();

            //Request from AliExpress
            string url = insertParameters(criteria);

            WebRequest req = WebRequest.Create(url);
            WebResponse resp = req.GetResponse();
            StreamReader reader = new StreamReader(resp.GetResponseStream());

            var doc = new HtmlDocument();
            doc.LoadHtml(reader.ReadToEnd());

            //select element holding items
            var itemElements = doc.DocumentNode.Descendants().First(p => p.Id.Contains("page")).Descendants().Where(p => p.Attributes.Contains("class") && p.Attributes["class"].Value == "item");

            //Cycle through all the elements
            foreach (var element in itemElements)
            {
                Item item = new Item();

                //Get name element
                var nameElement = HTMLUtility.GetNode_ByClass(element, "history-item product");
                var imageElement = HTMLUtility.GetNode_ByClass(element, "picCore");
                var priceElement = HTMLUtility.GetNode_ByClass(element, "price-m");
                var freesElement = HTMLUtility.GetNode_ByClass(element, "free-s");
                var mobileElement = HTMLUtility.GetNode_ByClass(element, "mobile-exclusive");
                var storeElement = HTMLUtility.GetNode_ByClass(element, "store ");
                var feedbackElement = HTMLUtility.GetNode_ByClass(element, "rate-num");
                var ordersElement = HTMLUtility.GetNode_ByClass(element, "order-num-a");

                if (nameElement != null)
                {
                    item.Name = nameElement.Attributes["title"].Value;
                    item.Link = nameElement.Attributes["href"].Value;
                }

                if (imageElement != null)
                {
                    item.ImageURL = imageElement != null ? imageElement.Attributes.First(p => p.Name.Contains("src")).Value : "";
                }

                if(priceElement != null && priceElement.HasChildNodes)
                {
                    item.Price = priceElement.Descendants().First(p => p.Attributes.Contains("itemprop") && p.Attributes["itemprop"].Value.Contains("price")).InnerText;
                    item.Unit = priceElement.Descendants().First(p => p.Attributes.Contains("class") && p.Attributes["class"].Value.Contains("unit")).InnerText;
                }

                if(freesElement != null)
                { 
                    item.FreeShipping = (freesElement != null);
                }
                
                if(mobileElement != null && mobileElement.ChildNodes[0] != null)
                {
                    item.MobileOnly = mobileElement.ChildNodes[1].InnerText;
                }

                if(storeElement != null)
                {
                    item.StoreName = storeElement.Attributes["title"].Value;
                }

                if(feedbackElement != null)
                {
                    item.Feedback = feedbackElement.Attributes["title"].Value;
                }

                if(ordersElement != null)
                {
                    item.Orders = ordersElement.InnerText;
                }

                items.Add(item);
            }

            //Async database store results and history
            AliDbApi api = new AliDbApi();
            Task.Factory.StartNew( () => api.InsertSearchHistory(criteria, items.Count));

            return items;
        }

        string insertParameters(SearchCriteria criteria)
        {
            string url = baseURL;
            url += searchText + criteria.SearchText.Replace(" ", "+");
            if (criteria.PriceFrom != 0) { url += seperator + minPrice + criteria.PriceFrom.ToString(); }
            if (criteria.PriceTo != 0) { url += seperator + maxPrice + criteria.PriceTo.ToString(); }

            switch (criteria.Sort)
            {
                case SortTypeAli.Price:
                    url += seperator + sortType + "price_asc";
                    break;

                case SortTypeAli.Quantity:
                    break;
            }

            if (criteria.FreeShipping) { url += seperator + freeShip + "y"; }
            else { url += seperator + freeShip + "n"; }

            if (criteria.SaleItems) { url += seperator + onSale + "y"; }
            else { url += seperator + onSale + "n"; }

            if (criteria.PieceOnly) { url += seperator + onePiece + "y"; }
            else { url += seperator + onePiece + "n"; }

            if (criteria.AppOnly) { url += seperator + appOnly + "y"; }
            else { url += seperator + appOnly + "n"; }

            url += seperator + fromCountry + criteria.ShipFrom;
            url += seperator + toCountry + criteria.ShipTo;
            url += seperator + page + criteria.Page.ToString();

            if (criteria.UnitPrice) { url += seperator + unitPrice + "y"; }
            else { url += seperator + unitPrice + "n"; }

            if (criteria.NeedQuery) { url += seperator + needQuery + "y"; }
            else { url += seperator + needQuery + "n"; }

            if (criteria.Grid) { url += seperator + grid + "y"; }
            else { url += seperator + grid + "n"; }

            return url;
        }
    }
}