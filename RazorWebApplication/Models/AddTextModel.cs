using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
using System;
using System.Threading.Tasks;
using RandomSongSearchEngine.DBContext;
using Microsoft.Extensions.DependencyInjection;

namespace RandomSongSearchEngine.Models
{
    /// <summary>
    /// Добавление новой песни
    /// </summary>
    public class AddTextModel : BasePageModel
    {
        public AddTextModel() { }
        public AddTextModel(IServiceScopeFactory serviceScopeFactory, ILogger<BasePageModel> logger) : base(serviceScopeFactory, logger) { }
    }
}
