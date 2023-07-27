using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApplication.Helpers
{
    public class ErrorResponses
    {
        public int ErrorCount { get; set; }

        public List<string> Errors { get; set; } = new();

        [DefaultValue("")]
        public string ExceptionMessage { get; set; }

    }
}
