using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphClimber
{
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> GetInterfacesAndBase(this Type type)
        {
            Type baseType = type.BaseType;

            if (baseType != null)
            {
                yield return baseType;
            }

            foreach (Type interfaceType in type.GetInterfaces())
            {
                yield return interfaceType;
            }
        }

        public static IEnumerable<Type> AsEnumerable(this Type type)
        {
            yield return type;
        }

        public static IEnumerable<Type> WithInterfaces(this Type type)
        {
            yield return type;

            foreach (Type interfaceType in type.GetInterfaces())
            {
                yield return interfaceType;
            }
        }

        public static bool IsNullable(this Type type)
        {
            return (type.IsGenericType &&
                    (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        /// <summary>
        /// Returns the closed generic type of the given generic open type,
        /// that the given type is assignable to.
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <param name="openGenericType">The open generic type.</param>
        /// <returns>The closed generic type of the given generic open type,
        /// that the given type is assignable to.</returns>
        public static IEnumerable<Type> GetClosedGenericTypeImplementation(this Type type, Type openGenericType)
        {
            if (type == null)
            {
                return Enumerable.Empty<Type>();
            }

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == openGenericType)
            {
                return Singletone(type);
            }

            if (!openGenericType.IsInterface)
            {
                return type.BaseType.GetClosedGenericTypeImplementation(openGenericType);
            }
            else
            {
                return type.GetInterfaces()
                    .SelectMany(x => GetClosedGenericTypeImplementation(x, openGenericType));
            }
        }

        private static IEnumerable<T> Singletone<T>(T element)
        {
            yield return element;
        }

        public static IDictionary<Type, Type> GetTypeGenericArgumentsMap(Type declaringType)
        {
            if (declaringType.IsGenericType)
            {
                Type genericTypeDefinition =
                    declaringType.GetGenericTypeDefinition();

                IDictionary<Type, Type> typeGenericArguments =
                    genericTypeDefinition.GetGenericArguments()
                        .Zip(declaringType.GetGenericArguments(),
                            Tuple.Create)
                        .ToDictionary(x => x.Item1,
                            x => x.Item2);

                return typeGenericArguments;
            }

            return new Dictionary<Type, Type>();
        }
    }
}