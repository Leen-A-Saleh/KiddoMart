using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace All_Baby_Essentials.Areas.Customer.Controllers
{
        [Area("Customer")]
    public class TestimonialController : Controller
    {
        // GET: TestimonialController
        public ActionResult Index()
        {
            return View();
        }

        // GET: TestimonialController/Details/5
        public ActionResult TestimonialsByID(int id)
        {
            return View();
        }

        // GET: TestimonialController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestimonialController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TestimonialController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TestimonialController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TestimonialController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TestimonialController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
