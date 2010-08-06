using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace System.BusinessObjects.Infrastructure
{
    public abstract class CommandQuery
    {
        private List<IDataParameter> _parameters = new List<IDataParameter>();
        public List<IDataParameter> Parameters
        {
            get
            {
                return _parameters;
            }
        }
        public abstract string Command { get; }
    }
}
