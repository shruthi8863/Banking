using BankingApplication.BusinessLogic; 
using BankingApplication.BusinessLogic.Interfaces;
using BankingApplication.BusinessLogic.Services;
using Microsoft.Extensions.Configuration;
using NLog;
using System.Text.RegularExpressions;

public class Program
{
    private static ILogger logger = LogManager.GetCurrentClassLogger();
    public static async Task Main()
    {

        #region task to calculate interest on monthly basis

        IInterestCalculationLogic interestCalculationLogic = new InterestCalculationLogic();

        _=Task.Run(async () => await interestCalculationLogic.UpdateInterestRates(DateTime.Now));

        #endregion

       

        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true);
        IConfiguration config = builder.Build();

        PromptForUserInput();

        var input = Console.ReadLine();

        if (!string.IsNullOrEmpty(input))
        {
            var result = ValidateInput(input);

            if (!result)
            {
                Console.WriteLine("Please enter a valid input");
            }
            else
            {
                IValidateTransactions validateTransactions = new ValidateTransactions();
                IProcessTransactions processTransactions = new ProcessTransactions();
                BankingTransactions bankingTransactions = new(validateTransactions, processTransactions, logger);
                await bankingTransactions.ProcessInputs(input);
            }
        }
        else
        {
            Console.WriteLine("Please enter a valid input");
        }

    }

  
    public static bool ValidateInput(string v)
    {
        Regex regEx = new("[(I|D|P|Q){1}]" ,RegexOptions.IgnoreCase);
      if(v.Length >1 || !regEx.IsMatch(v))
            return false;
        else
            return true;
    }
    public static void PromptForUserInput()
    {
        Console.WriteLine("Welcome to Awesome GIC Bank! What would you like to do?");
        Console.WriteLine("[I]Input transactions");
        Console.WriteLine("[D]efine interest rules");
        Console.WriteLine("[P]rint statement");
        Console.WriteLine("[Q]uit");
       
    }
}