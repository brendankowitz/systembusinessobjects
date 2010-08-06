﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.BusinessObjects.Infrastructure
{
    public abstract class Query<T>
    {
        public abstract TReturnType Expression<TReturnType>(IQueryable<T> query) where TReturnType : IQueryable;
    }
}
