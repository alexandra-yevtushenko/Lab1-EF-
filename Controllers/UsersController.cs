using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pineapple_shopModel;

namespace PineappleShop.Controllers
{
    public class UsersController : Controller
    {
        private readonly pineapple_shopModel.pineapple_shopModel _context;
        private User _currentUser;
        public UsersController(pineapple_shopModel.pineapple_shopModel context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            HttpContextAccessor http = new HttpContextAccessor();
            try
            {
                var claims = http.HttpContext.User.Claims;
                var login = claims.ToList()[0].Value;
                var user = _context.Users.FirstOrDefault(user => user.Login == login);
                if (user.StatusId == 0)
                    return View(_context.Users.ToList());
                else
                    return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult Delivery()
        {

            List<pineapple_shopModel.DeliveryInfo> DeliveryInfo =  _context.DeliveryInfos
                .Include(d => d.Staff)
                .Include(d => d.User).ToList();
            return View(DeliveryInfo);
        }
        // GET: Users/Details/5

        public IActionResult EditPassword(int id)
        {
            _currentUser = _context.Users.FirstOrDefault(user => user.Id == id);
            return View("ChangePassword", _context.Users.FirstOrDefault(user => user.Id == id));
        }
        public ActionResult Details(int id)
        {
            return View(_context.Users.FirstOrDefault(user => user.Id == id));
        }

        public async Task<IActionResult> ChangePassword([Bind("Id, PasswordHash")] User user, string confirm_password)
        {
            if (ModelState.IsValid)
            {
                _currentUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
                bool passwordsAreDifferent = (confirm_password != user.PasswordHash);

                if ((passwordsAreDifferent))
                {
                    if (passwordsAreDifferent)
                        ModelState.AddModelError("Error", "Passwords are different. Please try again.");
                }
                else
                {
                    _currentUser.PasswordHash = testcore.Controllers.AuthController.getHashSha256(confirm_password);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View("ChangePassword", _currentUser);
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            return View(_context.Users.FirstOrDefault(user => user.Id == id));
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                _context.Users.FirstOrDefault(user => user.Id == id).Id = Convert.ToInt32(collection.FirstOrDefault(item => item.Key == "Id").Value[0]);
                _context.Users.FirstOrDefault(user => user.Id == id).Login = collection.FirstOrDefault(item => item.Key == "Login").Value[0];
                _context.Users.FirstOrDefault(user => user.Id == id).PasswordHash = collection.FirstOrDefault(item => item.Key == "PasswordHash").Value[0];
                _context.Users.FirstOrDefault(user => user.Id == id).Email = collection.FirstOrDefault(item => item.Key == "Email").Value[0];
                _context.Users.FirstOrDefault(user => user.Id == id).StatusId = Convert.ToInt32(collection.FirstOrDefault(item => item.Key == "StatusId").Value[0]);
                _context.Users.FirstOrDefault(user => user.Id == id).RegDate = Convert.ToDateTime(collection.FirstOrDefault(item => item.Key == "RegDate").Value[0]);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_context.Users.FirstOrDefault(user => user.Id == id));
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(user => user.Id == id);
                _context.Remove(user);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}