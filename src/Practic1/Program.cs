using Practic1.Models.CLIManagement;

namespace Practic1;

public class Program
{
    public static void Main(string[] args)
    {
        CLIOperator cliOperator = new();

        if (args.Length > 0)
        {
            cliOperator.systemController.Setup(args);
            cliOperator.StartFile();
        }

        string? userInput;
        string initialString = CLIOperator.GetPrompt();

        while(true)
        {
            Console.Write(initialString);
            userInput = Console.ReadLine();

            cliOperator.DoCommand(userInput);
            Console.WriteLine("----------------");
        }
    }
} 
