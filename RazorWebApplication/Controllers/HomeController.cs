using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DBContext;
using RandomSongSearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RandomSongSearchEngine.Classes;
using RandomSongSearchEngine.Logger;
using RandomSongSearchEngine.BusinessLogic;

namespace RandomSongSearchEngine.Controllers
{
    //MVC возвращает из вьюхи модель с null во всех полях кроме специально заполненных
    //поэтому в HttpPost я пересоздаю scope и logger у моделек для сохранения работоспособности
    public class HomeController : Controller
    {
        private readonly ILogger<BaseMvcModel> _logger;
        private readonly IServiceScopeFactory _scope;

        public HomeController(IServiceScopeFactory serviceScopeFactory, ILogger<BaseMvcModel> logger)
        {
            _logger = logger;
            _scope = serviceScopeFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new IndexModel(_scope, _logger);
            await model.IndexOnGetAsync();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(IndexModel model)
        {
            model._serviceScopeFactory = _scope;
            await model.IndexOnPostAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Catalog(int id)
        {
            var model = new CatalogModel(_scope, _logger);
            await model.CatalogOnGetAsync(id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Catalog(CatalogModel oldModel, int id)
        {
            var model = new CatalogModel(_scope, _logger);
            model.NavigationButtons = oldModel.NavigationButtons;
            //model._serviceScopeFactory = _scope;
            //model._logger = _logger;
            await model.CatalogOnPostAsync(id);
            return View(model);
        }
        /// <summary>
        /// Переход на страницу редактирования песни при клике на неё в каталоге ("ChangeText")
        /// </summary>
        /// <param name="id">ID песни</param>
        /// <returns></returns>
        public IActionResult Redirect(int id)
        {
            return RedirectToAction(nameof(ChangeText), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> ChangeText(int id)
        {
            var model = new ChangeTextModel(_scope, _logger);
            await model.ChangeTextOnGetAsync(id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeText(ChangeTextModel model, string checkboxes)
        {
            model._serviceScopeFactory = _scope;
            model._logger = _logger;
            await model.ChangeTextOnPostAsync(checkboxes);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddText()
        {
            var model = new AddTextModel(_scope, _logger);
            await model.AddTextOnGetAsync();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddText(AddTextModel model)
        {
            model._serviceScopeFactory = _scope;
            model._logger = _logger;
            await model.AddTextOnPostAsync();
            if (model.SavedTextId == 0)
            {
                //чет не так, скорее всего песня с ткаим названием уже есть
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}

//public HomeController(IServiceProvider services)
//{
//    var service1 = services.GetService<DatabaseContext>();
//    var service2 = services.GetService<ILoggerProvider>();
//    _logger = (ILogger<IndexModel>)service2.CreateLogger("IndexModel");
//}
