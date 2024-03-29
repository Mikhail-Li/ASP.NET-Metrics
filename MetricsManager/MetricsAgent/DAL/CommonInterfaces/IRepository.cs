﻿using System;
using System.Collections.Generic;

namespace MetricsAgent.DAL.CommonInterfaces
{
    public interface IRepository<T> where T : class
    {
        IList<T> GetByPeriod(DateTimeOffset fromTime, DateTimeOffset toTime);

        void Create(T item);
    }
}
