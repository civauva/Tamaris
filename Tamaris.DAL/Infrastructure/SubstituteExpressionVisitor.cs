using System.Collections.Generic;
using System.Linq.Expressions;


namespace Tamaris.DAL.Infrastructure
{
	internal class SubstituteExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
	{
		public Dictionary<Expression, Expression> subst = new Dictionary<Expression, Expression>();

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (subst.TryGetValue(node, out Expression newValue))
			{
				return newValue;
			}

			return node;
		}
	}
}