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

namespace DotNetCoreSqlDb.Controllers
{
    [Authorize(Roles = "Team")]
    public class InventoryController : Controller
    {
        private readonly MyDatabaseContext _context;

        public InventoryController(MyDatabaseContext context)
        {
            _context = context;
        }

        // GET: Inventory
        [Authorize(Roles = "Team")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Inventory.ToListAsync());
        }

        public IActionResult SelectDetails(string partName)
        {
            Console.WriteLine("Select details");

            Console.WriteLine(partName);

            int partID = queryForId(partName);

            var redirect = Url.Action("Details", "Inventory", new { id = partID });

            return Json(new
            {
                redirectUrl = redirect
            });

            /*return RedirectToAction("Edit", "Inventory", new { id = partID });*/
        }

        // GET: Inventory/Details/5
        [Authorize(Roles = "Team")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // GET: Inventory/Details/5
        [Authorize(Roles = "Team")]
        public async Task<IActionResult> BarcodeEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }

            Console.WriteLine("barcode edit 1");
            //Console.WriteLine(inventory.Quantity);

            return View(inventory);
        }

        // GET: Inventory/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Team")]
        public async Task<IActionResult> BarcodeEdit(int id, [Bind("ID,PartName,PartType,SKU,Quantity,SuggestedQuantity,MinimumQuantity,UnitCost,Location,Barcode,Status")] Inventory inventory)
        {
            /*if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                return NotFound();
            }*/

            Console.WriteLine("barcode edit 2");
            Console.WriteLine(id);
            Console.WriteLine(inventory.ID);
            Console.WriteLine(inventory.Quantity);

            if (ModelState.IsValid)
            {
                _context.Update(inventory);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Inventory/Create
        [Authorize(Roles = "Business")]
        public IActionResult Create()
        {
            Console.WriteLine("Inventory create");

            return View();
        }

        // POST: Inventory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Create([Bind("ID,PartName,PartType,SKU,Quantity,SuggestedQuantity,MinimumQuantity,UnitCost,Location,Barcode,Status")] Inventory inventory)
        {
            Console.WriteLine(inventory.ID);

            if (ModelState.IsValid)
            {
                if(HttpContext.User.IsInRole("Admin"))
                {
                    inventory.Status = Inventory.InventoryStatus.Approved;
                } else
                {
                    inventory.Status = Inventory.InventoryStatus.Submitted;
                }
                
                Console.WriteLine("status");
                Console.WriteLine(inventory.Status);


                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventory);
        }

        public IActionResult SelectEdit(string partName)
        {
            Console.WriteLine("Select edit");

            Console.WriteLine(partName);

            int partID = queryForId(partName);

            var redirect = Url.Action("Edit", "Inventory", new { id = partID });

            return Json(new
            {
                redirectUrl = redirect
            });

            /*return RedirectToAction("Edit", "Inventory", new { id = partID });*/
        }

        // GET: Inventory/Edit/5
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Edit(int? id)
        {

            Console.WriteLine("edit!");

            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventory.FindAsync(id);
            ViewData["status"] = inventory.Status;

            ViewData["quantity"] = inventory.Quantity;
            ViewData["suggestedQuantity"] = inventory.SuggestedQuantity;


            if (!HttpContext.User.IsInRole("Admin") && inventory.Status == Inventory.InventoryStatus.Rejected)
            {
                return RedirectToAction("AccessDenied", "Login");
            }



            if (inventory == null)
            {
                return NotFound();
            }
            return View(inventory);
        }

        // POST: Inventory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,PartName,PartType,SKU,Quantity,SuggestedQuantity,MinimumQuantity,UnitCost,Location,Barcode,Status")] Inventory inventory)
        {
            Console.WriteLine("Inventory edit");

            /*inventory.PartName = (string)ViewData["partName"];
            

            inventory.Quantity = (int)ViewData["quantity"];
            inventory.UnitCost = (decimal)ViewData["unitCost"];*/

            Console.WriteLine("ID");
            Console.WriteLine("PartName");
            Console.WriteLine(inventory.PartName);
            Console.WriteLine("PartType");
            Console.WriteLine("SKU");
            Console.WriteLine("Quantity");
            Console.WriteLine("SuggestedQuantity");
            Console.WriteLine(inventory.SuggestedQuantity);
            Console.WriteLine("MinimumQuantity");
            Console.WriteLine("UnitCost");
            Console.WriteLine("Location");
            Console.WriteLine("Status");

            if (id != inventory.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (HttpContext.User.IsInRole("Admin") && inventory.Status == Inventory.InventoryStatus.Submitted)
                {
                    inventory.Status = Inventory.InventoryStatus.Approved;
                }

                //if (HttpContext.User.IsInRole("Admin"))
                //{
                //    inventory.SuggestedQuantity = null;
                //}
                

                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventory.ID))
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
            return View(inventory);
        }

        public IActionResult SelectDelete(string partName)
        {
            Console.WriteLine("Select delete");

            Console.WriteLine(partName);

            int partID = queryForId(partName);

            var redirect = Url.Action("Delete", "Inventory", new { id = partID });

            return Json(new
            {
                redirectUrl = redirect
            });

            /*return RedirectToAction("Edit", "Inventory", new { id = partID });*/
        }

        // GET: Inventory/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            Console.WriteLine("Inventory delete");
            Console.WriteLine(id);

            if (id == null)
            {
                Console.WriteLine("not found 1");
                return NotFound();
            }

            var inventory = await _context.Inventory
                .FirstOrDefaultAsync(m => m.ID == id);
            if (inventory == null)
            {
                Console.WriteLine("not found 2");
                return NotFound();
            }

            Console.WriteLine("found");
            return View(inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventory = await _context.Inventory.FindAsync(id);
            _context.Inventory.Remove(inventory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(int id)
        {
            return _context.Inventory.Any(e => e.ID == id);
        }

        public int queryForId(string partName)
        {
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
                    String sql = String.Format("SELECT id FROM [dbo].[Inventory] WHERE PartName = '{0}'", partName);

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

        public int queryForBarcode(string barcode)
        {
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
                    String sql = String.Format("SELECT id FROM [dbo].[Inventory] WHERE barcode = '{0}'", barcode);

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

        // GET: Inventory/Barcode
        [Authorize(Roles = "Business")]
        public IActionResult Barcode()
        {
            Console.WriteLine("Barcode");

            return View();
        }

        // POST: Inventory/Barcode
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> Barcode(int id, [Bind("ID,Barcode")] Inventory inventory)
        {
            Console.WriteLine("BARCODE");
            Console.WriteLine(id);
            Console.WriteLine(inventory.ID);
            Console.WriteLine(inventory.Barcode);

            int partID = queryForBarcode(inventory.Barcode);
            Console.WriteLine(partID);

            if (partID != -1)
            {
                ViewData["barcodeStatus"] = "found";
                //return RedirectToAction("Details", "Inventory", new { id = PartID });
                Console.WriteLine("go to BarcodeEdit");
                return RedirectToAction("BarcodeEdit", "Inventory", new { id = partID });

                /*
                var redirect = Url.Action("Details", "Inventory", new { id = partID });

                return Json(new
                {
                    redirectUrl = redirect
                });
                */
            }
            else
            {
                ViewData["barcodeStatus"] = "missing";
            }

            return View(inventory);
        }
    }
}
