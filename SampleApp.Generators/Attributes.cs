namespace SampleApp.Generators;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class NotifyPropertyChangedAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class PropertyChangedAttribute : Attribute;
