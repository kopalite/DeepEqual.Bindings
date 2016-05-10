using System;
using System.Linq.Expressions;

namespace DeepEqual.Bindings
{
    internal sealed class PropertySkipper<TType> where TType : class
    {
        public Expression<Func<TType, object>> Property { get; private set; }

        public PropertySkipper(Expression<Func<TType, object>> property)
        {
            Property = property;
        }
    }
}
