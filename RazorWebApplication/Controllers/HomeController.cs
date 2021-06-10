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
    public class HomeController : Controller
    {
        private readonly ILogger<BasePageModel> _logger;
        private readonly IServiceScopeFactory _scope;

        public HomeController(IServiceScopeFactory serviceScopeFactory, ILogger<BasePageModel> logger)
        {
            _logger = logger;
            _scope = serviceScopeFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new IndexModel(_scope, _logger);
            await model.OnGetAsync();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(IndexModel model)
        {
            model._serviceScopeFactory = _scope;
            await model.OnPostAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Catalog(int id)
        {
            var model = new CatalogModel(_scope, _logger);
            await model.OnGetAsync(id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Catalog(CatalogModel oldModel, int id)
        {
            var model = new CatalogModel(_scope, _logger);
            model.NavigationButtons = oldModel.NavigationButtons;
            //model._serviceScopeFactory = _scope;
            //model._logger = _logger;
            await model.OnPostAsync(id);
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
            await model.OnGetAsync(id);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeText(ChangeTextModel model, string checkboxes)
        {
            model._serviceScopeFactory = _scope;
            model._logger = _logger;
            await model.OnPostAsync(checkboxes);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddText()
        {
            var model = new AddTextModel(_scope, _logger);
            await model.OnGetAsync();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddText(AddTextModel model)
        {
            model._serviceScopeFactory = _scope;
            model._logger = _logger;
            await model.OnPostAsync();
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
