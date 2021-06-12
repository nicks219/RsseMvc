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
    public class AddTextModel : BaseMvcModel
    {
        public AddTextModel() { }
        public AddTextModel(IServiceScopeFactory serviceScopeFactory, ILogger<BaseMvcModel> logger) : base(serviceScopeFactory, logger) { }
    }
}
