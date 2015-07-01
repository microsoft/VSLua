<#
this.HandleType += (templateType, position) => {
	if (position != HandleTypePosition.BeforeTypeDeclaration) {
		return;
	}
#>

public interface I<#= templateType.TypeName #><#
	if (templateType.HasAncestor) {
		Write(" : I{0}", templateType.Ancestor.TypeName);
	}
#> {
<#
	foreach(var field in templateType.LocalFields) { #>
	<#= field.TypeName #> <#= field.NamePascalCase #> { get; }
<#	}
#>
}
<#
}; // event handler
#>