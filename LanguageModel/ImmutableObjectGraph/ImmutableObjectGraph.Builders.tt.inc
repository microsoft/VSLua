<#
this.HandleType += (templateType, position) => {
	if (position != HandleTypePosition.WithinTypeDeclaration || !templateType.AllFields.Any()) {
		return;
	}
#>

public <#= templateType.HasAncestor ? "new ": "" #>Builder ToBuilder() {
	return new Builder(this);
}
<# if (!templateType.IsAbstract) { #>

public static <#= templateType.HasAncestor ? "new ": "" #>Builder CreateBuilder() {
	return new Builder(DefaultInstance);
}
<# } #>

public <#= templateType.HasAncestor ? "new ": "" #>partial class Builder<#
	if (templateType.HasAncestor && templateType.Ancestor.AllFields.Any()) {
		Write(" : {0}.Builder", templateType.Ancestor.TypeName);
	}
#> {
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private <#= templateType.TypeName #> immutable;
<#		foreach(var field in templateType.LocalFields) { #>

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	protected <#= field.TypeNameForBuilderField #> <#= field.NameCamelCase #>;
<#		} #>

	internal Builder(<#= templateType.TypeName #> immutable)<#
		if (templateType.HasAncestor && templateType.Ancestor.AllFields.Any()) {
			Write(" : base(immutable)");
		} #> {
		this.immutable = immutable;

<#
		foreach(var field in templateType.LocalFields.Where(f => f.IsPrimitiveType)) {
#>
		this.<#= field.NameCamelCase #> = immutable.<#= field.NamePascalCase #>;
<#			} #>
	}
<#		foreach(var field in templateType.LocalFields) { #>

	public <#= field.TypeNameForBuilderProperty #> <#= field.NamePascalCase #> {
		get {
<# if (!field.IsPrimitiveType) { #>
			if (!this.<#= field.NameCamelCase #>.IsDefined) {
				this.<#= field.NameCamelCase #> = this.immutable.<#= field.NameCamelCase #> != null ? this.immutable.<#= field.NameCamelCase #>.ToBuilder() : null;
			}

			return this.<#= field.NameCamelCase #>.Value;
<# } else { #>
			return this.<#= field.NameCamelCase #>;
<# } #>
		}

		set {
			this.<#= field.NameCamelCase #> = value;
		}
	}
<#		} #>

	public <#= templateType.HasAncestor ? "new ": "" #><#= templateType.TypeName #> ToImmutable() {
<#			foreach(var templateField in templateType.AllFields.Where(f => !f.IsPrimitiveType)) { #>
		var <#= templateField.NameCamelCase #> = this.<#= templateField.NameCamelCase #>.IsDefined ? (this.<#= templateField.NameCamelCase #>.Value != null ? this.<#= templateField.NameCamelCase #>.Value.ToImmutable() : null) : this.immutable.<#= templateField.NamePascalCase #>;
<#			} #>
		return this.immutable = this.immutable<# if (templateType.LocalFields.Count > 0) { #>.With(<#
		bool firstInSequence = true;
		foreach(var field in templateType.AllFields) {
			if (!firstInSequence) { Write(","); }
			WriteLine("");
			if (!field.IsPrimitiveType) { #>
			ImmutableObjectGraph.Optional.For(<#= field.NameCamelCase #>)<#
			} else { #>
			ImmutableObjectGraph.Optional.For(this.<#= field.NamePascalCase #>)<#
			}
			firstInSequence = false;
		} #>
)<# } /* if (templateType.LocalFields.Count > 0) */ #>;
	}
}
<#
}; // event handler
#>