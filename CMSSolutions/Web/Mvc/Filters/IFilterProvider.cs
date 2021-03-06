﻿using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc.Filters
{
    public interface IFilterProvider : IDependency
    {
        void AddFilters(FilterInfo filterInfo);
    }

    public abstract class FilterProvider : IFilterProvider
    {
        void IFilterProvider.AddFilters(FilterInfo filterInfo)
        {
            AddFilters(filterInfo);
        }

        protected virtual void AddFilters(FilterInfo filterInfo)
        {
            if (this is IAuthorizationFilter)
                filterInfo.AuthorizationFilters.Add(this as IAuthorizationFilter);
            if (this is IActionFilter)
                filterInfo.ActionFilters.Add(this as IActionFilter);
            if (this is IResultFilter)
                filterInfo.ResultFilters.Add(this as IResultFilter);
            if (this is IExceptionFilter)
                filterInfo.ExceptionFilters.Add(this as IExceptionFilter);
        }
    }
}