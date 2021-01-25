using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppECartDemo.Models;
using WebAppECartDemo.ViewModel;

namespace WebAppECartDemo.Controllers
{
    public class ItemController : Controller
    {
        private ECartDBEntities objECartDBEntities;
        public ItemController()
        {
            objECartDBEntities = new ECartDBEntities();
        }


        // GET: Item
        public ActionResult Index()
        {

            ItemViewModel objItemViewModel = new ItemViewModel();
            objItemViewModel.CategorySelectListItem = (from objCat in objECartDBEntities.Categories
                                                       select new SelectListItem()
                                                       {
                                                           Text = objCat.CategoryName,
                                                           Value = objCat.CategoryId.ToString(),
                                                           Selected = true
                                                       });
            return View(objItemViewModel);
        }

        [HttpPost]
        public JsonResult Index(ItemViewModel objItemViewModel)
        {
            Item objItem = new Item();
            if (objItemViewModel.ImagePath == null)
            {
            
            }
            else
            {
                string NewImage = Guid.NewGuid() + Path.GetExtension(objItemViewModel.ImagePath.FileName);
                objItemViewModel.ImagePath.SaveAs(Server.MapPath("~/Images/" + NewImage));
                objItem.ImagePath = "~/Images/" + NewImage;
            }
           

           

           
            objItem.CategoryId = objItemViewModel.CategoryId;
            objItem.Description = objItemViewModel.Description;
            objItem.ItemCode = objItemViewModel.ItemCode;
            objItem.ItemId = Guid.NewGuid();
            objItem.ItemName = objItemViewModel.ItemName;
            objItem.ItemPrice = objItemViewModel.ItemPrice;
            objECartDBEntities.Items.Add(objItem);
            objECartDBEntities.SaveChanges();

            return Json(new {Success = true, Message = "Item is added Successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}