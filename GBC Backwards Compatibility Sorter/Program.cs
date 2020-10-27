
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GBC_Backwards_Compatibility_Sorter
{
    class Program
    {
        private const int ERROR_BAD_ARGUMENTS = 0xA0;

        private const string CGB_EXTENSION = ".gbc";
        private const long CGB_FLAG = 0x143;
        private const long CGB_FLAG_COMPATIBLE = 0x80;
        private const long CGB_FLAG_INCOMPATIBLE = 0xC0;
        private const string DIRECTORY_COMPATIBLE = "gb_compatible";
        private const string DIRECTORY_INCOMPATIBLE = "gb_incompatible";
        private static readonly string[] IGNORED_DIRECTORIES = { DIRECTORY_COMPATIBLE, DIRECTORY_INCOMPATIBLE };
        private static readonly ConsoleKey[] ALLOWED_INPUTS = { ConsoleKey.Y, ConsoleKey.N };

        static int Main(string[] args)
        {
            string currentPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            try
            {
                if (0 < args.Length)
                {
                    if (Directory.Exists(args[0]))
                    {
                        currentPath = args[0];
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Invalid path given");
                return ERROR_BAD_ARGUMENTS;
            }

            ConsoleKeyInfo input = new ConsoleKeyInfo();
            while (!ALLOWED_INPUTS.Contains(input.Key))
            {
                Console.Clear();
                Console.WriteLine("Going to sort in \"{0}\"", currentPath);
                Console.WriteLine("Sort subdirectories? Folders \"gb_compatible\" and \"gb_incompatible\" are excluded. (Y/N)");
                input = Console.ReadKey();
            }
            Console.WriteLine();
            romSort(currentPath, input.Key == ConsoleKey.Y);
            return 0;
        }

        private static void moveFile(string file, string subDirectory)
        {
            string path = String.Format(@"{0}\{1}", Path.GetDirectoryName(file), subDirectory);
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = Path.GetFileName(file);
            Console.WriteLine(String.Format("Move file \"{0}\" to \"{1}\"", fileName, path));
            File.Move(file, String.Format(@"{0}\{1}", path, fileName));
        }

        private static void romSort(string path, bool recursive)
        {
            var files = from file in Directory.EnumerateFiles(path) where Path.GetExtension(file) == CGB_EXTENSION select file;
            foreach (string file in files)
            {
                if(Path.GetExtension(file) == CGB_EXTENSION)
                {
                    int flagValue = 0;
                    using (FileStream fs = File.OpenRead(file))
                    {
                        fs.Seek(CGB_FLAG, SeekOrigin.Begin);
                        flagValue = fs.ReadByte();
                    }
                    if (flagValue == CGB_FLAG_COMPATIBLE)
                    {
                        moveFile(file, DIRECTORY_COMPATIBLE);
                    }
                    else if (flagValue == CGB_FLAG_INCOMPATIBLE)
                    {
                        moveFile(file, DIRECTORY_INCOMPATIBLE);
                    }
                }
            }

            if(recursive)
            {
                var subDirectories = from dir in Directory.EnumerateDirectories(path) where !IGNORED_DIRECTORIES.Contains(Path.GetFileName(dir)) select dir;
                foreach(string subDirectory in subDirectories)
                {
                    romSort(subDirectory, true);
                }
            }
        }
    }
}