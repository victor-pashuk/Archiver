using System;
using System.IO;

namespace GZipTestApplication
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Неправильное количество аргументов.");
                return 1;
            }

            var command = args[0].ToLower();
            var resourcePath = args[1];
            var destinationPath = args[2];

            var validator = new Validator(resourcePath, destinationPath, command);
            if (File.Exists(destinationPath))
                File.Delete(destinationPath);

            if (validator.IsValid)
            {
                var result = 0;
                var launcher = new Launcher(resourcePath, destinationPath);
                {
                    switch (command)
                    {
                        case "compress":
                            {
                                result = launcher.Launch((int)ActionEnum.Compress);
                                break;
                            }
                        case "decompress":
                            {
                                result = launcher.Launch((int)ActionEnum.Decompress);
                                break;
                            }
                    }
                }
                if (launcher.Exceptions.Count != 0)
                    foreach (var exceptionWithMessage in launcher.Exceptions)
                        Console.WriteLine(exceptionWithMessage.Message + exceptionWithMessage.Exception);
                Console.WriteLine(result);
                return result;
            }
            else
            {
                Console.WriteLine(validator.ExceptionMessage);
                return 1;
            }
        }
    }
}
