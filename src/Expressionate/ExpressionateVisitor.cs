using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Expressionate
{
    public class ExpressionateVisitor : ExpressionVisitor
    {
        private readonly IQueryProvider _provider;
        private readonly Dictionary<ParameterExpression, Expression> _replacements = new Dictionary<ParameterExpression, Expression>();

        internal ExpressionateVisitor(IQueryProvider provider)
        {
            _provider = provider;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!node.Method.IsStatic)
            {
                return base.VisitMethodCall(node);
            }

            var shouldUseExpression = node.Method.GetCustomAttributes(typeof(ExpressionateAttribute), false).Any();
            if (!shouldUseExpression)
            {
                return base.VisitMethodCall(node);
            }

            var className = node.Method.DeclaringType.AssemblyQualifiedName;
            var expressionClassName = GetExpressionateClassName(className);
            var expressionClass = Type.GetType(expressionClassName);
            var properties = expressionClass.GetRuntimeProperties();
            var result = properties.First(x => x.Name == node.Method.Name).GetValue(null);
            if (result is LambdaExpression expression)
            {
                RegisterReplacementParameters(node.Arguments.ToArray(), expression);
                return Visit(expression.Body);
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_replacements.TryGetValue(node, out Expression replacement))
            {
                return Visit(replacement);
            }

            return base.VisitParameter(node);
        }

        private void RegisterReplacementParameters(Expression[] parameterValues, LambdaExpression expressionToVisit)
        {
            if (parameterValues.Length != expressionToVisit.Parameters.Count)
                throw new ArgumentException(string.Format("The parameter values count ({0}) does not match the expression parameter count ({1})", parameterValues.Length, expressionToVisit.Parameters.Count));

            foreach (var (idx, p) in expressionToVisit.Parameters.Select((p, idx) => (idx, p)))
            {
                if (_replacements.ContainsKey(p))
                {
                    throw new Exception("Parameter already registered, this shouldn't happen.");
                }
                _replacements.Add(p, parameterValues[idx]);
            }
        }

        public static string GetExpressionateClassName(string name)
        {
            var parts = name.Split(',');

            return $"{parts.First()}_Expressionate,{string.Join(",", parts.Skip(1))}";
        }
    }
}
