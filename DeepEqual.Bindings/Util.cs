using System;
using System.Linq.Expressions;

namespace DeepEqual.Bindings
{
    internal class Util
    {
        public static string NameOf<TType>(Expression<Func<TType, object>> expression)
        {
            return NameOf((Expression)expression);
        }

        private static string NameOf(Expression expression)
        {
            var lambda = expression as LambdaExpression;
            if (lambda != null)
            {
                return NameOf(lambda.Body);
            }

            var unary = expression as UnaryExpression;
            if (unary != null)
            {
                return NameOf(unary.Operand);
            }

            var member = expression as MemberExpression;
            if (member != null)
            {
                return member.Member.Name;
            }

            throw new ArgumentException("Must be unary expression or member expression!");
        }
    }
}
