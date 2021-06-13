using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Services;
using RandomSongSearchEngine.DBContext;
using RandomSongSearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.BusinessLogic
{
    public static class IndexModelExtensions
    {
        public static async Task IndexOnGetAsync(this IndexModel model)
        {
            try
            {
                using (var scope = model._serviceScopeFactory.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
                    await model.CreateCheckboxesNamesAsync(database: database);
                }
                model.InitCheckedGenres();
            }
            catch (Exception e)
            {
                model._logger.LogError(e, "[IndexModel: OnGet Error]");
            }
        }

        public static async Task IndexOnPostAsync(this IndexModel model)
        {
            try
            {
                await model.GetRandomSongAsync();
                await model.IndexOnGetAsync();
            }
            catch (Exception e)
            {
                model._logger.LogError(e, "[IndexModel: OnPost Error]");
            }
        }

        /// <summary>
        /// Выбирает из базы данных случайную песню, относящуюся к заданным категориям
        /// </summary>
        private static async Task GetRandomSongAsync(this IndexModel model)
        {
            if (model.AreChecked == null || model.AreChecked.Count == 0)
            {
                return;
            }
            using (var scope = model._serviceScopeFactory.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<RsseContext>();
                int randomResult = await database.RandomizatorAsync(model.AreChecked);
                if (randomResult == 0)
                {
                    return;
                }
                await model.CreateTextAndTitleAsync(database, randomResult);
                model.SavedTextId = randomResult;
            }
        }
    }
}
