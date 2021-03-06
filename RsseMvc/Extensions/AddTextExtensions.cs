using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DBContext;
using RandomSongSearchEngine.Models;

namespace RandomSongSearchEngine.BusinessLogic
{
    public static class AddTextExtensions
    {
        public static async Task AddTextOnGetAsync(this AddTextModel model)
        {
            try
            {
                using (var scope = model._serviceScopeFactory.CreateScope())//
                {
                    var database = scope.ServiceProvider.GetRequiredService<RsseContext>();//

                    await model.CreateCheckboxesNamesAsync(database);
                }
            }
            catch (Exception e)
            {
                model._logger.LogError(e, "[AddTextModel]");
            }
        }

        public static async Task AddTextOnPostAsync(this AddTextModel model)
        {
            if (model.AreChecked == null || model.TextFromHtml == null || model.TitleFromHtml == null || model.AreChecked.Count == 0)
            {
                await model.AddTextOnGetAsync();
                return;
            }
            try
            {
                using (var scope = model._serviceScopeFactory.CreateScope())//
                {
                    var database = scope.ServiceProvider.GetRequiredService<RsseContext>();//

                    await model.AddSongToDatabaseAsync(database);
                    await model.CreateTextAndTitleAsync(database, model.SavedTextId);
                }
                await model.AddTextOnGetAsync();//
                model.InitCheckedGenres();
            }
            catch (Exception e)
            {
                model._logger.LogError(e, "[AddTextModel]");
                await model.AddTextOnGetAsync();//
            }
        }
    }
}
