using practic1.models;

namespace practic1;


public class Pracric1
{
    public static void Main(string[] args)
    {
        CLIOperator cliOperator = new();
        string? userInput, result, initialString;

        initialString = cliOperator.initialString;

        while(true)
        {
            Console.Write(initialString);
            userInput = Console.ReadLine();

            result = cliOperator.DoCommand(userInput);

            Console.WriteLine(result);
            Console.WriteLine("----------------");
        }
    }
} 