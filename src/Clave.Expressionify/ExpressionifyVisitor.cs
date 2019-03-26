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

        private LinkedListNode<Expression> _replacements;

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (GetOrCreateExpression(node.Method) is LambdaExpression expression)
            {
                _replacements = new LinkedList<Expression>(node.Arguments).First;
                return Visit(expression.Body);
            }

            return base.VisitMethodCall(node);
        }

        private static object GetOrCreateExpression(MethodInfo method)
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

            return MethodToExpressionMap[method] = GetMethodExpression(method);
        }

        private static object GetMethodExpression(MethodInfo method)
        {
            var className = method.DeclaringType.AssemblyQualifiedName;
            var expressionClassName = GetExpressionifyClassName(className);
            var expressionClass = Type.GetType(expressionClassName);
            var properties = expressionClass.GetRuntimeProperties();
            return properties.First(x => x.Name == method.Name).GetValue(null);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (_replacements != null)
            {
                var replacement = _replacements.Value;
                _replacements = _replacements.Next;
                return Visit(replacement);
            }

            return base.VisitParameter(node);
        }

        public static string GetExpressionifyClassName(string name)
        {
            var parts = name.Split(',');

            return $"{parts.First()}_Expressionify,{string.Join(",", parts.Skip(1))}";
        }
    }
}
