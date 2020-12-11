using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marble.Messaging.Extensions
{
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, string> PrimitiveTypeAliases = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(void), "void" }
        };

        public static string GetPrimitiveAlias(this Type type)
        {
            if (PrimitiveTypeAliases.ContainsKey(type))
            {
                return PrimitiveTypeAliases[type];
            }

            return type.Name;
        }

        public static string GetReadableName(this Type t)
        {
            if (!t.IsGenericType)
            {
                return t.GetPrimitiveAlias();
            }

            StringBuilder sb = new StringBuilder(t.Name.Substring(0, t.GetPrimitiveAlias().IndexOf('`')));
            sb.Append('<');
            bool appendComma = false;
            foreach (Type arg in t.GetGenericArguments())
            {
                if (appendComma) sb.Append(',');
                sb.Append(GetReadableName(arg));
                appendComma = true;
            }
            return sb.Append('>').ToString();
        }

        public static List<string> GetUsedNamespaces(this Type type, ref List<string> list)
        {
            if (type.Namespace != null)
            {
                list.Add(type.Namespace);
            }

            if (!type.IsGenericType)
            {
                return list;
            }
            
            foreach (Type arg in type.GetGenericArguments())
            {
               list.AddRange(GetUsedNamespaces(arg, ref list));
            }

            return list;
        }
        
        public static bool InheritsOrImplements(this Type child, Type parent)
        {
            if (child == parent)
            {
                return true;
            }
            
            parent = ResolveGenericTypeDefinition(parent);

            var currentChild = child.IsGenericType
                ? child.GetGenericTypeDefinition()
                : child;

            while (currentChild != typeof (object))
            {
                if (parent == currentChild || HasAnyInterfaces(parent, currentChild))
                    return true;

                currentChild = currentChild.BaseType != null
                               && currentChild.BaseType.IsGenericType
                    ? currentChild.BaseType.GetGenericTypeDefinition()
                    : currentChild.BaseType;

                if (currentChild == null)
                    return false;
            }
            return false;
        }

        private static bool HasAnyInterfaces(Type parent, Type child)
        {
            return child.GetInterfaces()
                .Any(childInterface =>
                {
                    var currentInterface = childInterface.IsGenericType
                        ? childInterface.GetGenericTypeDefinition()
                        : childInterface;

                    return currentInterface == parent;
                });
        }

        private static Type ResolveGenericTypeDefinition(Type parent)
        {
            var shouldUseGenericType = true;
            if (parent.IsGenericType && parent.GetGenericTypeDefinition() != parent)
                shouldUseGenericType = false;

            if (parent.IsGenericType && shouldUseGenericType)
                parent = parent.GetGenericTypeDefinition();
            return parent;
        }
    }
}