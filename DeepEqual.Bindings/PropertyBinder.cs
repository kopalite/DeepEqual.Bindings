using System;
using System.Linq.Expressions;

namespace DeepEqual.Bindings
{
    internal sealed class PropertyBinder<TSource, TDestination> where TSource : class where TDestination : class
    {
        public Expression<Func<TSource, object>> Source { get; }
        private string _sourceName;

        public Expression<Func<TDestination, object>> Destination { get; }
        private string _destinationName;

        private Func<TSource, TDestination, bool> _comparison;
        private string _comparisonDescription;

        public string ComparisonResult { get; private set; }

        public PropertyBinder(Expression<Func<TSource, object>> source,
                              Expression<Func<TDestination, object>> destination,
                              Expression<Action<TSource, TDestination>> comparison)
        {
            if (source == null || destination == null || comparison == null)
            {
                throw new ArgumentException("Given expressions cannot be null!");
            }

            Source = source;
            _sourceName = Util.NameOf(Source);

            Destination = destination;
            _destinationName = Util.NameOf(Destination);

            Func<TSource, TDestination, bool> function = (x1, x2) => 
            {
                try
                {
                    comparison.Compile()(x1, x2);
                    return true;
                }
                catch
                {
                    throw;
                }
            };

            _comparison = (x1, x2) => function(x1, x2);
            _comparisonDescription = _comparison.ToString();
        }

        public PropertyBinder(Expression<Func<TSource, object>> source,
                              Expression<Func<TDestination, object>> destination,
                              Expression<Func<TSource, TDestination, bool>> comparison)
        {
            if (source == null || destination == null || comparison == null)
            {
                throw new ArgumentException("Given expressions cannot be null!");
            }

            Source = source;
            _sourceName = Util.NameOf(Source);

            Destination = destination;
            _destinationName = Util.NameOf(Destination);

            _comparison = comparison.Compile();
            _comparisonDescription = comparison.ToString();
        }

        public void Compare(TSource obj1, TDestination obj2)
        {
            try
            {
                ComparisonResult = null;
                var areEqual = _comparison(obj1, obj2);
                if (!areEqual)
                {
                    ComparisonResult = string.Format("Bound properties '{0}' and '{1}' are not equal by comparison '{3}'", _sourceName, _destinationName, _comparisonDescription);
                }
            }
            catch (Exception ex)
            {
                ComparisonResult = ex.Message; 
            }
        }
    }
}
