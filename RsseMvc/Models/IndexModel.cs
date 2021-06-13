using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Services;
using RandomSongSearchEngine.DBContext;

namespace RandomSongSearchEngine.Models
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
    public class IndexModel : BaseMvcModel
    {
        public IndexModel() { }
        public IndexModel(IServiceScopeFactory serviceScopeFactory, ILogger<BaseMvcModel> logger) : base(serviceScopeFactory, logger) { }
    }
}

