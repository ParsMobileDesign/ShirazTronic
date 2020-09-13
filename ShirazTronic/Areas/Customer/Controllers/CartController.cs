using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShirazTronic.Data;
using ShirazTronic.Models;
using ShirazTronic.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ShirazTronic.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class CartController : Controller
    {
        ApplicationDbContext db;
        public CartController(ApplicationDbContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
            var cartItems = db.ShoppingCart.Include(e => e.Product).ThenInclude(p => p.Images).Where(sc => sc.AppUserId == claim.Value).ToList();

            var vmCartItems = new VmShoppingCart()
            {
                MemOrder = new MemOrder(),
                ShoppingCartItems = cartItems
            };
            vmCartItems.MemOrder.UserId = claim.Value;
            foreach (ShoppingCart item in vmCartItems.ShoppingCartItems)
                vmCartItems.MemOrder.Total += item.Count * item.Product.UnitPrice;

            return View(vmCartItems);
        }
        public IActionResult Summary()
        {
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
            var cartItems = db.ShoppingCart.Include(e => e.Product).ThenInclude(p => p.Images).Where(sc => sc.AppUserId == claim.Value).ToList();

            var vmShoppingCart = new VmShoppingCart()
            {
                MemOrder = new MemOrder(),
                ShoppingCartItems = cartItems
            };
            var user = db.AppUser.FirstOrDefault(e => e.Id == claim.Value);
            vmShoppingCart.MemOrder.UserId = claim.Value;
            vmShoppingCart.MemOrder.CustomerName = user.FName + " " + user.LName;
            vmShoppingCart.MemOrder.CuctomerPhoneNumber = user.PhoneNumber;
            foreach (ShoppingCart item in vmShoppingCart.ShoppingCartItems)
                vmShoppingCart.MemOrder.Total += item.Count * item.Product.UnitPrice;

            return View(vmShoppingCart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(VmShoppingCart shoppingCart)
        {
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
            var cartItems = db.ShoppingCart.Include(e => e.Product).Where(sc => sc.AppUserId == claim.Value).ToList();

            shoppingCart.MemOrder.UserId = claim.Value;
            shoppingCart.MemOrder.Date = new DateTime();
            // shoppingCart.MemOrder.Total
            shoppingCart.MemOrder.OrderStatus = Utility.OrderStatus_Submitted;
            shoppingCart.MemOrder.PaymentStatus = Utility.PaymentStatus_Pending;
            db.MemOrder.Add(shoppingCart.MemOrder);
            await db.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var memOrderItem = new MemOrderItem
                {
                    MemOrderId = shoppingCart.MemOrder.Id,
                    ProductId = item.ProductId,
                    Title = item.Product.Title,
                    Count = item.Count,
                    Price = item.Product.UnitPrice
                };
            }

            return View();
        }





        /// <summary>
        /// task ={ dec= Decrease , inc= increase , rem = remove}
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<IActionResult> cartChange(int cartId, string task)
        {
            var shoppingcartInDB = await db.ShoppingCart.FirstOrDefaultAsync(e => e.Id == cartId);
            if (shoppingcartInDB != null)
            {
                switch (task)
                {
                    case "dec":
                        if (shoppingcartInDB.Count > 1)
                            shoppingcartInDB.Count--;
                        break;
                    case "inc":
                        shoppingcartInDB.Count++;
                        break;
                    case "rem":
                        {
                            db.ShoppingCart.Remove(shoppingcartInDB);
                            var shoppingCartCount = db.ShoppingCart.Where(e => e.AppUser == shoppingcartInDB.AppUser).Count();
                            HttpContext.Session.SetInt32(Utility.ShoppingCartSession, shoppingCartCount);
                        }

                        break;
                }
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
