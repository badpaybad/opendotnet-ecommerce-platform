using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Models
{
    public class ChartKeyValueObject
    {
        public ChartKeyValueObject(string key, double value)
        {
            Key = key;
            Value = value;
        }

        public string Key;
        public double Value;
    }
}