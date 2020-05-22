using System.IO;
using System.Text;

namespace GZipTestApplication
{
    public class Validator
    {
        public bool IsValid => Validate();
        public StringBuilder ExceptionMessage { get; }

        private readonly string _resourcePath;
        private readonly string _destinationPath;
        private readonly string _command;

        public Validator(string resourcePath, string destinationPath, string command)
        {
            _resourcePath = resourcePath;
            _destinationPath = destinationPath;
            _command = command;
            ExceptionMessage = new StringBuilder();
        }

        private bool ValidationResourceFile()
        {
            bool result;
            if (_command == "decompress")
                result = _resourcePath.EndsWith(".gz") && File.Exists(_resourcePath);
            else
                result = File.Exists(_resourcePath);
            return result;
        }

        private bool ValidateDestinationPath()
        {
            bool result;
            if (_command == "compress")
                result = Directory.Exists(Path.GetDirectoryName(_destinationPath)) && _destinationPath.EndsWith(".gz");
            else
                result = Directory.Exists(Path.GetDirectoryName(_destinationPath));
            return result;
        }

        private bool Validate()
        {
            var isValidDestinationPath = ValidateDestinationPath();
            var isValidResourcePath = ValidationResourceFile();
            if (isValidResourcePath && isValidDestinationPath)
                return true;
            else
            {
                if (!isValidResourcePath)
                    ExceptionMessage.Append("Исходный файл не найден, проверьте корректность введеного пути.\n");
                if (!isValidDestinationPath)
                    ExceptionMessage.Append("Некорректный путь к файлу назначения.");
                return false;
            }

        }




    }
}
