using KarimStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace KarimStore.Controllers
{
    public class HomeController : Controller
    {
        Model1 db = new Model1();
        Random random = new Random();
        public ActionResult Index()
        {
            return View();
        }

        //**************************************************************************
        public ActionResult ProductList()
        {

            List<Product> ProductList = db.Products.ToList();

            if (User.Identity.IsAuthenticated)
            {
                // user is logged in, do something
                string uID = User.Identity.GetUserId();
                string userName = User.Identity.Name;
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = userManager.FindById(uID);
                var roles = userManager.GetRoles(user.Id);
                string roleName = roles.FirstOrDefault();
                ViewBag.RoleName = roleName;
            }
            else
            {
                ModelState.AddModelError("", "You need to Login before you can add an item to cart");
            }
            db.Dispose();
            return View(ProductList);
        }
        //**************************************************************************
        public ActionResult ViewCart()
        {
            List<Cart> CartList = new List<Cart>();
            string uID = User.Identity.GetUserId();
            CartList = db.Carts.Where(i => i.UserID == uID).ToList();
            return View(CartList);
        }
        public ActionResult AddToCart(int Id, int Quant)
        {
            string uID = User.Identity.GetUserId();
            List<Cart> Cartlist = db.Carts.ToList();
            Product e = db.Products.Where(i => i.ProductID == Id).FirstOrDefault();
            Cart s = new Cart();

            s.UserID = uID;
            s.ProductID = Id;
            s.ProductName = e.ProductName;
            s.UnitPrice = e.UnitPrice;
            s.Quantity = Quant;

            db.Carts.Add(s);
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "big mistake");
            }
            db.Dispose();

            return RedirectToAction("ProductList");
        }
        public ActionResult DeleteFromCart(int Id)
        {
            Cart e = db.Carts.Where(i => i.CartID == Id).FirstOrDefault();
            db.Carts.Remove(e);
            db.SaveChanges();

            db.Dispose();
            return RedirectToAction("ViewCart");
        }

        [Authorize(Roles = "Employee")]
        public ActionResult DeleteFromInv(int Id)
        {
            Invoice z = db.Invoices.Where(i => i.InvoicesID == Id).FirstOrDefault();
            db.Invoices.Remove(z);
            db.SaveChanges();

            db.Dispose();
            return RedirectToAction("ViewInvoices");
        }

        [Authorize(Roles = "Employee")]
        public ActionResult DeleteFromInvHistory(int Id)
        {
            InvoiceHistory z = db.InvoiceHistories.Where(i => i.InvoicesHistoryID == Id).FirstOrDefault();
            List<Invoice> k = db.Invoices.Where(i => i.InvoiceHistoryID == Id).ToList();
            db.Invoices.RemoveRange(k);
            db.InvoiceHistories.Remove(z);
            db.SaveChanges();

            db.Dispose();
            return RedirectToAction("ViewInvoiceHistoriesAsEmp");
        }
        public ActionResult CartDetails(int Id)
        {
            Cart e = db.Carts.Where(i => i.CartID == Id).FirstOrDefault();
            ProductPic k = db.ProductPics.Where(i => i.ProductID == e.ProductID).FirstOrDefault();

            if (k != null)
            {
                byte[] byteArray = k.ImageFile;
                string base64String = Convert.ToBase64String(byteArray);
                ViewBag.Base64String = base64String;
            }
            db.Dispose();
            return View(e);
        }
        public ActionResult RemoveFromStock()
        {
            bool CanProcessRequest = true;
            bool CartIsEmpty = false;
            List<Invoice> InvoiceList = new List<Invoice>();
            int ProductIdRetriver;
            int CartIdRetriver;
            List<Cart> CartList = new List<Cart>();
            string uID = User.Identity.GetUserId();
            int randomNumber = random.Next(0, 2147483646);

            CartList = db.Carts.Where(i => i.UserID == uID).ToList();
            // to calculate total
            decimal sub = 0; decimal tot = 0;
            for (int i = 0; i < CartList.Count; i++)
            {
                sub = decimal.Parse(CartList[i].Quantity.ToString()) * decimal.Parse(CartList[i].UnitPrice.ToString());
                tot += sub;
            }

            InvoiceHistory H = new InvoiceHistory();
            H.CustomerID = uID;
            H.Status = "On The Way";
            H.InvDate = DateTime.Now;
            H.Total = tot;
            db.InvoiceHistories.Add(H);
            db.SaveChanges();
            for (int j = 0; j < CartList.Count; j++)
            {
                ProductIdRetriver = CartList[j].ProductID;
                CartIdRetriver = CartList[j].CartID;

                var e = db.Products.Where(i => i.ProductID == ProductIdRetriver).FirstOrDefault();
                var C = db.Carts.Where(i => i.CartID == CartIdRetriver).FirstOrDefault();
                e.InStock = int.Parse(e.InStock.ToString()) - int.Parse(CartList[j].Quantity.ToString());
                if (e.InStock < 0)
                {
                    CanProcessRequest = false;
                }
                Invoice s = new Invoice();
                s.ProductID = C.ProductID;
                s.ProductName = C.ProductName;
                s.CustomerID = C.UserID;
                s.UnitPrice = C.UnitPrice;
                s.Quantity = C.Quantity;
                s.InvDate = DateTime.Now;
                s.InvoiceHistoryID = H.InvoicesHistoryID;

                //InvoiceList.Add(s);
                db.Invoices.Add(s);

            }
            if (CartList.Count == 0)
            {
                CartIsEmpty = true;
            }
            if (CanProcessRequest == true && CartIsEmpty == false)
            {
                //for (int i = 0; i < InvoiceList.Count; i++)
                //{
                //    db.Invoices.Add(InvoiceList[i]);
                //}
                db.SaveChanges();
                db.Dispose();
                return RedirectToAction("EmptyCart");
            }
            else
            {
                if (CartIsEmpty == true)
                {
                    TempData["ErrorMessage"] = "Your Cart is Empty";
                    db.InvoiceHistories.Remove(H);
                    db.SaveChanges();
                    if (!User.Identity.IsAuthenticated)
                    {
                        TempData["ErrorMessage2"] = "Log in First";
                    }
                    return RedirectToAction("ViewCart", "Home");                    
                }                
                else
                {
                    TempData["ErrorMessage"] = "There is an item or more with more quantity requested than available please change the quantity requested and try again"; return RedirectToAction("ViewCart", "Home");
                }

            }
        }
        public ActionResult EmptyCart()
        {
            string uID = User.Identity.GetUserId();
            List<Cart> CartList = new List<Cart>();
            CartList = db.Carts.Where(i => i.UserID == uID).ToList();

            for (int i = 0; i < CartList.Count; i++)
            {
                db.Carts.Remove(CartList[i]);
            }
            TempData["SuccessMessage"] = "Order has been submitted !";
            db.SaveChanges();
            db.Dispose();
            return RedirectToAction("ViewCart");
        }
        //*******************************************************
        public ActionResult ViewImages()
        {
            List<ProfilePic> picList = new List<ProfilePic>();

            picList = db.ProfilePics.ToList();
            return View(picList);
        }
        public ActionResult UploadProductPicImage(int id)
        {
            ProductPic e = db.ProductPics.Where(i => i.ProductID == id).FirstOrDefault();
            ViewBag.PID = id;
            
            if (e != null)
            {
                byte[] byteArray = e.ImageFile;
                string base64String = Convert.ToBase64String(byteArray);
                ViewBag.Base64String = base64String;
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult UploadImageForProducts(HttpPostedFileBase imageFile, int id)
        {
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                byte[] imageData = new byte[imageFile.ContentLength];
                imageFile.InputStream.Read(imageData, 0, imageFile.ContentLength);
                ProductPic imageModel = new ProductPic();


                imageModel.ProductID = id;
                imageModel.ImageFile = imageData;
                ProductPic old = db.ProductPics.Where(i => i.ProductID == id).FirstOrDefault();
                // Save the image data to the database
                // ...
                if (old != null)
                {
                    db.ProductPics.Remove(old);
                }

                db.ProductPics.Add(imageModel);
                db.SaveChanges();

                return RedirectToAction("ProductList", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Select a file before pressing upload";
                ModelState.AddModelError("", "Select a file before pressing upload");
                return RedirectToAction("UploadProductPicImage", new { id = id });
            }

            //return View();
        }


        public ActionResult UploadProfilePicImage()
        {
            string uID = User.Identity.GetUserId();
            ProfilePic e = db.ProfilePics.Where(i => i.UserID == uID).FirstOrDefault();
            if (e != null)
            {
                byte[] k = e.ImageFile;
                ViewData["arr"] = k;
                byte[] byteArray = e.ImageFile;

                string base64String = Convert.ToBase64String(byteArray);
                ViewBag.Base64String = base64String;
            }

            return View();
        }


        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase imageFile)
        {
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                byte[] imageData = new byte[imageFile.ContentLength];
                imageFile.InputStream.Read(imageData, 0, imageFile.ContentLength);
                string uID = User.Identity.GetUserId();
                ProfilePic imageModel = new ProfilePic();
                imageModel.UserID = User.Identity.GetUserId();
                imageModel.ImageFile = imageData;
                ProfilePic old = db.ProfilePics.Where(i => i.UserID == uID).FirstOrDefault();
                // Save the image data to the database
                // ...
                if (old != null)
                {
                    db.ProfilePics.Remove(old);
                }
                
                db.ProfilePics.Add(imageModel);
                db.SaveChanges();

                return RedirectToAction("Index", "Manage");
            }
            else
            {
                TempData["ErrorMessage"] = "Select a file before pressing upload";
                ModelState.AddModelError("", "Select a file before pressing upload");
                return RedirectToAction("UploadProfilePicImage");
            }
            //return View();
        }
        //*******************************************************

        public ActionResult ViewInvoiceHistoriesAsCust()
        {
            string uID = User.Identity.GetUserId();
            List<InvoiceHistory> InvoiceHistList = db.InvoiceHistories.Where(i => i.CustomerID == uID).ToList();
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Log in First";
            }
            return View(InvoiceHistList);
        }

       

        [Authorize]
        public ActionResult HistoryDetails(int Id)
        {
            //Invoice e = db.Invoices.Where(i => i.InvoiceHistoryID == Id).FirstOrDefault();
            

            string uID = User.Identity.GetUserId();
            string userName = User.Identity.Name;
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = userManager.FindById(uID);
            var roles = userManager.GetRoles(user.Id);
            string roleName = roles.FirstOrDefault();
            ViewBag.RoleName = roleName;
            List<Invoice> InvList = new List<Invoice>();
            InvList = db.Invoices.Where(i => i.InvoiceHistoryID == Id).ToList();

            db.Dispose();
            return View(InvList);
        }

        [Authorize(Roles = "Employee")]
        public ActionResult ViewInvoices()
        {
            List<Invoice> InvoiceList = db.Invoices.ToList();
            return View(InvoiceList);
        }
        [Authorize(Roles = "Employee")]
        public ActionResult ViewInvoiceHistoriesAsEmp()
        {
            List<InvoiceHistory> InvoiceHistList = db.InvoiceHistories.ToList();
            return View(InvoiceHistList);
        }
        [Authorize(Roles = "Employee")]
        public ActionResult EditInvHistory(int Id)
        {
            InvoiceHistory e = db.InvoiceHistories.Where(i => i.InvoicesHistoryID == Id).FirstOrDefault();
            db.Dispose();
            return View(e);
        }
        public ActionResult SaveInvHist(InvoiceHistory e)
        {
            InvoiceHistory c = db.InvoiceHistories.Where(i => i.InvoicesHistoryID == e.InvoicesHistoryID).FirstOrDefault();

            c.Status = e.Status;
            db.SaveChanges();
            db.Dispose();
            return Redirect("ViewInvoiceHistoriesAsEmp");
        }
        //*********************************************************************************************

        [Authorize(Roles = "Employee")]
        public ActionResult Create()
        {

            return View();
        }

        public ActionResult Add(Product s)
        {

            db.Products.Add(s);

            db.SaveChanges();
            db.Dispose();

            return Redirect("ProductList");
        }

        [Authorize(Roles = "Employee")]
        public ActionResult Edit(int Id)
        {
            ProductPic Img = db.ProductPics.Where(i => i.ProductID == Id).FirstOrDefault();
            if (Img != null)
            {
                byte[] byteArray = Img.ImageFile;
                string base64String = Convert.ToBase64String(byteArray);
                ViewBag.Base64String = base64String;
            }


            Product e = db.Products.Where(i => i.ProductID == Id).FirstOrDefault();
            db.Dispose();
            return View(e);
        }
        public ActionResult Save(Product s)
        {
            Product e = db.Products.Where(i => i.ProductID == s.ProductID).FirstOrDefault();

            e.ProductName = s.ProductName;
            e.UnitPrice = s.UnitPrice;
            e.InStock = s.InStock;
            db.SaveChanges();
            db.Dispose();
            return Redirect("ProductList");
        }
        public ActionResult Details(int Id)
        {
            Product e = db.Products.Where(i => i.ProductID == Id).FirstOrDefault();
            ProductPic z = db.ProductPics.Where(i => i.ProductID == Id).FirstOrDefault();
            if (z != null)
            {
                byte[] byteArray = z.ImageFile;
                string base64String = Convert.ToBase64String(byteArray);
                ViewBag.Base64String = base64String;
            }
            db.Dispose();
            return View(e);
        }

        [Authorize(Roles = "Employee")]
        public ActionResult Delete(int Id)
        {
            Product e = db.Products.Where(i => i.ProductID == Id).FirstOrDefault();
            db.Products.Remove(e);
            db.SaveChanges();

            db.Dispose();
            return RedirectToAction("ProductList");
        }
        //**************************************************************************

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}