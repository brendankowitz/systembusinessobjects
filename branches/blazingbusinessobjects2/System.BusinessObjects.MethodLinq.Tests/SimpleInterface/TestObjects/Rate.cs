using System;
using System.Collections.Generic;

namespace System.BusinessObjects.MethodLinq.Tests.SimpleInterface.TestObjects
{
    public class Rate
    {
        public Building Building { get; set; }
        public string Package { get; set; }
        public string Description { get; set; }
        public KeyValuePair<string,string> RoomCategory { get; set; }
        public Dictionary<DateTime,decimal> DayPrices { get; set; }
    }
}
