using System;
using System.IO;
using System.Text;
using Monitoring.Infrastructure.RomEditor;

namespace Monitoring.Routines.RomTester
{
    class Program
    {
        private const string InputFile = "input.rom";
        static void Main(string[] args)
        {
            var biosEditor = new BiosEditor();
            Console.WriteLine("Open rom file");

            try
            {
                using (var fs = File.Open(InputFile, FileMode.Open))
                {
                    biosEditor.Open(fs);

                    Console.WriteLine(biosEditor.BiosBootUpMessage);
                    var name = Guid.NewGuid().ToString();
                    Console.WriteLine($"New name {name}");
                    biosEditor.BiosBootUpMessage = name;

                    var output = biosEditor.Save();
                    using (var fileStream = File.Create("output.rom"))
                    {
                        output.Seek(0, SeekOrigin.Begin);
                        output.CopyTo(fileStream);
                    }
                    Console.WriteLine("New rom file saved");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
