using BOOSE;

namespace boosecli;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help")
        {
            PrintHelp();
            return 0;
        }

        var inputPath = args[0];
        if (!File.Exists(inputPath))
        {
            Console.Error.WriteLine($"File not found: {inputPath}");
            return 2;
        }

        // Optional: --png output.png
        string? pngOut = null;
        for (int i = 1; i < args.Length; i++)
        {
            if (args[i] is "--png" && i + 1 < args.Length)
            {
                pngOut = args[i + 1];
                i++;
            }
        }

        var programText = File.ReadAllText(inputPath);

        using var canvas = new BitmapCanvas();
        var interpreter = new BooseInterpreter(canvas, msg => Console.WriteLine(msg));

        try
        {
            var result = interpreter.Execute(programText);

            if (!string.IsNullOrWhiteSpace(pngOut))
            {
                canvas.SavePng(pngOut);
                Console.WriteLine($"Saved drawing to: {pngOut}");
            }

            Console.WriteLine($"Statements executed: {result.StatementsExecuted}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("ERROR: " + ex.Message);
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("BOOSE CLI Runner (text-based version)");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  boosecli <path-to-program.boose> [--png output.png]");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  boosecli ..\\..\\BOOSEexamplePrograms\\1unrestrictedDrawing.boose --png drawing.png");
        Console.WriteLine("  boosecli myProgram.boose");
    }
}
