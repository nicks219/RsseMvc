using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DBContext;
using RandomSongSearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.BusinessLogic
{
    public static class CatalogModelExtensions
    {
        public static async Task CatalogOnGetAsync(this CatalogModel model, int id)
        {
            model.PageNumber = id;
            try
            {
                using (var scope = model._serviceScopeFactory.CreateScope())//
                {
                    var database = scope.ServiceProvider.GetRequiredService<RsseContext>();//
                    await model.CreateSongsDataCatalogViewAsync(database, model.PageNumber, model.pageSize);
                }
            }
            catch (Exception e)
            {
                model._logger.LogError(e, "[CatalogModel]");
            }
        }

        public static async Task CatalogOnPostAsync(this CatalogModel model, int id)
        {
            //Предполагается, что браузер прислал неиспорченные данные
            model.PageNumber = id;
            if (model.NavigationButtons != null && model.NavigationButtons[0] == 2)
            {
                //double r = Math.Ceiling(SongsCount / (double)pageSize);
                int pageCount = Math.DivRem(model.SongsCount, model.pageSize, out int remainder);
                if (remainder > 0)
                {
                    pageCount++;
                }
                if (model.PageNumber < pageCount)
                {
                    model.PageNumber++;
                }
            }
            if (model.NavigationButtons != null && model.NavigationButtons[0] == 1)
            {
                if (model.PageNumber > 1)
                {
                    model.PageNumber--;
                }
            }
            await model.CatalogOnGetAsync(model.PageNumber);
        }
    }
}
