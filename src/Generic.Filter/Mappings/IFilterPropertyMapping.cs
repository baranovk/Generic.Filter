﻿using System.Linq.Expressions;

namespace Generic.Filter.Mappings
{
    public interface IFilterPropertyMapping
    {
        Expression MapFor(ParameterExpression item);
    }
}
