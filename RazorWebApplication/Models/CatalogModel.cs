using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
using RandomSongSearchEngine.DBContext;

namespace RandomSongSearchEngine.Models
{
    /// <summary>
    /// Каталог песен
    /// </summary>
    public class CatalogModel : BaseMvcModel
    {
        //private readonly ILogger<BasePageModel> _logger;
        public CatalogModel() { }

        public CatalogModel(IServiceScopeFactory serviceScopeFactory, ILogger<BaseMvcModel> logger) : base(serviceScopeFactory, logger)
        {
            //_logger = logger;
            //pageSize = 3;
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

        /// <summary>
        /// Количество песен на одной странице
        /// </summary>
        public readonly int pageSize = 3;
    }
}
