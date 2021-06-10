using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
using RandomSongSearchEngine.DBContext;
using RandomSongSearchEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.BusinessLogic
{
    public static class ChangeTextExtensinos
    {
        public static async Task ChangeTextOnGetAsync(this ChangeTextModel model, int id)
        {
            model.SavedTextId = id;
            try
            {
                if (model.SavedTextId == 0)
                {
                    //вот хер помню нахуя это надо
                    //return RedirectToPage("AddText");
                    throw new NotImplementedException();
                }
                using (var scope = model._serviceScopeFactory.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    await model.CreateTextAndTitleAsync(database, model.SavedTextId);
                    await model.CreateCheckboxesNamesAsync(database);
                    model.InitialCheckboxes = await model.CreateCheckedGenresAsync(database);
                    model.SerializeForView(model.InitialCheckboxes);
                }
            }
            catch (Exception e)
            {
                model._logger.LogError(e, "[ChangeTextModel]");
            }
        }

        public static async Task ChangeTextOnPostAsync(this ChangeTextModel model, string checkboxes)
        {
            model.InitialCheckboxes = model.DeserializeFromView(checkboxes);
            try
            {
                if (model.AreChecked == null || model.TextFromHtml == null || model.TitleFromHtml == null || model.AreChecked.Count == 0)
                {
                    await model.ChangeTextOnGetAsync(model.SavedTextId);
                    return;
                }

                using (var scope = model._serviceScopeFactory.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    await model.ChangeSongInDatabaseAsync(database, model.InitialCheckboxes);

                }
            }
            catch (Exception e)
            {
                model._logger.LogError(e, "[ChangeTextModel]");
            }
            await model.ChangeTextOnGetAsync(model.SavedTextId);
        }

        /// <summary>
        /// Десериализация в список категорий (и Id песни)
        /// </summary>
        /// <param name="s">Строка с сериализованными данными</param>
        /// <returns>Список категорий</returns>
        private static List<int> DeserializeFromView(this ChangeTextModel model, string s)
        {
            //Предполагается, что браузер прислал неиспорченные данные
            List<int> ints = new List<int>();
            string[] strings = s.Split(" ");
            foreach (var oneNumber in strings)
            {
                ints.Add(int.Parse(oneNumber));
            }
            model.SavedTextId = ints[ints.Count - 1];//[^1]
            ints.RemoveAt(ints.Count - 1);//[^1]
            return ints;
        }

        /// <summary>
        /// Сериализация из списка категорий (и Id песни)
        /// </summary>
        /// <param name="checkboxes">Список категорий</param>
        private static void SerializeForView(this ChangeTextModel model, List<int> checkboxes)
        {
            StringBuilder s = new StringBuilder();
            foreach (var i in checkboxes)
            {
                s.Append(i.ToString() + " ");
            }
            s.Append(model.SavedTextId.ToString());//
            model.InitialCheckboxesAndTextId = s.ToString().Trim();
        }

        /// <summary>
        /// Формируется список жанров, к которым принадлежит песня
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <returns>Список жанров</returns>
        private static async Task<List<int>> CreateCheckedGenresAsync(this ChangeTextModel model, DatabaseContext database)
        {
            List<int> checkedList = await database.CreateCheckedListChangeViewSql(model.SavedTextId).ToListAsync();
            foreach (int i in checkedList)
            {
                model.GenresChecked[i - 1] = "checked";
            }
            return checkedList;
        }
    }
}
