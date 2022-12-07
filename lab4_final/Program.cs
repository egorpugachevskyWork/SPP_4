using MainPart.ForGenerator;
using MainPart.ForScript;

class Program
{
    private static List<string> _namesOfFile = new List<string>() { "A.cs", "B.cs", "Akappuza.cs" }; //{ "Aleksei.cs", "Akappuza.cs", "Gigachad.cs", "Oop.cs", "SPP.cs", "A.cs", "B.cs" };
    private static string _writePath = "D:\\Studying\\third_course\\СПП\\generated_test";

    static void Main(string[] args)
    {
        var scripter = new TestScripter(_namesOfFile, _writePath, 1, 4, 4);
        scripter.Generate().GetAwaiter().GetResult();
        Console.WriteLine("Main is finished");
    }


}