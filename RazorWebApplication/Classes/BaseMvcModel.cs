using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DBContext;

namespace RandomSongSearchEngine.Classes
{
    /// <summary>
    /// Базовый класс для моделей RazorPage
    /// </summary>
    public class BaseMvcModel //: PageModel
    {
        //protected readonly IServiceScopeFactory _serviceScopeFactory;
        public IServiceScopeFactory _serviceScopeFactory;
        public ILogger<BaseMvcModel> _logger;

        public BaseMvcModel() { }

        //костыль, в финале удалить
        protected BaseMvcModel(IServiceScopeFactory serviceScopeFactory) { }

        public BaseMvcModel (IServiceScopeFactory serviceScopeFactory, ILogger<BaseMvcModel> logger)
        {
            //Console.Clear();
            GenresChecked = new List<string>();
            GenresNames = new List<string>();
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Заголовок песни из вьюхи
        /// </summary>
        [BindProperty]
        public string TitleFromHtml { get; set; }

        /// <summary>
        /// Текст песни из вьюхи
        /// </summary>
        [BindProperty]
        public string TextFromHtml { get; set; }

        /// <summary>
        /// Список выбраных чекбоксов из вьюхи
        /// </summary>
        [BindProperty]
        public List<int> AreChecked { get; set; }

        /// <summary>
        /// Список из названий песен и соответствующим им ID для CatalogView
        /// </summary>
        [BindProperty]
        public List<Tuple<string, int>> TitleAndTextID { get; set; }


        /// <summary>
        /// Сохраненный ID текста для перехода между вьюхами
        /// </summary>
        [BindProperty]
        public int SavedTextId { get; set; } = 3;

        /// <summary>
        /// Текст песни для вьюхи
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Заголовок песни для вьюхи
        /// </summary>
        public string MessageTitle { get; set; }

        /// <summary>
        /// Список с названиями жанров песен
        /// </summary>
        public List<string> GenresNames { get; set; }

        /// <summary>
        /// Количество жанров песен
        /// </summary>
        public int GenresCount { get; set; }

        /// <summary>
        /// Список строк "checked" или "unchecked" для создания чекбоксов во вьюхе
        /// </summary>
        public List<string> GenresChecked { get; set; }

        /// <summary>
        /// Модель базы данных с песнями, жанрами и их связью
        /// </summary>
        protected DatabaseContext Database { get; set; }

        /// <summary>
        /// Изменение отмеченных жанров в списке на "checked"
        /// </summary>
        public void InitCheckedGenres()
        {
            if (AreChecked != null)
            //{
            //    foreach (int i in GenresIdChecked)
            //    {
            //        GenresChecked[i - 1] = "checked";
            //    }
            //}
            //else
            {
                foreach (int i in AreChecked)
                {
                    GenresChecked[i - 1] = "checked";
                }
            }
        }

        /// <summary>
        /// Инициализация списка жанров с пометкой "unchecked"
        /// </summary>
        protected void InitUncheckedGenres()
        {

            GenresChecked = new List<string>();//

            for (int i = 0; i < GenresCount; i++)
            {
                GenresChecked.Add("unchecked");
            }
        }

        /// <summary>
        /// Изменение песни
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="initialCheckboxes">Новый список отмеченных во вьюхе жанров</param>
        /// <returns></returns>
        public async Task ChangeSongInDatabaseAsync(DatabaseContext database, List<int> initialCheckboxes)
        {
            DataTransfer dt = new DataTransfer
            {
                TitleFromHtml = TitleFromHtml.Trim(),
                TextFromHtml = TextFromHtml.Trim(),
                AreChecked = AreChecked,
                SavedTextId = SavedTextId
            };
            //тут можно проверить имя на свопадение с существующим. редкая ошибка
            //if (ModelState.IsValid)
            {
                await database.ChangeSongInDatabaseAsync(initialCheckboxes, dt);
            }
        }

        /// <summary>
        /// Добавление песни
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <returns></returns>
        public async Task AddSongToDatabaseAsync(DatabaseContext database)
        {
            DataTransfer dt = new DataTransfer
            {
                TitleFromHtml = TitleFromHtml.Trim(),
                TextFromHtml = TextFromHtml.Trim(),
                AreChecked = AreChecked
            };
            //if (ModelState.IsValid)
            {
                //Получим 0 при ошибке
                SavedTextId = await database.AddSongToDatabaseAsync(dt);
            }
        }

        /// <summary>
        /// Создание заголовка и текста песни
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="textId">ID песни</param>
        /// <returns></returns>
        public async Task CreateTextAndTitleAsync(DatabaseContext database, int textId)
        {
            //метод оставляет на экране ранее введенный текст в случае большинства исключений
            var r = await database.CreateTitleAndTextSql(textId).ToListAsync();
            if (r.Count > 0)
            {
                Message = r[0].Item1;
                MessageTitle = r[0].Item2;
            }
            else
            {
                Message = TextFromHtml;
                MessageTitle = TitleFromHtml;
            }
        }

        /// <summary>
        /// Создание списка жанров с количеством песен для чекбоксов
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <returns></returns>
        public async Task CreateCheckboxesNamesAsync(DatabaseContext database)
        {
            List<Tuple<string, int>> genresNames = await database.CreateCheckboxesDataSql().ToListAsync();

            GenresNames = new List<string>();//

            foreach (var r in genresNames)
            {
                if (r.Item2 > 0)
                {
                    GenresNames.Add(r.Item1 + ": " + r.Item2);
                }
                else
                {
                    GenresNames.Add(r.Item1);
                }
            }
            GenresCount = GenresNames.Count;
            InitUncheckedGenres();
        }

        /// <summary>
        /// Создание заголовков песен и их ID для CatalogModel
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="savedLastViewedPage">Текущая страница каталога</param>
        /// <param name="pageSize">Количество песен на странице</param>
        /// <returns></returns>
        public async Task CreateSongsDataCatalogViewAsync(DatabaseContext database, int savedLastViewedPage, int pageSize)
        {
            TitleAndTextID = await database.CreateSongsDataCatalogViewSql(savedLastViewedPage, pageSize).ToListAsync();
            SavedTextId = TitleAndTextID[0].Item2;
        }
    }
}
