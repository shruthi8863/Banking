using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.BusinessLogic.Models
{
    public class RatesRuleDetails
    {
        public string RuleId { get; set; }

        public string Date { get; set; }

        public decimal Rate { get; set; }
    }
}
