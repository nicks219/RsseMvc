using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
using RandomSongSearchEngine.DBContext;

namespace RandomSongSearchEngine.Pages
{
    //TODO убери js из layout
    //TODO при каждом создании BasePageModel происходит injected context контекста Pasha который не уничтожается - может это и жрёт память? (нет)
    //TODO разберись с transparent
    //TODO удаляй каменты
    //TODO вынеси цсс из layout
    //TODO удали (?) error, validationScript, viewImoprt, viewStart
    //TODO appsettings.json "Microsoft": "Warning" instead of "Information"
    //TODO ставь решарпер и асинк-плагин: get brains (см. stepik) до 7 ноября
    //1. поля (статика сверху) - конструктор - свойства - методы
    //2. паблик-протектед-приват (свойства и методы)
    //3. документация на паблик и протектед

    //NEW TODO
    //catalog Math.DivRem()
    /// <summary>
    /// Вывод случайной песни по выбранным категориям
    /// </summary>
    public class IndexModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IServiceScopeFactory serviceScopeFactory, ILogger<IndexModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            //_logger.LogId(this);
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    await CreateCheckboxesNamesAsync(database: database);
                }
                InitCheckedGenres();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[IndexModel: OnGet Error]");
            }
        }

        public async Task OnPostAsync()
        {
            try
            {
                await GetRandomSongAsync();
                await OnGetAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[IndexModel: OnPost Error]");
            }
        }

        /// <summary>
        /// Выбирает из базы данных случайную песню, относящуюся к заданным категориям
        /// </summary>
        private async Task GetRandomSongAsync()
        {
            if (AreChecked.Count == 0)
            {
                return;
            }
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                int randomResult = await database.RandomizatorAsync(AreChecked);
                if (randomResult == 0)
                {
                    return;
                }
                await CreateTextAndTitleAsync(database, randomResult);
                SavedTextId = randomResult;
            }
        }
    }
}
