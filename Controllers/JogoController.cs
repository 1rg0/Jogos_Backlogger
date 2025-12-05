using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jogos_Backlogger.Controllers
{
    public class JogoController : Controller
    {
        // GET: JogoController
        public ActionResult Index()
        {
            return View();
        }

        // GET: JogoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: JogoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: JogoController/Create
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

        // GET: JogoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: JogoController/Edit/5
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

        // GET: JogoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: JogoController/Delete/5
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
