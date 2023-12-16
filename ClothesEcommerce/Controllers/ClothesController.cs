using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Antlr.Runtime.Tree;
using ClothesEcommerce.Models;
using PagedList;
using PayPal.Api;

namespace ClothesEcommerce.Controllers
{
    
    public class ClothesController : Controller
    {
        ClothesEcomerceEntities3 db = new ClothesEcomerceEntities3();

        
        public ActionResult Index(string selectedType, string selectedCategory,string selectedPrice,int?page)
        {
            
            var types =new List<string>() { "T-shirt", "Pants", "Shoes", "Hoodie", "Jacket"};
            var categories =new List<string>(){ "Men", "Women", "Children", "Boys", "Girls" };
            var prices =new List<string>(){ "High To Low", "Low To High" };

            ViewBag.Types = new SelectList(types);
            ViewBag.Categories = new SelectList(categories);
            ViewBag.Prices = new SelectList(prices);


            var data = (from x in db.Clothes select x);

            if (!string.IsNullOrEmpty(selectedType))
            {
                data = data.Where(d => d.Type == selectedType);
            }

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                data = data.Where(d => d.Category == selectedCategory);
            }
            if (!string.IsNullOrEmpty(selectedPrice))
            {
                if(selectedPrice == "High To Low")
                {
                    data = data.OrderByDescending(p => p.Price);
                }
                else if (selectedPrice == "Low To High")
                {
                    data = data.OrderBy(p => p.Price);
                }
            }
            if (data.Count() == 0)
            {
                ViewBag.NoFiler= "Sorry, No result found";
            }

            int pagesize = 6, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            data = data.OrderByDescending(x => x.ClothesID);
           
            return View(data.ToList().ToPagedList(pageindex,pagesize));
        }
        
        [CustomAuthorization]
        public ActionResult Create()
        {
            var Types = new List<String>
            {
                "T-shirt","Pants","Shoes","Hoodie","Jacket"
            };
            SelectList ClothesType = new SelectList(Types);
            ViewBag.ClothesType = ClothesType;

            var Categories = new List<String>
            {
                "Men","Women","Children","Boys","Girls"
            };
            SelectList ClothesCategory = new SelectList(Categories);
            ViewBag.ClothesCategory = ClothesCategory;
            return View();
        }
       

        [CustomAuthorization]
        [HttpPost]
        public ActionResult Create(Cloth clothes, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                db.Clothes.Add(clothes);

                foreach (var file in files)
                {
                    var picturePath = uploadimgfile(file);
                    if (picturePath != "-1")
                    {
                        db.Pictures.Add(new Picture { ClothesID = clothes.ClothesID, PicturePath = picturePath });
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Details(int id)
        {
            var clothes = db.Clothes.Find(id);
            return View(clothes);
        }
        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cloth = db.Clothes.Find(id);

            if (cloth == null)
            {
                return HttpNotFound();
            }

            return View(cloth);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var cloth = db.Clothes.Find(id);
            var picture = (from x in db.Pictures where x.ClothesID == id select x).ToList();

            if (cloth == null)
            {
                return HttpNotFound();
            }

            // Display a confirmation alert using JavaScript
            ViewBag.ShowAlert = true;

            // Perform the deletion
            db.Clothes.Remove(cloth);
            foreach (var item in picture)
            {
                db.Pictures.Remove(item);
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/ClothesPictures/"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/ClothesPictures/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }

        public ActionResult Search(string id)
        {
            if (id.Length == 0)
            {
                ViewBag.Error = "Please enter a text";
                return RedirectToAction("Index", new { id = ViewBag.Error });
            }

            var cloth = (from x in db.Clothes where x.ClothName.Contains(id) select x).ToList();
            if (cloth.Count == 0)
            {
                ViewBag.Error = "We couldn't find this Product";
                return RedirectToAction("Index", new { id = ViewBag.Error });
            }
            return View(cloth);
        }
        [CustomAuthorization]
        public ActionResult Update(int id)
        {
            var Types = new List<String>
            {
                "T-shirt","Pants","Shoes","Hoodie","Jacket"
            };
            SelectList ClothesType = new SelectList(Types);
            ViewBag.ClothesType = ClothesType;

            var Categories = new List<String>
            {
                "Men","Women","Children","Boys","Girls"
            };
            SelectList ClothesCategory = new SelectList(Categories);
            ViewBag.ClothesCategory = ClothesCategory;

            var cloth = (from x in db.Clothes where x.ClothesID == id select x).First();
            Cloth cloth1 = new Cloth();
            cloth1.ClothesID= id;
            cloth1.ClothName = cloth.ClothName;
            cloth1.Price = cloth.Price;
            cloth1.Description = cloth.Description;
            cloth1.Category = cloth.Category;
            cloth1.Type = cloth.Type;

            return View(cloth1);
        }
        [CustomAuthorization]
        [HttpPost]
        public ActionResult Update(Cloth cloth)
        {
            var Types = new List<String>
            {
                "T-shirt","Pants","Shoes","Hoodie","Jacket"
            };
            SelectList ClothesType = new SelectList(Types);
            ViewBag.ClothesType = ClothesType;

            var Categories = new List<String>
            {
                "Men","Women","Children","Boys","Girls"
            };
            SelectList ClothesCategory = new SelectList(Categories);
            ViewBag.ClothesCategory = ClothesCategory;
            if (ModelState.IsValid) { 
            var cloth1= (from x in db.Clothes where x.ClothesID == cloth.ClothesID select x).First();
            if (cloth != null)
            {
                cloth1.ClothName = cloth.ClothName;
                cloth1.Price = cloth.Price;
                cloth1.Description = cloth.Description;
                cloth1.Category = cloth.Category;
                cloth1.Type = cloth.Type;

                db.SaveChanges();
                ViewBag.Msg = "Successfully updated";
            }
            else
            {
                ViewBag.Msg = "Something went wrong";

            }
            }
            
            
            return View();
        }

        [AllowUserAccess]
        public ActionResult Cart()
        {
            var user = Convert.ToInt32(Session["user"]);

            var card = (from x in db.Card_ where x.UserId == user select x).ToList();
            var amount = (from x in db.Card_ where x.UserId == user select x).ToList();

            float total = 0;
            foreach(var i in amount)
            {
                total +=Convert.ToInt32(i.Quantity * i.Amount);
            }
            ViewBag.Total = total; 
            return View(card);
        }

        [AllowUserAccess]

        [HttpPost]
        public ActionResult Cart(int id)
        {

            var price = (from x in db.Clothes where x.ClothesID == id select x.Price).FirstOrDefault();
            Card_ card = new Card_();
            card.ClothId = id;
            card.Quantity = 1;  
            card.Amount = price * card.Quantity;
            card.UserId = Convert.ToInt32(Session["user"]);

            var crd = (from x in db.Card_ where x.UserId == card.UserId && x.ClothId == card.ClothId select x).FirstOrDefault();
            if (crd == null)
            {
                db.Card_.Add(card);
                db.SaveChanges();
                ViewBag.Msg = "The Product Succesfly added to your cart";
            }
            else
            {
                ViewBag.Msg = "The Product Already in your cart";

            }
            return RedirectToAction("Index");
        }
        [AllowUserAccess]

        public ActionResult IncrementQuantity(int id)
        {
            var user = Convert.ToInt32(Session["user"]);

            var qt = (from x in db.Card_ where x.CardId == id select x).First();
            qt.Quantity = qt.Quantity + 1;
            db.SaveChanges();


            return RedirectToAction("Cart");
        }
        [AllowUserAccess]

        public ActionResult decrementQuantity(int id)
        {
            var user = Convert.ToInt32(Session["user"]);

            var qt = (from x in db.Card_ where x.CardId == id select x).First();
            qt.Quantity = qt.Quantity - 1;
            db.SaveChanges();


            return RedirectToAction("Cart");
        }
        [AllowUserAccess]

        public ActionResult DeleteCardItem(int id)
        {
            var item = (from x in db.Card_ where x.CardId == id select x).FirstOrDefault();
            db.Card_.Remove(item);
            db.SaveChanges();
            ViewBag.DeleteConfirmation="The Item Succefly deleted";
            return RedirectToAction("Cart");
        }
        [AllowUserAccess]

        public ActionResult Checkout()
        {
            return View();
        }

        

        

    }
}
