using System.Collections.Generic;

namespace RandomSongSearchEngine.DBEntities
{
    /// <summary>
    /// Строка таблицы бд с текстами песен
    /// </summary>
    public class TextEntity
    {
        public int TextID { get; set; }

        public string Title { get; set; }

        public string Song { get; set; }

        public ICollection<GenreTextEntity> GenreTextInText { get; set; }
    }
}