using System.Reflection;

namespace Marble.Messaging.Transformers
{
    public class ProcedureName
    {
        public static string FromMethodInfo(MethodInfo methodInfo)
        {
            return FromMethodName(methodInfo.Name);
        }

        public static string FromMethodName(string methodName)
        {
            return methodName;
        }
    }
}