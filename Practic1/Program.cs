using practic1.models;

namespace practic1;

public class Pracric1
{
    public static void Main(string[] args)
    {
        CLIOperator cLIOperator = new();

        if (args.Length > 0)
        {
            cLIOperator.systemController.setup(args);
            cLIOperator.systemController.StartFile();
        }

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
