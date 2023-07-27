using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.BusinessLogic.Models
{
    public class RatesRuleRequest
    {
        public string RuleId { get; set; }

        public string Date { get; set; }

        public string Rate { get; set; }
    }
}
