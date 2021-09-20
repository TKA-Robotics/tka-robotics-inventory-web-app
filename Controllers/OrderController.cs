using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DotNetCoreSqlDb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Reflection.Metadata.Ecma335;
using System.Drawing.Printing;

namespace DotNetCoreSqlDb.Controllers
{
    [Authorize(Roles = "Team")]
    public class OrderController : Controller
    {
        private readonly MyDatabaseContext _context;

        public OrderController(MyDatabaseContext context)
        {
            _context = context;
        }

        // GET: Order
        [Authorize(Roles = "Team")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Order.ToListAsync());
        }

        public IActionResult SelectDetails(string partName)
        {
            Console.WriteLine("Select details");

            Console.WriteLine(partName);

            int partID = queryForId(partName);

            var redirect = Url.Action("Details", "Order", new { id = partID });

            return Json(new
            {
                redirectUrl = redirect
            });

            /*return RedirectToAction("Edit", "Inventory", new { id = partID });*/
        }

        // GET: Order/Details/5
        [Authorize(Roles = "Team")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        [Authorize(Roles = "Team")]
        public IActionResult Create()
        {
            Console.WriteLine("Order create");

            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Team")]
        public async Task<IActionResult> Create([Bind("ID,Timestamp,OrderingStudent,RoboticsTeam,Vendor,PartName,SKU,Link,Quantity,Price,Barcode,Justification,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                
                Console.WriteLine("status");
                Console.WriteLine(order.Status);

                /*Inventory inventoryTest = new Inventory();

                inventoryTest.PartName = order.PartName;
                inventoryTest.Quantity = order.Quantity;

                _context.Add(inventoryTest);*/


                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public IActionResult SelectAddToInventory(string partName)
        {
            Console.WriteLine("Add To Inventory");

            Console.WriteLine(partName);

            int partID = queryForId(partName);

            Console.WriteLine("partID");
            Console.WriteLine(partID);

            var redirect = Url.Action("AddToInventory", "Order", new { id = partID });

            return Json(new
            {
                redirectUrl = redirect
            });

            /*return RedirectToAction("Edit", "Inventory", new { id = partID });*/
        }

        [Authorize(Roles = "Business")]
        public async Task<IActionResult> AddToInventory(int? id)
        {

            Console.WriteLine("AddToInventory");
            Console.WriteLine("Order ID");
            Console.WriteLine(id);

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);

            ViewData["status"] = order.Status;

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> AddToInventory(int id, [Bind("ID,Timestamp,OrderingStudent,RoboticsTeam,Vendor,PartName,SKU,Link,Quantity,Price,Justification,Status")] Order order)
        {
            Console.WriteLine("Add to Inventory Post");


            if (id != order.ID)
            {
                Console.WriteLine("NOT FOUND");
                return NotFound();
            }

            Console.WriteLine("FOUND");

            if (ModelState.IsValid)
            {

                order.Status = Order.OrderStatus.Added;

                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                Inventory inventory = new Inventory();

                inventory.PartName = order.PartName;
                InventoryController ic = new InventoryController(_context);
                int partID = -1;
                partID = ic.queryForId(inventory.PartName);
                
                Console.WriteLine("PART ID");
                Console.WriteLine(partID);

                if(partID == -1)
                {
                    inventory.SKU = order.SKU;
                    inventory.UnitCost = order.Price;
                    inventory.Quantity = order.Quantity;
                    inventory.Status = Inventory.InventoryStatus.Approved;

                    _context.Add(inventory);
                    await _context.SaveChangesAsync();
                } else
                {
                    Console.WriteLine("UPDATING EXISTING ITEM");
                    var updatedInventory = await _context.Inventory.FindAsync(partID);
                    Console.WriteLine(updatedInventory.Quantity);
                    Console.WriteLine(order.Quantity);
                    updatedInventory.Quantity += order.Quantity;
                    Console.WriteLine(updatedInventory.Quantity);

                    _context.Update(updatedInventory);
                    await _context.SaveChangesAsync();
                }
                


                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public IActionResult SelectEdit(string partName)
        {
            Console.WriteLine("Select edit");

            Console.WriteLine(partName);

            int partID = queryForId(partName);

            var redirect = Url.Action("Edit", "Order", new { id = partID });

            return Json(new
            {
                redirectUrl = redirect
            });

            /*return RedirectToAction("Edit", "Inventory", new { id = partID });*/
        }

        // GET: Order/Edit/5
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Edit(int? id)
        {

            Console.WriteLine("edit!");

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            /*ViewData["status"] = order.Status;*/

/*            ViewData["quantity"] = order.Quantity;
            ViewData["suggestedQuantity"] = order.SuggestedQuantity;*/


            /*if (!HttpContext.User.IsInRole("Admin") && inventory.Status == Inventory.InventoryStatus.Rejected)
            {
                return RedirectToAction("AccessDenied", "Login");
            }*/



            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Timestamp,OrderingStudent,RoboticsTeam,Vendor,PartName,SKU,Link,Quantity,Price,Barcode,Justification,Status")] Order order)
        {
            Console.WriteLine("Order edit");


            if (id != order.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {          

                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        public IActionResult SelectDelete(string partName)
        {
            Console.WriteLine("Select delete");

            Console.WriteLine(partName);

            int partID = queryForId(partName);

            var redirect = Url.Action("Delete", "Order", new { id = partID });

            return Json(new
            {
                redirectUrl = redirect
            });

            /*return RedirectToAction("Edit", "Inventory", new { id = partID });*/
        }

        // GET: Order/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            Console.WriteLine("Order delete");
            Console.WriteLine(id);

            if (id == null)
            {
                Console.WriteLine("not found 1");
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.ID == id);
            if (order == null)
            {
                Console.WriteLine("not found 2");
                return NotFound();
            }

            Console.WriteLine("found");
            return View(order);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Inventory.Any(e => e.ID == id);
        }

        public int queryForId(string partName)
        {
            Console.WriteLine("queryForId");
            Console.WriteLine(partName);

            int partID = -1;

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "tkainventoryserver.database.windows.net";
                builder.UserID = "tka";
                builder.Password = "M1dKnightRobotic$";
                builder.InitialCatalog = "coredb";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    String sql = String.Format("SELECT id FROM [dbo].[Order] WHERE PartName = '{0}'", partName);

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("reading");
                                partID = reader.GetInt32(0);
                                Console.WriteLine(partID);
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine("SQL Exception");
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("done");

            return partID;
        }
    }
}
