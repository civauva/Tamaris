using System;
using System.Linq.Expressions;


namespace Tamaris.DAL.Infrastructure
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
			if (a != null && b == null)
				return a;

			if (a == null && b != null)
				return b;

            ParameterExpression p = a.Parameters[0];

            SubstituteExpressionVisitor visitor = new SubstituteExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {
			if (a != null && b == null)
				return a;

			if (a == null && b != null)
				return b;

            ParameterExpression p = a.Parameters[0];

            SubstituteExpressionVisitor visitor = new SubstituteExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }
}