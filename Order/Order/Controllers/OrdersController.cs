using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Order.Models;
using LinqKit;

namespace Order.Controllers
{
    public class OrdersController : Controller
    {
        private TSQL2012Entities db = new TSQL2012Entities();

        // GET: Orders
        public ActionResult Index(string orderID, string customerName, string employees, string companyName, string orderDate, string shippedDate, string requiredDate)
        {
            IQueryable<Orders> orders = db.Orders;

            ViewBag.employees = new SelectList(db.Employees, "EmployeeID", "FullName");
            ViewBag.companyName = new SelectList(db.Shippers, "ShipperID", "CompanyName");

            var pred = PredicateBuilder.True<Orders>();

            if (!String.IsNullOrEmpty(orderID))
            {
                pred = pred.And(x => x.OrderID.ToString().Equals(orderID));
            }

            if (!String.IsNullOrEmpty(customerName))
            {
                pred = pred.And(x => x.Customers.CompanyName.Contains(customerName));
            }

            if (!String.IsNullOrEmpty(employees))
            {
                pred = pred.And(x => x.EmployeeID.ToString().Equals(employees));
            }

            if (!String.IsNullOrEmpty(companyName))
            {
                pred = pred.And(x => x.Shippers.ShipperID.ToString().Equals(companyName));
            }

            if (!String.IsNullOrEmpty(orderDate))
            {
                DateTime orderDt;
                DateTime.TryParse(orderDate, out orderDt);
                pred = pred.And(x => x.OrderDate==(orderDt));
            }

            if (!String.IsNullOrEmpty(shippedDate))
            {
                DateTime shippedDt;
                DateTime.TryParse(shippedDate, out shippedDt);
                pred = pred.And(x => x.ShippedDate==(shippedDt));
            }

            if (!String.IsNullOrEmpty(requiredDate))
            {
                DateTime requiredDt;
                DateTime.TryParse(requiredDate, out requiredDt);
                pred = pred.And(x => x.RequiredDate==(requiredDt));
            }
            return View(orders.AsExpandable().Where(pred).Include(o => o.Employees).Include(o => o.Customers).Include(o => o.Shippers).ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            List<Double> price = new List<Double>();
            List<int> pname = new List<int>();

            foreach (var item in db.OrderDetails)
            {
                price.Add(Decimal.ToDouble(item.Products.UnitPrice));
                pname.Add(item.Products.ProductID);
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FullName");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName");
            ViewBag.ShipperID = new SelectList(db.Shippers, "ShipperID", "CompanyName");
            ViewBag.productName = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.unitPrice = price;
            ViewBag.proName = pname;
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipperID,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry,productName,UnitPrice,Qty")] Orders orders)
        {
            if (ModelState.IsValid)
            {   
                orders.Freight = (string.IsNullOrEmpty(orders.Freight.ToString())) ? 0 : orders.Freight;
                orders.ShipCountry = (string.IsNullOrEmpty(orders.ShipCountry)) ? string.Empty : orders.ShipCountry;
                orders.ShipCity = (string.IsNullOrEmpty(orders.ShipCity)) ? string.Empty : orders.ShipCity;
                orders.ShipRegion = (string.IsNullOrEmpty(orders.ShipRegion)) ? string.Empty : orders.ShipRegion;
                orders.ShipPostalCode = (string.IsNullOrEmpty(orders.ShipPostalCode)) ? string.Empty : orders.ShipPostalCode;
                orders.ShipAddress = (string.IsNullOrEmpty(orders.ShipAddress)) ? string.Empty : orders.ShipAddress;
                orders.ShipName = (string.IsNullOrEmpty(orders.ShipName)) ? string.Empty : orders.ShipName;
                db.Orders.Add(orders);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FullName");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName");
            ViewBag.ShipperID = new SelectList(db.Shippers, "ShipperID", "CompanyName");
            ViewBag.productName = new SelectList(db.Products, "ProductID", "ProductName");
            return View(orders);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            List<Double> price = new List<Double>();
            List<int> pname = new List<int>();

            foreach (var item in db.OrderDetails)
            {
                price.Add(Decimal.ToDouble(item.Products.UnitPrice));
                pname.Add(item.Products.ProductID);
            }

            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "FullName");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName");
            ViewBag.ShipperID = new SelectList(db.Shippers, "ShipperID", "CompanyName");
            ViewBag.productName = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.unitPrice = price;
            ViewBag.proName = pname;
            return View(orders);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,CustomerID,EmployeeID,OrderDate,RequiredDate,ShippedDate,ShipperID,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry,productName,UnitPrice,Qty")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                orders.Freight = (string.IsNullOrEmpty(orders.Freight.ToString())) ? 0 : orders.Freight;
                orders.ShipCountry = (string.IsNullOrEmpty(orders.ShipCountry)) ? string.Empty : orders.ShipCountry;
                orders.ShipCity = (string.IsNullOrEmpty(orders.ShipCity)) ? string.Empty : orders.ShipCity;
                orders.ShipRegion = (string.IsNullOrEmpty(orders.ShipRegion)) ? string.Empty : orders.ShipRegion;
                orders.ShipPostalCode = (string.IsNullOrEmpty(orders.ShipPostalCode)) ? string.Empty : orders.ShipPostalCode;
                orders.ShipAddress = (string.IsNullOrEmpty(orders.ShipAddress)) ? string.Empty : orders.ShipAddress;
                orders.ShipName = (string.IsNullOrEmpty(orders.ShipName)) ? string.Empty : orders.ShipName;

                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeID = new SelectList(db.Employees, "EmployeeID", "LastName");
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "CompanyName");
            ViewBag.ShipperID = new SelectList(db.Shippers, "ShipperID", "CompanyName");
            return View(orders);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders orders = db.Orders.Find(id);
            db.Orders.Remove(orders);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
