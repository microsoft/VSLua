<#
HiddenTypes.Add(typeof(RedMetaType));
this.HandleType += (templateType, position) => {
	var redType = new RedMetaType(templateType);
	if (redType.RecursiveParent == null) {
		return;
	}

	switch (position) {
		case HandleTypePosition.WithinTypeDeclaration:
			if (redType.IsDerivedFromRecursiveParent) {
#>

public <#= redType.TypeName #> AsRoot {
	get { return new <#= redType.TypeName #>(this, this); }
}
<#
			}
#>

public <#= redType.HasAncestor ? "new " : "" #><#= redType.TypeName #> WithRoot(<#= redType.GreenType.RecursiveParent.TypeName #> root) {
	var spine = root.GetSpine(this);
	if (spine.IsEmpty) {
		throw new System.ArgumentException("Root does not belong to the same tree.");
	}

	return new <#= redType.TypeName #>(this, root);
}
<#
			break;
		case HandleTypePosition.AfterTypeDeclaration:
			this.OnHandleRedType(redType, HandleTypePosition.BeforeTypeDeclaration);
			var recursiveParent = redType.SelfAndAncestors.FirstOrDefault(a => a.Equals(redType.RecursiveParent));
			var baseTypes = new List<string>();
			baseTypes.Add("System.IEquatable<" + redType.TypeName + ">");
			baseTypes.Add(recursiveParent != null ? ("IRecursiveParent<" + redType.RecursiveTypeFromFamily.TypeName + ">") : "IRecursiveType");
#>

public partial struct <#= redType.TypeName #> : <#= String.Join(", ", baseTypes) #> {
<#
			if (recursiveParent != null) {
#>
	private static readonly System.Func<<#=recursiveParent.RecursiveType.TypeName#>, <#=recursiveParent.GreenType.RecursiveType.TypeName#>> toUnrooted = r => r.<#=recursiveParent.GreenType.RecursiveType.TypeName#>;
	private static readonly System.Func<<#=recursiveParent.GreenType.RecursiveType.TypeName#>, <#=recursiveParent.GreenType.RecursiveParent.TypeName#>, <#=recursiveParent.RecursiveType.TypeName#>> toRooted = (u, r) => new <#=recursiveParent.RecursiveType.TypeName#>(u, r);

<#
			}
#>
	private readonly <#= redType.GreenType.TypeName #> greenNode;

	private readonly <#= redType.GreenType.RecursiveParent.TypeName #> root;
<#
			if (recursiveParent != null) {
#>
	private Optional<<#= ((RedMetaType.RedMetaField)recursiveParent.RecursiveField).RootedCollectionTypeAdapterName #>> <#= recursiveParent.RecursiveField.NameCamelCase #>;
<#
			}
#>

	internal <#= redType.TypeName #>(<#= redType.GreenType.TypeName #> <#= redType.GreenType.TypeNameCamelCase #>, <#= redType.GreenType.RecursiveParent.TypeName #> root) {
		this.greenNode = <#= redType.GreenType.TypeNameCamelCase #>;
		this.root = root;
<#
			if (recursiveParent != null) {
#>
		this.<#= recursiveParent.RecursiveField.NameCamelCase #> = default(Optional<<#= ((RedMetaType.RedMetaField)recursiveParent.RecursiveField).RootedCollectionTypeAdapterName #>>);
<#
			}
#>
	}

	public static implicit operator <#= redType.GreenType.TypeName #>(<#= redType.TypeName #> rooted) {
		return rooted.<#= redType.GreenType.TypeName #>;
	}

	public static bool operator ==(<#= redType.TypeName #> that, <#= redType.TypeName #> other) {
		return that.<#= redType.GreenType.TypeName #> == other.<#= redType.GreenType.TypeName #>
		    && that.Root.<#= redType.GreenType.RecursiveParent.TypeName #> == other.Root.<#= redType.GreenType.RecursiveParent.TypeName #>;
	}

	public static bool operator !=(<#= redType.TypeName #> that, <#= redType.TypeName #> other) {
		return !(that == other);
	}

	/// <summary>Gets the parent of this object in the hierarchy.</summary>
	public <#= redType.RecursiveParent.TypeName #> Parent {
		get {
			this.ThrowIfDefault();
			var greenParent = this.root.GetParent(this.<#= redType.GreenType.TypeName #>);
			return greenParent != null ? new <#=redType.RecursiveParent.TypeName#>(greenParent, this.root) : default(<#= redType.RecursiveParent.TypeName #>);
		}
	}

	public <#= redType.RecursiveParent.TypeName #> Root {
		get { return this.root != null ? this.root.AsRoot : default(<#= redType.RecursiveParent.TypeName #>); }
	}

	public uint Identity {
		get {
			this.ThrowIfDefault();
			return (uint)this.greenNode.Identity;
		}
	}
<#
			foreach (var descendent in redType.Descendents) {
				var greenDescendent = ((RedMetaType)descendent).GreenType;
#>

	public bool Is<#=greenDescendent.TypeName#> {
		get { return this.greenNode is <#=greenDescendent.TypeName#>; }
	}

	public <#=descendent.TypeName#> As<#=greenDescendent.TypeName#> {
		get {
			var downcast = this.greenNode as <#=greenDescendent.TypeName#>;
			return downcast != null ? new <#=descendent.TypeName#>(downcast, this.root) : default(<#=descendent.TypeName#>);
		}
	}
<#
			}

			foreach (var ancestor in redType.Ancestors) {
				var greenAncestor = ((RedMetaType)ancestor).GreenType;
#>

	public <#=ancestor.TypeName#> As<#=greenAncestor.TypeName#> {
		get { return this.greenNode != null ? new <#=ancestor.TypeName#>((<#=greenAncestor.TypeName#>)this.greenNode, this.root) : default(<#=ancestor.TypeName#>); }
	}
<#
			}

			if (redType.RecursiveParent.Equals(redType)) {
#>

	public bool IsRoot {
		get { return this.root == this.greenNode; }
	}
<#
			}

			foreach (var field in redType.AllFields) {
				if (field.IsRecursiveCollection) {
#>

	public <#= ((RedMetaType.RedMetaField)field).RootedCollectionTypeName #> <#= field.NamePascalCase #> {
		get {
			if (!this.<#= field.NameCamelCase #>.IsDefined) {
				this.ThrowIfDefault();
				this.<#= field.NameCamelCase #> = Optional.For(Adapter.Create(this.greenNode.<#= field.NamePascalCase #>, toRooted, toUnrooted, this.root));
			}

			return this.<#= field.NameCamelCase #>.Value;
		}
	}
<#
				} else {
#>

	public <#= field.GreenField.TypeName #> <#= field.NamePascalCase #> {
		get {
			this.ThrowIfDefault();
			return this.greenNode.<#= field.NamePascalCase #>;
		}
	}
<#
				}

				this.OnHandleRedField(redType, field);
			}
#>

	/// <summary>Gets the unrooted representation of this object in the hierarchy.</summary>
	public <#= redType.GreenType.TypeName #> <#= redType.GreenType.TypeName #> {
		get { return this.greenNode; }
	}

	/// <summary>Returns a new instance of this object with any number of properties changed.</summary>
	public <#= redType.TypeName #> With(<#
		WriteParameters(redType.AllFields, ParameterStyle.Optional);
#>) {
		this.ThrowIfDefault();
		var newGreenNode = this.greenNode.With(<# WriteArguments(redType.AllFields, ArgSource.Argument); #>);
		var newRoot = this.root.ReplaceDescendent(this.greenNode, newGreenNode);
		return new <#= redType.TypeName #>(newGreenNode, newRoot);
	}
<#
			if (redType.IsRecursive) {
				if (!redType.IsAbstract) {
#>

	public static <#= redType.TypeName #> Create(<# WriteParameters(redType.AllFields, ParameterStyle.OptionalOrRequired); #>) {
		var greenNode = <#= redType.GreenType.TypeName#>.Create(<# WriteArguments(redType.AllFields, ArgSource.Argument); #>);
		return greenNode.AsRoot;
	}
<#
				}
#>

	public <#=redType.RecursiveType.TypeName#> Find(<#= templateType.RequiredIdentityField.TypeName #> identity) {
		this.ThrowIfDefault();
		return new <#=redType.RecursiveType.TypeName#>(this.greenNode.Find(identity), this.root);
	}

	public bool TryFind(<#= redType.RequiredIdentityField.TypeName #> identity, out <#= redType.RecursiveType.TypeName #> value) {
		this.ThrowIfDefault();
		<#= redType.GreenType.RecursiveType.TypeName #> greenValue;
		if (this.greenNode.TryFind(identity, out greenValue)) {
			value = new <#= redType.RecursiveType.TypeName #>(greenValue, this.root);
			return true;
		}

		value = default(<#= redType.RecursiveType.TypeName #>);
		return false;
	}

	public System.Collections.Generic.IEnumerator<<#=redType.RecursiveType.TypeName#>> GetEnumerator() {
		return this.<#=redType.RecursiveField.NamePascalCase#>.GetEnumerator();
	}
<#
			} // if IsRecursive

			this.OnHandleRedType(redType, HandleTypePosition.WithinTypeDeclaration);
#>

	public override bool Equals(object obj) {
		if (obj is <#= redType.TypeName #>) {
			var other = (<#= redType.TypeName #>)obj;
			return this.Equals(other);
		}

		return false;
	}

	public bool Equals(<#= redType.TypeName #> other) {
		return this.greenNode == other.greenNode && this.root == other.root;
	}

	public override int GetHashCode() {
		return this.greenNode == null ? 0 : this.greenNode.GetHashCode();
	}

	private <#= redType.TypeName #> NewSpine(<#= redType.GreenType.TypeName #> leaf) {
		var newRoot = this.root.ReplaceDescendent(this.greenNode, leaf);
		return new <#= redType.TypeName #>(leaf, newRoot);
	}

	/// <summary>Gets a value indicating whether this struct has not been initialized to represent an object.</summary>
	public bool IsDefault {
		get { return this.greenNode == null; }
	}

	/// <summary>Throws an exception if this struct does not have a backing <#= redType.GreenType.TypeName #>.</summary>
	private void ThrowIfDefault() {
		if (this.greenNode == null) {
			throw new System.InvalidOperationException();
		}
	}
<#
	if (recursiveParent != null) {
#>

	System.Collections.Generic.IEnumerable<IRecursiveType> IRecursiveParent.Children {
		get {
			this.ThrowIfDefault();
			return this.Children.Cast<IRecursiveType>();
		}
	}
	
	System.Collections.Generic.IEnumerable<<#= redType.RecursiveTypeFromFamily.TypeName #>> IRecursiveParent<<#= redType.RecursiveTypeFromFamily.TypeName #>>.Children {
		get {
			this.ThrowIfDefault();
			var that = this;
			return this.greenNode.Children.Select(c => new <#= redType.RecursiveTypeFromFamily.TypeName #>(c, that.root));
		}
	}

	ParentedRecursiveType<IRecursiveParent<IRecursiveType>, IRecursiveType> IRecursiveParent.GetParentedNode(<#= templateType.RequiredIdentityField.TypeName #> identity) {
		this.ThrowIfDefault();
		var result = this.greenNode.GetParentedNode(identity);
		return new ParentedRecursiveType<IRecursiveParent<IRecursiveType>, IRecursiveType>(result.Value, result.Parent);
	}
<#
	}
#>
}
<#
			this.OnHandleRedType(redType, HandleTypePosition.AfterTypeDeclaration);
			break;
		default:
			break;
	} // switch
}; // event handler
#><#+
[DebuggerDisplay("{TypeName,nq} (red)")]
protected class RedMetaType : MetaType {
	private readonly MetaType greenType;

	public RedMetaType(MetaType greenType)
		: base(greenType.Type) {
		if (greenType is RedMetaType) {
			throw new ArgumentException("Red type handed in as green type.");
		}

		this.greenType = greenType;
	}

	public override MetaType GreenType {
		get { return this.greenType; }
	}

	public override string TypeName {
		get { return "Rooted" + base.TypeName; }
	}

	public override MetaType RecursiveType {
		get { return Wrap(base.RecursiveType); }
	}

	public override IEnumerable<MetaType> Ancestors {
		get { return Wrap(base.Ancestors); }
	}
	
	public override MetaType Ancestor {
		get { return Wrap(base.Ancestor); }
	}

	public override IEnumerable<MetaType> Descendents {
		get { return Wrap(base.Descendents); }
	}

	public override bool Equals(object value) {
		var other = value as RedMetaType;
		return other != null && this.GreenType.Equals(other.GreenType);
	}

	public override int GetHashCode() {
		return this.GreenType.GetHashCode();
	}

	private static RedMetaType Wrap(MetaType type) {
		return type != null ? new RedMetaType(type) : null;
	}

	private static IEnumerable<MetaType> Wrap(IEnumerable<MetaType> type) {
		return type.Select(t => t is RedMetaType ? t : new RedMetaType(t));
	}
	
	protected override MetaField CreateField(FieldInfo field) {
		return new RedMetaField(field, this);
	}

	protected override string GetTypeNameCore(Type type, bool isTemplateType) {
		if (isTemplateType) {
			return HasRootedTypeVariant(type) ? ("Rooted" + type.Name) : type.Name;
		} else {
			return type.FullName;
		}
	}
	
	private bool HasRootedTypeVariant(Type type) {
		if (!TemplateTypes.Any(tt => tt.Type == type && tt.RecursiveParent != null)) {
			return false;
		}

		return true;
	}

	[DebuggerDisplay("{DeclaringType.TypeName,nq}.{NamePascalCase,nq} (red)")]
	public class RedMetaField : MetaField {
		public RedMetaField(FieldInfo field, MetaType declaringType)
			: base(field, declaringType) {
		}

		public string RootedCollectionTypeAdapterName {
			get {
				return string.Format(
					CultureInfo.InvariantCulture,
					"ImmutableObjectGraph.Adapters.Immutable{3}RootAdapter<{0}, {1}, {2}>",
					this.DeclaringType.RecursiveParent.GreenType.RecursiveType.TypeName,
					this.DeclaringType.RecursiveParent.RecursiveType.TypeName,
					this.DeclaringType.RecursiveParent.GreenType.RecursiveParent.TypeName,
					this.CollectionType);
			}
		}

		public string RootedCollectionTypeName {
			get {
				return string.Format(
					CultureInfo.InvariantCulture,
					"System.Collections.Immutable.IImmutable{1}<{0}>",
					this.DeclaringType.RecursiveParent.RecursiveType.TypeName,
					this.CollectionType);
			}
		}

		public override MetaField GreenField {
			get { return ((RedMetaType)this.DeclaringType).GreenType.LocalFields.Single(f => f.Field == this.Field); }
		}

		public override string TypeName {
			get {
				if (this.IsRecursiveCollection) {
					return string.Format(
						"System.Collections.Immutable.IImmutable{1}<{0}>",
						this.ElementTypeName,
						this.CollectionType);
				} else {
					return base.TypeName;
				}
			}
		}

		public override string ArgumentReference {
			get {
				if (this.IsRecursiveCollection) {
					return string.Format("{1}.IsDefined ? ({2})((ImmutableObjectGraph.Adapters.IImmutableCollectionAdapter<{0}>){1}.Value).UnderlyingCollection : default({3})", this.GreenField.ElementTypeName, this.NameCamelCase, this.GreenField.TypeName, this.GreenField.OptionalTypeName);
				} else {
					return base.ArgumentReference;
				}
			}
		}

		private string CollectionType {
			get {
				bool ordered = false;
				Type type = this.FieldType;
				if (type.IsArray) {
					ordered = true;
				}

				if (type.IsGenericType && type.GenericTypeArguments.Length == 1) {
					Type immutableSet = typeof(IImmutableSet<>);
					var genericImmutableSet = immutableSet.MakeGenericType(type.GenericTypeArguments[0]);
					if (!genericImmutableSet.IsAssignableFrom(type)) {
						Type readOnlyList = typeof(IReadOnlyList<>);
						var genericReadOnlyList = readOnlyList.MakeGenericType(type.GenericTypeArguments[0]);
						if (genericReadOnlyList.IsAssignableFrom(type)) {
							ordered = true;
						}
					}
				}

				return ordered ? "List" : "Set";
			}
		}
	}
}
#>
