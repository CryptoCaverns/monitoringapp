using System;
using System.IO;
using System.Text;
using Monitoring.Infrastructure.RomEditor;

namespace Monitoring.Routines.RomTester
{
    class Program
    {
        private const string Dir = "382";
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(Dir);

            foreach (var file in files)
            {
                var biosEditor = new BiosEditor();
                Console.WriteLine($"Open rom file {file}");

                try
                {
                    using (var fs = File.Open($"{file}", FileMode.Open))
                    {
                        biosEditor.Open(fs);
                        biosEditor.BiosBootUpMessage = "24D7E9BF-61BF-403B-A840-1A7451FC493A";

                        var output = biosEditor.Save();
                        using (var fileStream = File.Create($"o/{file}"))
                        {
                            output.Seek(0, SeekOrigin.Begin);
                            output.CopyTo(fileStream);
                        }
                        Console.WriteLine($"New rom file saved {file}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.ReadKey();
        }
    }
}
