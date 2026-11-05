using practic1.models;

namespace practic1;

public class Pracric1
{

    public static void Main(string[] args)
    {
        CLIOperator cLIOperator = new();

        string? userInput;
        string initialString = CLIOperator.GetPrompt();

        while(true)
        {
            Console.Write(initialString);
            userInput = Console.ReadLine();

            cLIOperator.DoCommand(userInput);
            Console.WriteLine("----------------");
        }
    }
} 
