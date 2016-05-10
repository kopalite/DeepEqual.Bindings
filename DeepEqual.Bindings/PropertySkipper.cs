using System;
using System.Linq.Expressions;

namespace DeepEqual.Bindings
{
    internal sealed class PropertySkipper<TType> where TType : class
    {
        private readonly Expression<Func<TType, object>> _property;

        public PropertySkipper(Expression<Func<TType, object>> property)
        {
            _property = property;
        }
    }
}
