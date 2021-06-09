using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
using RandomSongSearchEngine.DBContext;

namespace RandomSongSearchEngine.Pages
{
    /// <summary>
    /// Каталог песен
    /// </summary>
    public class CatalogModel : BasePageModel
    {
        private readonly ILogger<CatalogModel> _logger;

        /// <summary>
        /// Количество песен на одной странице
        /// </summary>
        private readonly int pageSize;

        public CatalogModel(IServiceScopeFactory serviceScopeFactory, ILogger<CatalogModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            pageSize = 15;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())//
                {
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();//
                    SongsCount = database.Text.Count();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[CatalogModel]: no database");
                //в случае отсутствия бд мы не придём к null referenece exception из-за TitleAndTextID
                TitleAndTextID = new List<Tuple<string, int>>();
            }
        }

        /// <summary>
        /// Используется для определения какая кнопка была нажата во вьюхе
        /// </summary>
        [BindProperty]
        public List<int> NavigationButtons { get; set; }

        /// <summary>
        /// Количество песен в базе данных
        /// </summary>
        [BindProperty]
        public int SongsCount { get; set; }

        /// <summary>
        /// Номер последней просмотренной страницы
        /// </summary>
        public int PageNumber { get; set; }

        public async Task OnGetAsync(int id)
        {
            PageNumber = id;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())//
                {
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();//
                    await CreateSongsDataCatalogViewAsync(database, PageNumber, pageSize);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[CatalogModel]");
            }
        }

        /// <summary>
        /// Переход на страницу редактирования песни при клике на неё ("ChangeText")
        /// </summary>
        /// <param name="id">ID песни</param>
        /// <returns></returns>
        public IActionResult OnGetRedirect(int id)
        {
            SavedTextId = id;
            return RedirectToPage("ChangeText", new { id });
        }

        public async Task OnPostAsync(int id)
        {
            //Предполагается, что браузер прислал неиспорченные данные
            PageNumber = id;
            if (NavigationButtons != null && NavigationButtons[0] == 2)
            {
                //double r = Math.Ceiling(SongsCount / (double)pageSize);
                int pageCount = Math.DivRem(SongsCount, pageSize, out int remainder);
                if (remainder > 0)
                {
                    pageCount++;
                }
                if (PageNumber < pageCount)
                {
                    PageNumber++;
                }
            }
            if (NavigationButtons != null && NavigationButtons[0] == 1)
            {
                if (PageNumber > 1)
                {
                    PageNumber--;
                }
            }
            await OnGetAsync(PageNumber);
        }
    }
}
