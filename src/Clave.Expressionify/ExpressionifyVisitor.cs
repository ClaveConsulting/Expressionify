using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Clave.Expressionify
{
    public class ExpressionifyVisitor : ExpressionVisitor
    {
        private static readonly IDictionary<MethodInfo, object> MethodToExpressionMap = new Dictionary<MethodInfo, object>();

        private readonly IQueryProvider _provider;

        private readonly Dictionary<ParameterExpression, Expression> _replacements = new Dictionary<ParameterExpression, Expression>();

        internal ExpressionifyVisitor(IQueryProvider provider)
        {
            _provider = provider;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!node.Method.IsStatic)
            {
                return base.VisitMethodCall(node);
            }

            var shouldUseExpression = node.Method.GetCustomAttributes(typeof(ExpressionifyAttribute), false).Any();
            if (!shouldUseExpression)
            {
                return base.VisitMethodCall(node);
            }

            if (GetMethodExpression(node.Method) is LambdaExpression expression)
            {
                RegisterReplacementParameters(node.Arguments.ToArray(), expression);
                return Visit(expression.Body);
            }

            throw new Exception("Property marked with [Expressionify] must be of type Expression<Func<>>");
        }

        private static object GetMethodExpression(MethodInfo method)
        {
            if (MethodToExpressionMap.TryGetValue(method, out var result))
            {
                return result;
            }

            var className = method.DeclaringType.AssemblyQualifiedName;
            var expressionClassName = GetExpressionifyClassName(className);
            var expressionClass = Type.GetType(expressionClassName);
            var properties = expressionClass.GetRuntimeProperties();
            return MethodToExpressionMap[method] = properties.First(x => x.Name == method.Name).GetValue(null);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_replacements.TryGetValue(node, out Expression replacement))
            {
                _replacements.Remove(node);
                return Visit(replacement);
            }

            return base.VisitParameter(node);
        }

        private void RegisterReplacementParameters(Expression[] parameterValues, LambdaExpression expressionToVisit)
        {
            if (parameterValues.Length != expressionToVisit.Parameters.Count)
                throw new ArgumentException($"The parameter values count ({parameterValues.Length}) does not match the expression parameter count ({expressionToVisit.Parameters.Count})");

            foreach (var (p, v) in expressionToVisit.Parameters.Zip(parameterValues, ValueTuple.Create))
            {
                if (_replacements.ContainsKey(p))
                {
                    throw new Exception("Parameter already registered, this shouldn't happen.");
                }
                _replacements.Add(p, v);
            }
        }

        public static string GetExpressionifyClassName(string name)
        {
            var parts = name.Split(',');

            return $"{parts.First()}_Expressionify,{string.Join(",", parts.Skip(1))}";
        }
    }
}
