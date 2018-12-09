﻿using System;

namespace CMSSolutions.Caching
{
    public interface ICacheManager
    {
        TResult Get<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire);

        ICache<TKey, TResult> GetCache<TKey, TResult>();

        void Reset();
    }
}