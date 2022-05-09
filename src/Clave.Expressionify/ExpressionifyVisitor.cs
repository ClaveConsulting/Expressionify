using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Clave.Expressionify
{
    public class ExpressionifyVisitor : ExpressionVisitor
    {
        private static readonly IDictionary<MethodInfo, LambdaExpression?> MethodToExpressionMap = new ConcurrentDictionary<MethodInfo, LambdaExpression?>();

        private readonly Dictionary<ParameterExpression, Expression> _replacements = new Dictionary<ParameterExpression, Expression>();

        internal bool HasReplacedCalls { get; private set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (GetMethodExpression(node.Method) is LambdaExpression expression)
            {
                HasReplacedCalls = true;
                RegisterReplacementParameters(node.Arguments, expression);
                var result = Visit(expression.Body);
                UnregisterReplacementParameters(expression);
                return result;
            }

            return base.VisitMethodCall(node);
        }

        private static object? GetMethodExpression(MethodInfo method)
        {
            if (MethodToExpressionMap.TryGetValue(method, out var result))
            {
                return result;
            }

            if (!method.IsStatic)
            {
                return MethodToExpressionMap[method] = null;
            }

            var shouldUseExpression = method.GetCustomAttributes(typeof(ExpressionifyAttribute), false).Any();
            if (!shouldUseExpression)
            {
                return MethodToExpressionMap[method] = null;
            }

            var properties = method.DeclaringType?.GetRuntimeProperties();
            var expression = properties
                ?.Where(x => x.Name.StartsWith($"{method.Name}_Expressionify_"))
                .FirstOrDefault(x => x.MatchesTypeOf(method))
                ?.GetValue(null);

            if (expression is null)
            {
                throw new Exception($"Code generation seems to have failed, could not find expresion for method {GetFullName(method.DeclaringType)}.{method.Name}()");
            }

            return MethodToExpressionMap[method] = expression as LambdaExpression;
        }

        private static string GetFullName(Type type)
        {
            if(type.DeclaringType is Type parent)
            {
                return GetFullName(parent) + "." + type.Name;
            }

            return type.Name;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return _replacements.TryGetValue(node, out var replacement)
                ? Visit(replacement) :
                base.VisitParameter(node);
        }

        private void RegisterReplacementParameters(IReadOnlyCollection<Expression> parameterValues, LambdaExpression expressionToVisit)
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
    }
}
