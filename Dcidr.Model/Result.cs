using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model
{
    public class Result
    {
        public Result(string option, decimal score)
        {
            Option = option;
            Score = score;
        }

        public string Option { get; }
        public decimal Score { get; }
    }
}
