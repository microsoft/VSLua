<#
this.ContributeBaseTypes += (templateType, baseTypes) => {
	if (templateType.IsRecursiveType) {
		baseTypes.Add("IRecursiveDiffingType<" + templateType.TypeName + "ChangedProperties, " + templateType.TypeName + ".DiffGram>");
	}
};

this.HandleType += (templateType, position) => {
	if (templateType.IsRecursiveType) {
		string enumTypeName = GetPropertiesEnumTypeName(templateType);
		string diffTypeName = GetDiffGramTypeName(templateType);
		switch (position) {
			case HandleTypePosition.WithinTypeDeclaration:
#>

/// <summary>
/// A description of a change made to an instance of an immutable object.
/// </summary>
[DebuggerDisplay("{Kind} {(Before ?? After).Caption} ({<#= templateType.RequiredIdentityField.NamePascalCase #>}) {Changes}")]
public struct DiffGram {
	private DiffGram(<#= templateType.TypeName #> before, <#= templateType.TypeName #> after, ChangeKind kind, <#= enumTypeName #> changes)
		: this() {
		this.Before = before;
		this.After = after;
		this.Kind = kind;
		this.Changes = changes;
	}

	public static DiffGram Change(<#= templateType.TypeName #> before, <#= templateType.TypeName #> after, <#= enumTypeName #> changes) {
		return new DiffGram(before, after, ChangeKind.Replaced, changes);
	}

	public static DiffGram Add(<#= templateType.TypeName #> value) {
		return new DiffGram(null, value, ChangeKind.Added, default(<#= enumTypeName #>));
	}

	public static DiffGram Remove(<#= templateType.TypeName #> value) {
		return new DiffGram(value, null, ChangeKind.Removed, default(<#= enumTypeName #>));
	}

	/// <summary>
	/// Gets the leaf node before the change.
	/// </summary>
	public <#= templateType.TypeName #> Before { get; private set; }

	/// <summary>
	/// Gets the leaf node after the change.
	/// </summary>
	public <#= templateType.TypeName #> After { get; private set; }

	/// <summary>
	/// Gets the kind of change made to the alterered node.
	/// </summary>
	public ChangeKind Kind { get; private set; }

	/// <summary>
	/// Gets the kinds of changes made to node if <see cref="Kind"/> is <see cref="ChangeKind.Replaced"/>.
	/// </summary>
	public <#= enumTypeName #> Changes { get; private set; }
<#	foreach (var field in templateType.AllFields.Where(f => !f.IsRecursiveCollection)) { #>

	/// <summary>
	/// Gets a value indicating whether the <see cref="<#= field.DeclaringType.TypeName #>.<#= field.NamePascalCase #>" /> was changed.
	/// </summary>
	public bool Is<#=field.NamePascalCase#>Changed {
		get { return (this.Changes & <#= enumTypeName #>.<#= field.NamePascalCase #>) != <#= enumTypeName #>.None; }
	}
<#	} #>

	/// <summary>
	/// Gets the identity of the affected object.
	/// </summary>
	public <#= templateType.RequiredIdentityField.TypeName #> <#= templateType.RequiredIdentityField.NamePascalCase #> {
		get { return (this.Before ?? this.After).<#= templateType.RequiredIdentityField.NamePascalCase #>; }
	}
}

public static class Comparers {
	/// <summary>Gets an equatable comparer that considers only the persistent identity of a pair of values.</summary>
	public static System.Collections.Generic.IEqualityComparer<<#= templateType.TypeName #>> <#= templateType.RequiredIdentityField.NamePascalCase #> {
		get { return ImmutableObjectGraph.Comparers.Identity; }
	}

	/// <summary>Gets an equatable comparer that compares all properties between two instances.</summary>
	public static System.Collections.Generic.IEqualityComparer<<#= templateType.TypeName #>> ByValue {
		get { return ImmutableObjectGraph.Comparers.ByValue<<#= templateType.TypeName #>ChangedProperties, DiffGram>(deep: false); }
	}

	/// <summary>Gets an equatable comparer that considers all properties between two instances and their children.</summary>
	public static System.Collections.Generic.IEqualityComparer<<#= templateType.TypeName #>> ByValueWithDescendents {
		get { return ImmutableObjectGraph.Comparers.ByValue<<#= templateType.TypeName #>ChangedProperties, DiffGram>(deep: true); }
	}

	internal static System.Collections.Generic.IEqualityComparer<ParentedRecursiveType<<#= templateType.RecursiveParent.TypeName #>, <#= templateType.RecursiveTypeFromFamily.TypeName #>>> Parented<#= templateType.TypeName #><#= templateType.RequiredIdentityField.NamePascalCase #> {
		get { return ImmutableObjectGraph.Comparers.Parented<<#= templateType.RecursiveParent.TypeName #>, <#= templateType.TypeName #>>(); }
	}
}

<#= templateType.TypeName #>ChangedProperties IRecursiveDiffingType<<#= templateType.TypeName #>ChangedProperties, <#= templateType.TypeName #>.DiffGram>.ParentProperty {
	get { return <#= templateType.TypeName #>ChangedProperties.Parent; }
}

<#= templateType.TypeName #>ChangedProperties IRecursiveDiffingType<<#= templateType.TypeName #>ChangedProperties, <#= templateType.TypeName #>.DiffGram>.PositionUnderParentProperty {
	get { return <#= templateType.TypeName #>ChangedProperties.PositionUnderParent; }
}

<#= templateType.TypeName #>ChangedProperties IRecursiveDiffingType<<#= templateType.TypeName #>ChangedProperties, <#= templateType.TypeName #>.DiffGram>.DiffProperties(IRecursiveType other) {
	return this.DiffProperties((<#= templateType.TypeName #>)other);
}

<#= templateType.TypeName #>.DiffGram IRecursiveDiffingType<<#= templateType.TypeName #>ChangedProperties, <#= templateType.TypeName #>.DiffGram>.Change(IRecursiveType before, IRecursiveType after, <#= templateType.TypeName #>ChangedProperties diff) {
	return DiffGram.Change((<#= templateType.TypeName #>)before, (<#= templateType.TypeName #>)after, diff);
}

<#= templateType.TypeName #>.DiffGram IRecursiveDiffingType<<#= templateType.TypeName #>ChangedProperties, <#= templateType.TypeName #>.DiffGram>.Add(IRecursiveType after) {
	return DiffGram.Add((<#= templateType.TypeName #>)after);
}

<#= templateType.TypeName #>.DiffGram IRecursiveDiffingType<<#= templateType.TypeName #>ChangedProperties, <#= templateType.TypeName #>.DiffGram>.Remove(IRecursiveType before) {
	return DiffGram.Remove((<#= templateType.TypeName #>)before);
}

bool IRecursiveDiffingType<<#= templateType.TypeName #>ChangedProperties, <#= templateType.TypeName #>.DiffGram>.Equals(<#= templateType.TypeName #>ChangedProperties first, <#= templateType.TypeName #>ChangedProperties second) {
	return first == second;
}

<#= templateType.TypeName #>ChangedProperties IRecursiveDiffingType<<#= templateType.TypeName #>ChangedProperties, <#= templateType.TypeName #>.DiffGram>.Union(<#= templateType.TypeName #>ChangedProperties first, <#= templateType.TypeName #>ChangedProperties second) {
	return first | second;
}

protected virtual <#= enumTypeName #> DiffProperties(<#= templateType.TypeName #> other) {
	if (other == null) {
		throw new System.ArgumentNullException("other");
	}

	var propertiesChanged = <#= enumTypeName #>.None;

	if (this != other) {
		if (!this.GetType().IsEquivalentTo(other.GetType())) {
			propertiesChanged |= <#= enumTypeName #>.Type;
		}
<#
	var additionalFields = templateType.LocalFields.Where(f => !f.IsRecursiveCollection);
	foreach (var field in additionalFields) {
#>

		if (this.<#= field.NamePascalCase #> != other.<#= field.NamePascalCase #>) {
			propertiesChanged |= <#= enumTypeName #>.<#= field.NamePascalCase #>;
		}
<#
	}
#>
	}

	return propertiesChanged;
}
<#
				break;
			case HandleTypePosition.AfterTypeDeclaration:
				var fields = templateType.Concat(templateType.Descendents)
					.SelectMany(t => t.LocalFields)
					.Where(f => !f.IsRecursiveCollection)
					.GroupBy(f => f.NamePascalCase);
				int fieldsCount = 4 + fields.Count();
				string enumBaseTypeName = fieldsCount > 32 ? "ulong" : "uint";
#>

[System.Flags]
public enum <#= enumTypeName #> : <#=enumBaseTypeName#> {
	/// <summary>
	/// No change was made to the node.  It was either entirely added or removed. 
	/// </summary>
	None = 0x0,

	/// <summary>
	/// The type of the node was changed.
	/// </summary>
	Type = 0x1,

	/// <summary>
	/// The node's position within its parent's list of children changed.
	/// </summary>
	PositionUnderParent = 0x2,
	
	/// <summary>
	/// The node was removed and reappeared in a new position in the tree (with a different parent).
	/// </summary>
	Parent = 0x4,
<#
				int counter = 3;
				// We're interested in enumerating the union of all fields on this and derived types,
				// excluding the recursive collection itself.
				foreach (var field in fields) {
#>

	/// <summary>
	/// The <#= field.Key #> property was changed.
	/// </summary>
	<#= field.Key #> = <#= string.Format("0x{0:x}", (ulong)Math.Pow(2, counter)) #>,
<#
					counter++;
				}
#>

	/// <summary>
	/// All flags in this enum.
	/// </summary>
	All = Type | PositionUnderParent | Parent<#
	foreach (var field in fields) {
		Write(" | ");
		Write(field.Key);
	}
#>,
}
<#
				break;
			default:
				break;
		}
	} else if (templateType.IsDerivedFromRecursiveType) {
		MetaType recursiveType = templateType.RecursiveTypeFromFamily;
		string enumTypeName = GetPropertiesEnumTypeName(recursiveType);
		string diffTypeName = GetDiffGramTypeName(recursiveType);
		switch (position) {
			case HandleTypePosition.WithinTypeDeclaration:
				var additionalFields = templateType.LocalFields.Where(f => !f.IsRecursiveCollection);
				if (additionalFields.Any()) {
#>

protected override <#= enumTypeName #> DiffProperties(<#= recursiveType.TypeName #> other) {
	var propertiesChanged = base.DiffProperties(other);

	var other<#= templateType.TypeName #> = other as <#= templateType.TypeName #>;
	if (other<#= templateType.TypeName #> != null) {<#
	foreach (var field in additionalFields) {
#>

		if (this.<#= field.NamePascalCase #> != other<#= templateType.TypeName #>.<#= field.NamePascalCase #>) {
			propertiesChanged |= <#= enumTypeName #>.<#= field.NamePascalCase #>;
		}
<#
	}
#>
	}

	return propertiesChanged;
}
<#
				}

				break;
			default:
				break;
		} // switch
	} // if derived from recursive type

	if (templateType.IsRecursiveParent) {
		MetaType recursiveType = templateType.RecursiveTypeFromFamily;
		string enumTypeName = GetPropertiesEnumTypeName(recursiveType);
		string diffTypeName = GetDiffGramTypeName(recursiveType);
		string parentedTemplateType = "Parented" + templateType.RecursiveType.TypeName;
		switch (position) {
			case HandleTypePosition.WithinTypeDeclaration:
				string changesSinceKeyword = templateType.HasAncestor ? "override " : "virtual ";
				break;
			default:
				break;
		} // switch
	} // if IsRecursiveParent
}; // green type event handler

this.HandleRedType += (templateType, position) => {
	MetaType recursiveType = templateType.RecursiveTypeFromFamily;
	string enumTypeName = GetPropertiesEnumTypeName(recursiveType.GreenType);
	string diffTypeName = GetDiffGramTypeName(recursiveType.GreenType);
	switch (position) {
		case HandleTypePosition.WithinTypeDeclaration:
#>

	public System.Collections.Generic.IReadOnlyList<<#= diffTypeName #>> ChangesSince(<#= templateType.TypeName #> priorVersion) {
		this.ThrowIfDefault();
		return this.greenNode.ChangesSince(priorVersion.<#= templateType.GreenType.TypeName #>);
	}
<#
			break;
		default:
			break;
	}
}; // red type event handler
#><#+
	private string GetPropertiesEnumTypeName(MetaType templateType) {
		return templateType.TypeName + "ChangedProperties";
	}

	private string GetDiffGramTypeName(MetaType templateType) {
		return templateType.TypeName + ".DiffGram";
	}
#>
