// No idea why but required is missing somehow

// Internal.cs
// ReSharper disable UnusedType.Global

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit;
    internal class RequiredMemberAttribute : Attribute;
    internal class CompilerFeatureRequiredAttribute(string name) : Attribute
    {
        internal string Name { get; } = name;
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Constructor)]
    internal class SetsRequiredMembersAttribute : Attribute;
}