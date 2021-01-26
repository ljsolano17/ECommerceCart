using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppECartDemo.Models;
using WebAppECartDemo.ViewModel;

namespace WebAppECartDemo.Controllers
{
    public class ShoppingController : Controller
    {
        private ECartDBEntities objECartDbEntities;
        private List<ShoppingCartModel> listOfShoppingCartModels;


        public ShoppingController()
        {
            objECartDbEntities = new ECartDBEntities();
            listOfShoppingCartModels = new List<ShoppingCartModel>();
        }


        // GET: Shopping
        public ActionResult Index()
        {

            IEnumerable<ShoppingViewModel> listOfShoppingViewModel = (from objItem in objECartDbEntities.Items
                                                                      join
                                                                      objCate in objECartDbEntities.Categories
                                                                      on objItem.CategoryId equals objCate.CategoryId
                                                                      select new ShoppingViewModel()
                                                                      {
                                                                          ImagePath = objItem.ImagePath,
                                                                          ItemName = objItem.ItemName,
                                                                          Description = objItem.Description,
                                                                          ItemPrice = objItem.ItemPrice,
                                                                          ItemId = objItem.ItemId,
                                                                          Category = objCate.CategoryName,
                                                                          ItemCode = objItem.ItemCode
                                                                      }).ToList();

            return View(listOfShoppingViewModel);
        }
        [HttpPost]
        public JsonResult Index(string ItemId)
        {

            

            ShoppingCartModel objShoppingCartModel = new ShoppingCartModel();
            Item objItem = objECartDbEntities.Items.Single(model => model.ItemId.ToString() == ItemId);
            if (Session["CartCounter"] != null)
            {
                listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            }
            if (listOfShoppingCartModels.Any(model => model.ItemId.ToString() == ItemId))
            {
                //Segunda vez que se agrega al carrito
                objShoppingCartModel = listOfShoppingCartModels.Single(model => model.ItemId == ItemId);
                objShoppingCartModel.Quantity = objShoppingCartModel.Quantity + 1;
                objShoppingCartModel.Total = objShoppingCartModel.Quantity * objShoppingCartModel.UnitPrice;
            }
            else
            {
                //Primera vez que se agrega al carrito
                objShoppingCartModel.ItemId = ItemId;
                objShoppingCartModel.ImagePath = objItem.ImagePath;
                objShoppingCartModel.ItemName = objItem.ItemName;
                objShoppingCartModel.Quantity = 1;
                objShoppingCartModel.Total = objItem.ItemPrice;
                objShoppingCartModel.UnitPrice = objItem.ItemPrice;
                listOfShoppingCartModels.Add(objShoppingCartModel);

            }
            //Llevar un contador de productos en el carrito
            Session["CartCounter"] = listOfShoppingCartModels.Count;
            Session["CartItem"] = listOfShoppingCartModels;
            return Json(new { Success = true, Counter = listOfShoppingCartModels.Count },JsonRequestBehavior.AllowGet);

        }

       
        public ActionResult ShoppingCart()
        {
            listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            return View(listOfShoppingCartModels);
        }
        [HttpPost]
        public ActionResult AddOrder()
        {
            int OrderId = 0;
            listOfShoppingCartModels = Session["CartItem"] as List<ShoppingCartModel>;
            Order orderObj = new Order()
            {
                OrderDate = DateTime.Now,
                OrderNumber = String.Format("ddmmyyyyHHmmsss", DateTime.Now)
            };
            objECartDbEntities.Orders.Add(orderObj);
            objECartDbEntities.SaveChanges();
            OrderId = orderObj.OrderId;

            foreach(var item in listOfShoppingCartModels)
            {
                OrderDetail objOrderDetail = new OrderDetail();
                objOrderDetail.Total = item.Total;
                objOrderDetail.ItemId = item.ItemId;
                objOrderDetail.OrderId = OrderId;
                objOrderDetail.Quantity = item.Quantity;
                objOrderDetail.UnitPrice = item.UnitPrice;
                objECartDbEntities.OrderDetails.Add(objOrderDetail);
                objECartDbEntities.SaveChanges();

            }

            Session["CartItem"] = null;
            Session["CartCounter"] = null;
            return RedirectToAction("Index");
        }

    }
}