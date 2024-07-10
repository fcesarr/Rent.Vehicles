using System.Linq.Expressions;

namespace Rent.Vehicles.Consumers.IntegrationTests.Extensions;

public static class ExpressionExtension
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        // get the visitor
        var visitor = new ParameterUpdateVisitor(right.Parameters.First(), left.Parameters.First());
        // replace the parameter in the expression just created
        if (visitor.Visit(right) is Expression<Func<T, bool>> exp)
        {
            right = exp;
        }

        // now you can and together the two expressions
        var binExp = Expression.And(left.Body, right.Body);
        // and return a new lambda, that will do what you want. NOTE that the binExp has reference only to te newExp.Parameters[0] (there is only 1) parameter, and no other
        return Expression.Lambda<Func<T, bool>>(binExp, right.Parameters);
    }

    private class ParameterUpdateVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _newParameter;
        private readonly ParameterExpression _oldParameter;

        public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (ReferenceEquals(node, _oldParameter))
            {
                return _newParameter;
            }

            return base.VisitParameter(node);
        }
    }
}
