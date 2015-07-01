<#
HiddenTypes.Add(typeof(MetaType));
HiddenTypes.Add(typeof(ArgSource));
HiddenTypes.Add(typeof(HandleTypePosition));
HiddenTypes.Add(typeof(ParameterStyle));
HiddenTypes.Add(typeof(OptionalStyle));

DiscoverTemplateTypes();
#><#+
protected const string GreenFieldName = "greenNode";

protected static HashSet<Type> HiddenTypes = new HashSet<Type>();

protected static HashSet<MetaType> TemplateTypes { get; private set; }

protected static string PascalCase(string name) {
	return name.Substring(0,1).ToUpperInvariant() + name.Substring(1);
}

protected static string CamelCase(string name) {
	return name.Substring(0,1).ToLowerInvariant() + name.Substring(1);
}

protected static void DiscoverTemplateTypes() {
	var rootType = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
	TemplateTypes = new HashSet<MetaType>();
	var pendingTypes = new Queue<Type>();

	Func<Type, bool> includeTypeTest = t => true
						&& t.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>() == null
						&& t.GetCustomAttribute<ImmutableObjectGraph.IgnoreAttribute>() == null
						&& !HiddenTypes.Any(ht => ht.IsEquivalentTo(t));

	foreach (var nestedType in rootType.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)) {
		if (includeTypeTest(nestedType)) {
			pendingTypes.Enqueue(nestedType);
		}
	}

	while (pendingTypes.Count > 0)
	{
		var type = pendingTypes.Dequeue();
		if (!includeTypeTest(type)) {
			continue;
		}

		var metaType = new MetaType(type);
		if (TemplateTypes.Add(metaType)) {
			foreach(var field in metaType.LocalFields) {
				var memberType = field.ElementType;

				if (memberType.DeclaringType == type.DeclaringType) { // if this is a type that also appears in the T4 template
					pendingTypes.Enqueue(memberType);
				}
			}

			if (type.DeclaringType != null) {
				foreach (var siblingNestedType in type.DeclaringType.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public)) {
					if (includeTypeTest(siblingNestedType)) {
						pendingTypes.Enqueue(siblingNestedType);
					}
				}
			}
		}
	}
}

private static bool IsCollectionType(Type type) {
	if (type.IsArray) {
		return true;
	}

	if (type.IsGenericType && type.GenericTypeArguments.Length == 1) {
		Type readOnlyCollection = typeof(IReadOnlyCollection<>);
		var genericReadOnlyCollection = readOnlyCollection.MakeGenericType(type.GenericTypeArguments[0]);
		if (genericReadOnlyCollection.IsAssignableFrom(type)) {
			return true;
		}
	}

	return false;
}

private static bool TryGetCollectionElementType(Type collectionType, out Type elementType) {
	if (collectionType.IsArray) {
		elementType = collectionType.GetElementType();
		return true;
	}

	if (collectionType.IsGenericType && collectionType.GenericTypeArguments.Length == 1) {
		Type readOnlyCollection = typeof(IReadOnlyCollection<>);
		var genericReadOnlyCollection = readOnlyCollection.MakeGenericType(collectionType.GenericTypeArguments[0]);
		if (genericReadOnlyCollection.IsAssignableFrom(collectionType)) {
			elementType = collectionType.GenericTypeArguments[0];
			return true;
		}
	}

	elementType = null;
	return false;
}

private static Type GetTypeOrCollectionMemberType(Type collectionOrOtherType) {
	Type memberType;
	if (TryGetCollectionElementType(collectionOrOtherType, out memberType)) {
		return memberType;
	}

	return collectionOrOtherType;
}

private enum ArgSource {
	Property,
	Argument,
	OptionalArgumentOrProperty,
	OptionalArgumentOrTemplate,
	Missing,
}

private enum OptionalStyle {
	None,
	WhenNotRequired,
	Always,
}

private void WriteArguments(IEnumerable<MetaType.MetaField> fields, ArgSource source = ArgSource.Property, int indent = 3, bool concatenated = false, OptionalStyle asOptional = OptionalStyle.None) {
	PushIndent(new string('\t', indent));
	bool firstInSequence = !concatenated;

	foreach(var field in fields) {
		ArgSource fieldSource = (source == ArgSource.OptionalArgumentOrTemplate && field.IsRequired) ? ArgSource.Argument : source;
		if (!firstInSequence) { Write(","); }
		WriteLine("");
		Write("{0}: ", field.NameCamelCase); // named arguments are important because the order can vary (due to required fields, etc.)

		bool optionalWrap = asOptional != OptionalStyle.None && (asOptional == OptionalStyle.Always || !field.IsRequired);
		if (optionalWrap) {
			Write("Optional.For(");
		}

		switch (fieldSource) {
			case ArgSource.Property:
				Write("this.{0}", field.NamePascalCase);
				break;
			case ArgSource.Argument:
				Write(field.ArgumentReference);
				break;
			case ArgSource.OptionalArgumentOrProperty:
				Write("{0}.GetValueOrDefault(this.{1})", field.NameCamelCase, field.NamePascalCase);
				break;
			case ArgSource.OptionalArgumentOrTemplate:
				Write("{0}.GetValueOrDefault(DefaultInstance.{1})", field.NameCamelCase, field.NamePascalCase);
				break;
			case ArgSource.Missing:
				Write("default({0})", field.OptionalTypeName);
				break;
		}

		if (optionalWrap) {
			Write(")");
		}

		firstInSequence = false;
	}

	PopIndent();
}

private enum ParameterStyle {
	Required,
	Optional,
	OptionalOrRequired,
}

private void WriteParameters(IEnumerable<MetaType.MetaField> fields, ParameterStyle style, int indent = 2, bool concatenated = false) {
	PushIndent(new string('\t', indent));
	bool firstInSequence = !concatenated;
	if (style == ParameterStyle.OptionalOrRequired) {
		fields = SortRequiredFieldsFirst(fields);
	}

	foreach(var field in fields) {
		bool fieldOptional = style == ParameterStyle.Optional || (style == ParameterStyle.OptionalOrRequired && !field.IsRequired);
		if (!firstInSequence) { Write(","); }
		WriteLine("");
		Write(fieldOptional ? field.OptionalTypeName : field.TypeName);
		Write(" ");
		Write(field.NameCamelCase);
		if (fieldOptional) {
			Write(" = default(" + field.OptionalTypeName + ")");
		}

		firstInSequence = false;
	}
	PopIndent();
}
	
private static IEnumerable<MetaType.MetaField> SortRequiredFieldsFirst(IEnumerable<MetaType.MetaField> fields) {
	return fields.Where(f => f.IsRequired).Concat(fields.Where(f => !f.IsRequired));
}

[DebuggerDisplay("{TypeName,nq}")]
protected class MetaType {
	public MetaType(Type type) {
		this.Type = type;
		this.RequiredIdentityField = this.CreateField(new SpecialFieldInfo("identity", typeof(uint), required: true));
		this.OptionalIdentityField = this.CreateField(new SpecialFieldInfo("identity", typeof(uint), required: false));
		this.ActualIdentityFieldTypeName = "int"; // this is so Interlocked.Increment works.
	}

	public Type Type { get; private set; }

	public ImmutableList<MetaField> LocalFields {
		get { return this.Type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Select(this.CreateField).ToImmutableList(); }
	}

	public ImmutableList<MetaField> AllFields {
		get {
			var fieldsWithBase = this.LocalFields;
			if (this.HasAncestor) {
				fieldsWithBase = fieldsWithBase.InsertRange(0, this.Ancestor.AllFields);
			}

			return fieldsWithBase;
		}
	}

	public MetaField RequiredIdentityField { get; private set; }

	public MetaField OptionalIdentityField { get; private set; }

	public string ActualIdentityFieldTypeName { get; private set; }

	public MetaField LookupTableField {
		get { return this.CreateField(new SpecialFieldInfo("lookupTable", this.NodeLookupTableTypeName, required: true)); }
	}

	public bool IsRecursiveType {
		get { return this.GetTypeFamily().Any(t => this.Equals(t.RecursiveType)); }
	}

	public bool IsDerivedFromRecursiveType {
		get {
			var recursiveType = this.RecursiveTypeFromFamily;
			return recursiveType != null && recursiveType.Type.IsAssignableFrom(this.Type);
		}
	}

	public bool IsDerivedFromRecursiveParent {
		get {
			var recursiveParent = this.RecursiveParent;
			return recursiveParent != null && recursiveParent.Type.IsAssignableFrom(this.Type);
		}
	}

	public MetaField RecursiveField {
		get { return this.LocalFields.SingleOrDefault(f => f.IsCollection && !f.IsDefinitelyNotRecursive && this.RootAncestorOrThisType.Type.IsAssignableFrom(f.ElementType)); }
	}

	public virtual MetaType RecursiveType {
		get { return this.RecursiveField != null ? this.FindMetaType(this.RecursiveField.ElementType) : null; }
	}

	public virtual MetaType RecursiveTypeFromFamily {
		get { return this.GetTypeFamily().SingleOrDefault(t => t.IsRecursiveType); }
	}

	/// <summary>Gets the type that contains the collection of this (or a base) type.</summary>
	public virtual MetaType RecursiveParent {
		get {
			var result = this.GetTypeFamily().SingleOrDefault(t => t.RecursiveType != null && t.RecursiveType.Type.IsAssignableFrom(this.Type));
			return result;
		 }
	}

	public bool IsRecursiveParent {
		get { return this.Equals(this.RecursiveParent); }
	}

	public bool ChildrenAreOrdered {
		get { return typeof(IReadOnlyList<>).MakeGenericType(this.RecursiveType.Type).IsAssignableFrom(this.RecursiveField.FieldType); }
	}

	public bool ChildrenAreSorted {
		get { return typeof(ImmutableSortedSet<>).MakeGenericType(this.RecursiveType.Type).IsAssignableFrom(this.RecursiveField.FieldType); }
	}

	public bool IsRecursive {
		get { return this.LocalFields.Count(f => f.IsCollection && this.RootAncestorOrThisType.Type.IsAssignableFrom(f.ElementType) && !f.IsDefinitelyNotRecursive) == 1; }
	}
	
	public virtual IEnumerable<MetaType> Ancestors {
		get {
			for (var ancestor = this.Ancestor; ancestor != null; ancestor = ancestor.Ancestor) {
				yield return ancestor;
			}
		}
	}

	public IEnumerable<MetaType> SelfAndAncestors {
		get {
			return new [] { this }.Concat(this.Ancestors);
		}
	}

	public virtual MetaType Ancestor {
		get { return TemplateTypes.FirstOrDefault(mt => mt.Type.IsEquivalentTo(this.Type.BaseType)); }
	}

	public bool HasAncestor {
		get { return this.Ancestor != null; }
	}

	public virtual IEnumerable<MetaType> Descendents {
		get { return TemplateTypes.Where(t => this.Type.IsAssignableFrom(t.Type) && !this.Type.IsEquivalentTo(t.Type)); }
	}

	public bool HasDescendent {
		get { return this.Descendents.Any(); }
	}

	public bool IsAbstract { 
		get { return this.Type.IsAbstract; }
	}

	public virtual MetaType GreenType {
		get { return this; }
	}

	public virtual string TypeName {
		get { return this.Type.Name; }
	}
	
	public string TypeNameCamelCase {
		get { return CamelCase(this.TypeName); }
	}

	public string OptionalTypeName {
		get { return GetOptionalTypeName(this.TypeName); }
	}

	public virtual MetaType RootAncestorOrThisType {
		get {
			MetaType current = this;
			while (current.Ancestor != null) {
				current = current.Ancestor;
			}

			return current;
		}
	}
	
	public virtual string NodeLookupTableTypeName {
		get { return string.Format("System.Collections.Immutable.ImmutableDictionary<{1}, System.Collections.Generic.KeyValuePair<{0}, {1}>>", this.RecursiveType.TypeName, this.RequiredIdentityField.TypeName); }
	}

	public virtual string NodeLookupTableKeyValuePairTypeName {
		get { return string.Format("System.Collections.Generic.KeyValuePair<{0}, {1}>", this.RecursiveType.TypeName, this.RequiredIdentityField.TypeName); }
	}

	public virtual HashSet<MetaType> GetTypeFamily() {
		var set = new HashSet<MetaType>();
		var furthestAncestor = this.Ancestors.LastOrDefault() ?? this;
		set.Add(furthestAncestor);
		foreach(var relative in furthestAncestor.Descendents) {
			set.Add(relative);
		}

		return set;
	}

	public MetaType GetFirstCommonAncestor(MetaType cousin) {
		foreach (var ancestor in this.SelfAndAncestors) {
			if (cousin.SelfAndAncestors.Contains(ancestor)) {
				return ancestor;
			}
		}

		throw new ArgumentException("No common ancestor between " + this.TypeName + " and " + cousin.TypeName);
	}

	public IEnumerable<MetaField> GetFieldsBeyond(MetaType ancestor) {
		var fields = ImmutableList.Create<MetaField>();
		if (!ancestor.Equals(this)) {
			fields = fields.AddRange(this.LocalFields)
			               .InsertRange(0, this.Ancestor.GetFieldsBeyond(ancestor));
		}

		return fields;
	}

	public override int GetHashCode() {
		return this.Type.GetHashCode();
	}

	public override bool Equals(object value) {
		var other = value as MetaType;
		return other != null && this.Type.IsEquivalentTo(other.Type);
	}

	protected virtual MetaField CreateField(FieldInfo field) {
		return new MetaField(field, this);
	}

	protected virtual MetaType FindMetaType(Type type) {
		return TemplateTypes.SingleOrDefault(mt => mt.Type.IsEquivalentTo(type));
	}

	protected virtual string GetTypeName(Type type) {
		var typeName = new StringBuilder();

		if (type.IsGenericType) {
			typeName.Append(type.FullName.Substring(0, type.FullName.IndexOf('`')));
			typeName.Append("<");
			foreach (Type typeArgument in type.GenericTypeArguments) {
				typeName.Append(GetTypeName(typeArgument));
				typeName.Append(", ");
			}

			typeName.Length -= 2;
			typeName.Append(">");
		} else {
			var templateDeclaringType = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.DeclaringType;
			typeName.Append(this.GetTypeNameCore(type, type.DeclaringType == templateDeclaringType));
		}

		return typeName.ToString();
	}

	protected virtual string GetTypeNameCore(Type type, bool isTemplateType) {
		return isTemplateType ? type.Name : type.FullName;
	}
	
	protected static string GetOptionalTypeName(string typeName)
	{
		return "ImmutableObjectGraph.Optional<" + typeName + ">";
	}

	protected virtual string GetOptionalTypeName(Type type)
	{
		return GetOptionalTypeName(this.GetTypeName(type));
	}

	[DebuggerDisplay("{DeclaringType.TypeName,nq}.{NamePascalCase,nq}")]
	public class MetaField {
		public MetaField(FieldInfo field, MetaType declaringType) {
			this.Field = field;
			this.DeclaringType = declaringType;
		}

		public FieldInfo Field { get; private set; }

		public Type FieldType {
			get { return this.Field.FieldType; }
		}

		public virtual MetaField GreenField {
			get { return this; }
		}

		public MetaType DeclaringType { get; private set; }

		public string NamePascalCase {
			get { return PascalCase(this.Field.Name); }
		}

		public string NameCamelCase {
			get { return CamelCase(this.Field.Name); }
		}

		public bool IsRequired {
			get { return this.Field.IsDefined(typeof(RequiredAttribute), false); }
		}

		public bool IsDefinitelyNotRecursive {
			get { return this.Field.IsDefined(typeof(NotRecursiveAttribute), false); }
		}

		public bool IsCollection {
			get { return IsCollectionType(this.Field.FieldType); }
		}

		public bool IsRecursiveCollection {
			get { return this.IsCollection && this.DeclaringType.RecursiveType != null && this.ElementType.IsEquivalentTo(this.DeclaringType.RecursiveType.Type); }
		}

		public bool IsPrimitiveType {
			get { return !TemplateTypes.Any(mt => mt.Type.IsEquivalentTo(this.FieldType)) && !this.IsCollection; }
		}
			
		public virtual Type ElementType {
			get { return GetTypeOrCollectionMemberType(this.Field.FieldType); }
		}

		public string ElementTypeName {
			get { return this.DeclaringType.GetTypeName(GetTypeOrCollectionMemberType(this.Field.FieldType)); }
		}

		public virtual string TypeName {
			get { 
				var specialField = this.Field as SpecialFieldInfo;
				if (specialField != null && this.Field.FieldType == null) {
					return specialField.FieldTypeName;
				}
			
				return this.DeclaringType.GetTypeName(this.Field.FieldType);
			}
		}

		public virtual string OptionalTypeName {
			get { return GetOptionalTypeName(this.TypeName); }
		}

		public virtual string TypeNameForBuilderField {
			get {
				return TemplateTypes.Any(mt => mt.Type.IsEquivalentTo(this.FieldType)) || IsCollectionType(this.FieldType)
					? "ImmutableObjectGraph.Optional<" + this.TypeName + ".Builder>"
					: this.TypeName;
			}
		}

		public string TypeNameForBuilderProperty {
			get {
				return TemplateTypes.Any(mt => mt.Type.IsEquivalentTo(this.FieldType)) || IsCollectionType(this.FieldType)
					? this.TypeName + ".Builder"
					: this.TypeName;
			}
		}

		public virtual string ArgumentReference {
			get { return this.NameCamelCase; }
		}

		public DistinguisherAttribute Distinguisher {
			get { return this.Field.GetCustomAttributes(typeof(DistinguisherAttribute), false).FirstOrDefault() as DistinguisherAttribute; }
		}
	}

	internal class SpecialFieldInfo : FieldInfo {
		private string fieldName;
		private Type fieldType;
		private string fieldTypeName;
		private bool required;

		internal SpecialFieldInfo(string fieldName, Type fieldType, bool required) {
			this.fieldName = fieldName;
			this.fieldType = fieldType;
			this.required = required;
		}

		internal SpecialFieldInfo(string fieldName, string fieldTypeName, bool required) {
			this.fieldName = fieldName;
			this.fieldTypeName = fieldTypeName;
			this.required = required;
		}

		public override FieldAttributes Attributes {
			get { throw new NotImplementedException(); }
		}

		public override Type FieldType {
			get { return this.fieldType; }
		}

		public string FieldTypeName {
			get { return this.fieldTypeName; }
		}

		public override RuntimeFieldHandle FieldHandle {
			get { throw new NotImplementedException(); }
		}

		public override object GetValue(object obj) {
			throw new NotImplementedException();
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}

		public override Type DeclaringType {
			get { throw new NotImplementedException(); }
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(bool inherit) {
			throw new NotImplementedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit) {
			if (attributeType == typeof(RequiredAttribute) && this.required) {
				return true;
			}

			return false;
		}

		public override Type ReflectedType {
			get { throw new NotImplementedException(); }
		}

		public override string Name {
			get { return this.fieldName; }
		}
	}
}
#>
