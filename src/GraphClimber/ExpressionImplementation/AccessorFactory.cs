using System;
using System.Linq.Expressions;
using GraphClimber.ExpressionCompiler;
using GraphClimber.ExpressionCompiler.Extensions;

namespace GraphClimber
{
    internal class AccessorFactory : IAccessorFactory
    {
        private readonly IExpressionCompiler _compiler;

        public AccessorFactory(IExpressionCompiler compiler)
        {
            _compiler = compiler;
        }

        public Action<object, T> GetSetter<T>(IStateMember member)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object));
            ParameterExpression value = Expression.Parameter(typeof(T));

            Expression<Action<object, T>> lambda =
                Expression.Lambda<Action<object, T>>
                    (member.GetSetExpression(GetValue(member, instance), value),
                        "Setter_" + member.Name,
                        new[] {instance, value});

            return _compiler.Compile(lambda);
        }

        private static Expression GetValue(IStateMember member, ParameterExpression instance)
        {
            if (member.OwnerType.IsValueType)
            {
                return Expression.Field(instance.Convert(typeof(FastBox<>).MakeGenericType(member.OwnerType)), "Value");    
            }

            return instance;
        }

        public Func<object, T> GetGetter<T>(IStateMember member)
        {
            ParameterExpression instance = Expression.Parameter(typeof(object));

            Expression<Func<object, T>> lambda =
                Expression.Lambda<Func<object, T>>
                    (
                        // We need to convert to the type, since sometimes we fake the member type.
                        member.GetGetExpression(GetValue(member, instance)).Convert(typeof(T)),
                        "Getter_" + member.Name,
                        new[] {instance});

            return _compiler.Compile(lambda);
        }
    }
}