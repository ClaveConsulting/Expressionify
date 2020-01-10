using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Clave.Expressionify
{
    public class ExpressionifyVisitor : ExpressionVisitor
    {
        private static readonly IDictionary<MethodInfo, object> MethodToExpressionMap = new Dictionary<MethodInfo, object>();

        private readonly Dictionary<ParameterExpression, Expression> _replacements = new Dictionary<ParameterExpression, Expression>();

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (GetMethodExpression(node.Method) is LambdaExpression expression)
            {
                RegisterReplacementParameters(node.Arguments, expression);
                var result = Visit(expression.Body);
                UnregisterReplacementParameters(expression);
                return result;
            }

            return base.VisitMethodCall(node);
        }

        private static object GetMethodExpression(MethodInfo method)
        {
            if (MethodToExpressionMap.TryGetValue(method, out var result))
            {
                return result;
            }

            if (!method.IsStatic)
            {
                return MethodToExpressionMap[method] = false;
            }

            var shouldUseExpression = method.GetCustomAttributes(typeof(ExpressionifyAttribute), false).Any();
            if (!shouldUseExpression)
            {
                return MethodToExpressionMap[method] = false;
            }

            var className = method.DeclaringType.Name;
            var expressionClassName = GetExpressionifyClassName(className);
            var expressionClass = method.DeclaringType.Assembly.ExportedTypes
                .Where(t => t.Namespace == method.DeclaringType.Namespace)
                .FirstOrDefault(t => t.Name == expressionClassName);
            if(expressionClass == null)
            {
                throw new Exception($"Could not find type {expressionClassName} containing the expressionified method {method.Name}");
            }

            var properties = expressionClass.GetRuntimeProperties();
            return MethodToExpressionMap[method] = properties
                .First(x => x.Name == method.Name)
                .GetValue(null);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_replacements.TryGetValue(node, out Expression replacement))
            {
                return Visit(replacement);
            }

            return base.VisitParameter(node);
        }

        private void RegisterReplacementParameters(IReadOnlyList<Expression> parameterValues, LambdaExpression expressionToVisit)
        {
            if (parameterValues.Count != expressionToVisit.Parameters.Count)
                throw new ArgumentException($"The parameter values count ({parameterValues.Count}) does not match the expression parameter count ({expressionToVisit.Parameters.Count})");

            foreach (var (p, v) in expressionToVisit.Parameters.Zip(parameterValues, ValueTuple.Create))
            {
                _replacements.Add(p, v);
            }
        }

        private void UnregisterReplacementParameters(LambdaExpression expressionToVisit)
        {
            foreach (var p in expressionToVisit.Parameters)
            {
                _replacements.Remove(p);
            }
        }

        public static string GetExpressionifyClassName(string name)
        {
            return $"{name}_Expressionify";
        }
    }
}
