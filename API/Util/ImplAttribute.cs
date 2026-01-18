#pragma warning disable IDE0130
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Method)]
internal class ImplAttribute<T> : Attribute;

[AttributeUsage(AttributeTargets.Method)]
internal class ImplAttribute<T1, T2> : Attribute;

[AttributeUsage(AttributeTargets.Method)]
internal class ImplAttribute<T1, T2, T3> : Attribute;

[AttributeUsage(AttributeTargets.Method)]
internal class ImplAttribute<T1, T2, T3, T4> : Attribute;