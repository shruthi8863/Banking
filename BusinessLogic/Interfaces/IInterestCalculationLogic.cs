using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.BusinessLogic.Interfaces
{
    internal interface IInterestCalculationLogic
    {
        Task UpdateInterestRates(DateTime currentDate);
    }
}
