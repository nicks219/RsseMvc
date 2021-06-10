using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
using RandomSongSearchEngine.DBContext;

namespace RandomSongSearchEngine.Models
{
    /// <summary>
    /// Изменение существующей песни
    /// </summary>
    public class ChangeTextModel : BasePageModel
    {
        public ChangeTextModel() { }
        public ChangeTextModel(IServiceScopeFactory serviceScopeFactory, ILogger<BasePageModel> logger) : base(serviceScopeFactory, logger) { }

        /// <summary>
        /// Сериализованный список категорий и Id песни
        /// </summary>
        [BindProperty]
        public string InitialCheckboxesAndTextId { get; set; }

        /// <summary>
        /// Список категорий в первоначальном варианте песни
        /// </summary>
        public List<int> InitialCheckboxes { get; set; }
    }
}
