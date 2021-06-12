using System;

namespace RandomSongSearchEngine.Classes
{
    /// <summary>
    /// Моё исключение
    /// </summary>
    public class DataExistsException : Exception
    {
        public DataExistsException(string message) : base(message)
        {
        }

        public DataExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}