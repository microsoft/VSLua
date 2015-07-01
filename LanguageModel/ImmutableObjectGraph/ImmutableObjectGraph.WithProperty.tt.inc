<#
// TODO: Rather than declare these WithProperty() methods on every type (base and derived)
//       we should instead declare them ONCE on a static class as extension methods:
//       public static class SomeTypeWith {
//           public static T WithProperty(this T self, string value) where T : TBase {
//               return (T)self.With(property: value);
//           }
//       }
//       That retains the ability for folks to find these methods on base and derived types,
//       And for the return type to be the same as the original type, but without having to
//       define the method on so many types. Reduction in code gen size is usually a good thing.

this.HandleField += (templateType, field) => { 
	if (field.DeclaringType.Equals(templateType)) { // field is declared locally
#>

/// <summary>Returns a new instance with the <#= field.NamePascalCase #> property set to the specified value.</summary>
public <#= templateType.TypeName #> With<#= field.NamePascalCase #>(<#= field.TypeName #> value) {
	if (value == this.<#= field.NamePascalCase #>) {
		return this;
	}

	return this.With(<#= field.NameCamelCase #>: Optional.For(value));
}
<#
	} else { // else field is declared on a base type
#>

/// <summary>Returns a new instance with the <#= field.NamePascalCase #> property set to the specified value.</summary>
public new <#= templateType.TypeName #> With<#= field.NamePascalCase #>(<#= field.TypeName #> value) {
	return (<#= templateType.TypeName #>)base.With<#= field.NamePascalCase #>(value);
}
<#
	}
}; // event handler

this.HandleRedField += (templateType, field) => {
	if (field.IsRecursiveCollection) {
#>

/// <summary>Returns a new instance with the <#= field.NamePascalCase #> property set to the specified value.</summary>
public <#= templateType.TypeName #> With<#= field.NamePascalCase #>(<#= field.TypeName #> value) {
	this.ThrowIfDefault();
	var adapter = (ImmutableObjectGraph.Adapters.IImmutableCollectionAdapter<<#= templateType.GreenType.RecursiveTypeFromFamily.TypeName #>>)value;
	var mutatedLeaf = this.greenNode.With<#= field.NamePascalCase #>(adapter.UnderlyingCollection);
	return this.NewSpine(mutatedLeaf);
}
<#
	} else {
#>

/// <summary>Returns a new instance with the <#= field.NamePascalCase #> property set to the specified value.</summary>
public <#= templateType.TypeName #> With<#= field.NamePascalCase #>(<#= field.GreenField.TypeName #> value) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.With<#= field.NamePascalCase #>(value);
	return this.NewSpine(mutatedLeaf);
}
<#
	}
};
#>