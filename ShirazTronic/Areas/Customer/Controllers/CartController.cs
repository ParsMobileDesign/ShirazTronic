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
using Stripe.Checkout;
using Stripe;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace ShirazTronic.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IConfiguration Configuration;
        public CartController(ApplicationDbContext _db,IConfiguration iConfig)
        {
            db = _db;
            Configuration = iConfig;
            StripeConfiguration.ApiKey = Configuration["Stripe:SecretKey"];

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
        public async Task<IActionResult> Checkout(VmShoppingCart shoppingCart, string stripeToken)
        {
            string userId = U.getUserId(this);
            var cartItems = db.ShoppingCart.Include(e => e.Product).Where(sc => sc.AppUserId == userId).ToList();

            shoppingCart.MemOrder.UserId = userId;
            shoppingCart.MemOrder.Date = new DateTime();
            shoppingCart.MemOrder.OrderStatus = U.OrderStatus_Submitted;
            shoppingCart.MemOrder.PaymentStatus = U.PaymentStatus_Pending;
            db.MemOrder.Add(shoppingCart.MemOrder);
            await db.SaveChangesAsync();
            shoppingCart.MemOrder.Total = 0;

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
                db.MemOrderItem.Add(memOrderItem);
                shoppingCart.MemOrder.Total += memOrderItem.Count * memOrderItem.Price;
            }
            //await db.SaveChangesAsync();
            db.ShoppingCart.RemoveRange(cartItems);
            HttpContext.Session.SetInt32(U.ShoppingCartSession, 0);
            await db.SaveChangesAsync();

            var option = new ChargeCreateOptions()
            {
                Amount = Convert.ToInt32(shoppingCart.MemOrder.Total * 100),
                Currency = "usd",
                Description = "Order Id:" + shoppingCart.MemOrder.Id,
                Source = stripeToken
            };

            var service = new ChargeService();
            Charge charge = service.Create(option);

            if (charge.BalanceTransactionId == null)
                shoppingCart.MemOrder.PaymentStatus = U.PaymentStatus_Rejected;
            else
                shoppingCart.MemOrder.TransactionId = charge.BalanceTransactionId;
            if(charge.Status.ToLower()=="succeeded")
            {
                shoppingCart.MemOrder.PaymentStatus = U.PaymentStatus_Approved;
                shoppingCart.MemOrder.OrderStatus = U.OrderStatus_Submitted;
            }
            else
                shoppingCart.MemOrder.PaymentStatus = U.PaymentStatus_Approved;

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Home");
        }
        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession()
        {
            string userId = U.getUserId(this);
            var cartItems = db.ShoppingCart.Include(e=>e.Product).Where(sc => sc.AppUserId == userId).ToList();
            decimal total = 0;
            foreach (var item in cartItems)
                total += item.Count * item.Product.UnitPrice;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                         PriceData = new SessionLineItemPriceDataOptions
                         {
                            UnitAmount =(long) total*100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                               Name = "Checkout your Purchase",
                            },
                         },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:44312/Customer/Cart/Checkout/",
                CancelUrl = "https://localhost:44312/Customer/Cart/DiscartCheckout/",
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return Json(new { id = session.Id });
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
                            HttpContext.Session.SetInt32(U.ShoppingCartSession, shoppingCartCount);
                        }

                        break;
                }
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
