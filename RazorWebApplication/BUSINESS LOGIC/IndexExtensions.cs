﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
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
        public static async Task OnGetAsync(this IndexModel model)
        {
            try
            {
                using (var scope = model._serviceScopeFactory.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    await model.CreateCheckboxesNamesAsync(database: database);
                }
                model.InitCheckedGenres();
            }
            catch (Exception e)
            {
                model._logger.LogError(e, "[IndexModel: OnGet Error]");
            }
        }

        public static async Task OnPostAsync(this IndexModel model)
        {
            try
            {
                await model.GetRandomSongAsync();
                await model.OnGetAsync();
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
                var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
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