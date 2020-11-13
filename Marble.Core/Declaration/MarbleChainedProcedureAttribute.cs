using System;

namespace Marble.Core.Declaration
{
    /// <summary>
    /// Use this attribute as an ADDON to the MarbleProcedureAttribute -
    /// if this function call directly returns
    /// another external marble procedure result.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MarbleChainedProcedureAttribute : Attribute
    {
    }
}
