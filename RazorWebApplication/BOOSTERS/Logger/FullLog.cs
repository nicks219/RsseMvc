using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RandomSongSearchEngine.Logger
{
    public static class FullLog
    {
        /// <summary>
        /// Логгирует Id модели, треда и http-контекста
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="model">Модель, из которой вызывается метод</param>
        public static void LogId (this ILogger logger, BasePageModel model)
        {
            var modelId = model.GetHashCode();
            //костыли - у модели сейчас нет контекста
            var httpContextId = "MockContext";// model.HttpContext.GetHashCode();
            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            logger.LogInformation("[Model ID: {0} Thread ID: {1} HttpContextID: {2}]", modelId, threadId, httpContextId);
        }
    }
}
