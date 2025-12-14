using System;

namespace GDScriptInterface.Abstractions
{
    /// <summary>
    /// An attribute used to mark a class for generating GDScript interface mappings.
    /// This attribute specifies the target interface type for which the mapping will be generated.
    /// </summary>
    /// <remarks>
    /// The marked class must meet the following requirements; otherwise, diagnostic errors will be reported:
    /// <list type="bullet">
    /// <item><description>The class must be declared as <c>static</c>. If not, the diagnostic <c>GDINTF003</c> ("Invalid GenerateGDScriptInterfaceMapping usage") will be reported.</description></item>
    /// <item><description>The class must be declared as <c>partial</c>. If not, the diagnostic <c>GDINTF003</c> will also be reported.</description></item>
    /// <item><description>The specified interface type must not contain generic type parameters. If it does, the diagnostic <c>GDINTF005</c> ("Generic interface not supported") will be reported.</description></item>
    /// <item><description>The interface must not use types that are incompatible with Godot's Variant type in its members. If it does, the diagnostic <c>GDINTF001</c> ("Invalid Variant-compatible type") will be reported.</description></item>
    /// <item><description>The interface must not contain indexers. If it does, the diagnostic <c>GDINTF004</c> ("Indexer not supported") will be reported.</description></item>
    /// <item><description>The interface must not include methods with generic type parameters. If it does, the diagnostic <c>GDINTF006</c> ("Generic method not supported") will be reported.</description></item>
    /// <item><description>The provided type in the constructor must be an interface. If not, the diagnostic <c>GDINTF002</c> ("Type must be an interface") will be reported.</description></item>
    /// </list>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class GenerateGDScriptInterfaceMappingAttribute : Attribute
    {
        /// <summary>
        /// Gets the interface <see cref="Type"/> for which GDScript mapping code will be generated.
        /// This type must represent a non-generic interface.
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateGDScriptInterfaceMappingAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">
        /// The interface <see cref="Type"/> to generate a GDScript-compatible mapping for.
        /// Provide the interface type (e.g. <c>typeof(IMyInterface)</c>).
        /// </param>
        public GenerateGDScriptInterfaceMappingAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }

}