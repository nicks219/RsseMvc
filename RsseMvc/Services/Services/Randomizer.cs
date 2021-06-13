using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RandomSongSearchEngine.DBContext;

namespace RandomSongSearchEngine.Services
{
    /// <summary>
    /// Выборка случайной песни
    /// </summary>
    public static class Randomizer
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Возвращает ID случайно выбранной песни из выбраных категорий
        /// </summary>
        /// <param name="database">Контекст базы данных</param>
        /// <param name="areChecked">Список выбраных категорий</param>
        /// <returns></returns>
        public static async Task<int> RandomizatorAsync(this RsseContext database, List<int> areChecked)
        {
                int[] chosenOnes = areChecked.ToArray();
                int howManySongs = await database.CreateSongsListRandomizerSql(chosenOnes).CountAsync();//
                if (howManySongs == 0)
                {
                    return 0;
                }
                int coin = GetRandom(howManySongs);
                var result = await database.CreateSongsListRandomizerSql(chosenOnes).Skip(coin).Take(1).FirstAsync();//
                return result;
        }

        /// <summary>
        /// Потокобезопасная генерация случайного числа в заданном диапазоне
        /// </summary>
        /// <param name="howMany">Количество песен, доступных для выборки</param>
        /// <returns></returns>
        private static int GetRandom(int howMany)
        {
            lock (random)
            {
                int coin = random.Next(0, howMany);
                return coin;
            }
        }
    }
}