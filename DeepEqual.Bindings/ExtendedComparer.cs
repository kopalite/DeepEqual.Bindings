using DeepEqual.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DeepEqual.Bindings
{
    public class ExtendedComparer<TSource, TDestination> where TSource : class where TDestination : class
    {
        private List<PropertySkipper<TSource>> _sourceSkippers;
        private List<PropertySkipper<TDestination>> _destinationSkippers;
        private List<PropertyBinder<TSource, TDestination>> _propertyBinders;

        private ExtendedComparer()
        {
            _sourceSkippers = new List<PropertySkipper<TSource>>();
            _destinationSkippers = new List<PropertySkipper<TDestination>>();
            _propertyBinders = new List<PropertyBinder<TSource, TDestination>>();
        }

        public static ExtendedComparer<TSource, TDestination> New()
        {
            return new ExtendedComparer<TSource, TDestination>();
        }

        public ExtendedComparer<TSource, TDestination> Skip(Expression<Func<TSource, object>> source)
        {
            _sourceSkippers.Add(new PropertySkipper<TSource>(source));
            return this;
        }

        public ExtendedComparer<TSource, TDestination> Skip(Expression<Func<TDestination, object>> destination)
        {
            _destinationSkippers.Add(new PropertySkipper<TDestination>(destination));
            return this;
        }

        public ExtendedComparer<TSource, TDestination> Bind(Expression<Func<TSource, object>> source,
                                                            Expression<Func<TDestination, object>> destination)
        {
            Action<TSource, TDestination> action = (x1, x2) => source.Compile()(x1).ShouldDeepEqual(destination.Compile()(x2), null);
            _propertyBinders.Add(new PropertyBinder<TSource, TDestination>(source, destination, (x1, x2) => action(x1, x2)));
            return this;
        }

        public ExtendedComparer<TSource, TDestination> Bind(Expression<Func<TSource, object>> source,
                                                            Expression<Func<TDestination, object>> destination,
                                                            Expression<Func<TSource, TDestination, bool>> comparison)
        {
            _propertyBinders.Add(new PropertyBinder<TSource, TDestination>(source, destination, comparison));
            return this;
        }

        public bool AreEqual(TSource obj1, TDestination obj2)
        {
            string difference;
            return AreEqual(obj1, obj2, out difference);
        }

        public bool AreEqual(TSource obj1, TDestination obj2, out string difference)
        {
            //Trying default deep equality.

            difference = null;

            try
            {
                var comparer = obj1.WithDeepEqual(obj2);

                foreach (var sourceSkipper in _sourceSkippers)
                {
                    comparer = comparer.IgnoreSourceProperty(sourceSkipper.Property);
                }

                foreach (var destinationScipper in _destinationSkippers)
                {
                    comparer = comparer.IgnoreDestinationProperty(destinationScipper.Property);
                }

                foreach (var binding in _propertyBinders)
                {
                    var sourceName = Util.NameOf(binding.Source);
                    var destinationName = Util.NameOf(binding.Destination);
                    comparer = comparer.IgnoreProperty(pr => pr.DeclaringType == typeof(TSource) && pr.Name == sourceName)
                                       .IgnoreProperty(pr => pr.DeclaringType == typeof(TDestination) && pr.Name == destinationName);
                }

                comparer.Assert();
            }
            catch (Exception ex)
            {
                difference = ex.Message;
            }

            //If they are not deep equal by default, we will return false, no need to check custom binders. 

            if (!string.IsNullOrWhiteSpace(difference))
            {
                return false;
            }

            difference = null;

            _propertyBinders.ForEach(b => b.Compare(obj1, obj2));
            var differences = _propertyBinders.Where(b => !string.IsNullOrWhiteSpace(b.ComparisonResult)).Select(b => b.ComparisonResult);
            if (differences.Any())
            {
                difference = string.Join(Environment.NewLine, differences);
            }
            

            return string.IsNullOrWhiteSpace(difference);
        }

        public ExtendedComparer<TDestination, TSource> Reverseded()
        {
            var reversedComparer = new ExtendedComparer<TDestination, TSource>();

            foreach (var propertyBinder in _propertyBinders)
            {
                var reversedBinder = propertyBinder.Reversed();

                reversedComparer._propertyBinders.Add(reversedBinder);
            }

            foreach (var sourceSkipper in _sourceSkippers)
            {
                reversedComparer._destinationSkippers.Add(sourceSkipper);
            }

            foreach (var destinationSkipper in _destinationSkippers)
            {
                reversedComparer._sourceSkippers.Add(destinationSkipper);
            }

            return reversedComparer;
        }
    }
}
