using System;
using System.Linq.Expressions;

namespace Iauq.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static string GetTypeName<TType>(this Expression<Func<TType, object>> expression)
            where TType : class
        {
            MemberExpression memberExpression = GetMemberInfo(expression);

            string typeName = memberExpression.Type.Name;

            return typeName;
        }

        public static string GetPropertyName<TType>(this Expression<Func<TType, object>> expression)
            where TType : class
        {
            MemberExpression memberExpression = GetMemberInfo(expression);

            string propertyName = memberExpression.Member.Name;

            return propertyName;
        }

        private static MemberExpression GetMemberInfo(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;

            if (lambdaExpression == null)
                throw new ArgumentNullException("expression");

            MemberExpression memberExpression = null;

            switch (lambdaExpression.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpression =
                        ((UnaryExpression) lambdaExpression.Body).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpression = lambdaExpression.Body as MemberExpression;
                    break;
            }

            if (memberExpression == null)
                throw new InvalidOperationException("expression");

            return memberExpression;
        }
    }
}