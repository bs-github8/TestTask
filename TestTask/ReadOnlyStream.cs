using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localStream;
        private char _bufferedChar;
        private string _filePath;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            _filePath = fileFullPath;
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            get => _localStream.EndOfStream;
            //private set;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        /// <exception cref="EndOfStreamException">Попытка считать после последнего символа</exception>
        public char ReadNextChar()
        {
            // TODO : Необходимо считать очередной символ из _localStream
            var b = _localStream.Read();

            if (b == -1)
            {
                throw new EndOfStreamException();
            }

            return (char)b;
        }

        /// <summary>
        /// Закрыть поток.
        /// </summary>
        public void Close()
        {
            _localStream.Close();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            _localStream = new StreamReader(_filePath, new UTF8Encoding(false));
        }

        public void Dispose()
        {
            _localStream.Dispose();
        }
    }
}
