using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
using RandomSongSearchEngine.DBContext;

namespace RandomSongSearchEngine.Pages
{
    /// <summary>
    /// Изменение существующей песни
    /// </summary>
    public class ChangeTextModel : BasePageModel
    {
        private readonly ILogger<ChangeTextModel> _logger;

        public ChangeTextModel(IServiceScopeFactory serviceScopeFactory, ILogger<ChangeTextModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
        }

        /// <summary>
        /// Сериализованный список категорий и Id песни
        /// </summary>
        [BindProperty]
        public string InitialCheckboxesAndTextId { get; set; }

        /// <summary>
        /// Список категорий в первоначальном варианте песни
        /// </summary>
        private List<int> InitialCheckboxes { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            SavedTextId = id;
            try
            {
                if (SavedTextId == 0)
                {
                    return RedirectToPage("AddText");
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    await CreateTextAndTitleAsync(database, SavedTextId);
                    await CreateCheckboxesNamesAsync(database);
                    InitialCheckboxes = await CreateCheckedGenresAsync(database);
                    SerializeForView(InitialCheckboxes);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ChangeTextModel]");
            }
            return Page();
        }

        public async Task OnPostAsync(string checkboxes)
        {
            InitialCheckboxes = DeserializeFromView(checkboxes);
            try
            {
                if (AreChecked.Count == 0 || TextFromHtml == null || TitleFromHtml == null)
                {
                    await OnGetAsync(SavedTextId);
                    return;
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    await ChangeSongInDatabaseAsync(database, InitialCheckboxes);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[ChangeTextModel]");
            }
            await OnGetAsync(SavedTextId);
        }

        /// <summary>
        /// Десериализация в список категорий (и Id песни)
        /// </summary>
        /// <param name="s">Строка с сериализованными данными</param>
        /// <returns>Список категорий</returns>
        private List<int> DeserializeFromView(string s)
        {
            //Предполагается, что браузер прислал неиспорченные данные
            List<int> ints = new List<int>();
            string[] strings = s.Split(" ");
            foreach (var oneNumber in strings)
            {
                ints.Add(int.Parse(oneNumber));
            }
            SavedTextId = ints[ints.Count - 1];//[^1]
            ints.RemoveAt(ints.Count - 1);//[^1]
            return ints;
        }

        /// <summary>
        /// Сериализация из списка категорий (и Id песни)
        /// </summary>
        /// <param name="checkboxes">Список категорий</param>
        private void SerializeForView(List<int> checkboxes)
        {
            StringBuilder s = new StringBuilder();
            foreach (var i in checkboxes)
            {
                s.Append(i.ToString() + " ");
            }
            s.Append(SavedTextId.ToString());//
            InitialCheckboxesAndTextId = s.ToString().Trim();
        }

        /// <summary>
        /// Формируется список жанров, к которым принадлежит песня
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <returns>Список жанров</returns>
        private async Task<List<int>> CreateCheckedGenresAsync(DatabaseContext database)
        {
            List<int> checkedList = await database.CreateCheckedListChangeViewSql(SavedTextId).ToListAsync();
            foreach (int i in checkedList)
            {
                GenresChecked[i - 1] = "checked";
            }
            return checkedList;
        }
    }
}
