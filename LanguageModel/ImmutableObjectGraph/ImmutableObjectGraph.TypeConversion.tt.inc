<#
this.HandleType += (templateType, position) => {
	if (position != HandleTypePosition.WithinTypeDeclaration) {
		return;
	}

	if (!templateType.IsAbstract && (templateType.HasAncestor || templateType.Descendents.Any())) {
#>

internal static <#= templateType.TypeName #> CreateWithIdentity(<# WriteParameters(templateType.AllFields.Concat(templateType.OptionalIdentityField), ParameterStyle.OptionalOrRequired); #>) {
	if (!<#= templateType.OptionalIdentityField.NameCamelCase #>.IsDefined) {
		<#= templateType.OptionalIdentityField.NameCamelCase #> = NewIdentity();
	}

	return DefaultInstance<# if (templateType.LocalFields.Count > 0) { #>.WithFactory(<#
	WriteArguments(templateType.AllFields.Concat(templateType.OptionalIdentityField), ArgSource.OptionalArgumentOrTemplate, asOptional: OptionalStyle.Always); 
#>)<# } /* if (templateType.LocalFields.Count > 0) */ #>;
}
<#
	}

	if (templateType.HasAncestor && !templateType.Ancestor.IsAbstract) {
#>

public <#= templateType.Ancestor.TypeName#> To<#= templateType.Ancestor.TypeName#>() {
	return <#= templateType.Ancestor.TypeName #>.CreateWithIdentity(<# WriteArguments(templateType.Ancestor.AllFields.Concat(templateType.RequiredIdentityField), indent: 2, asOptional: OptionalStyle.WhenNotRequired); #>);
}
<#
	}

	foreach (MetaType derivedType in templateType.Descendents.Where(d => !d.IsAbstract)) {
#>

public virtual <#= derivedType.TypeName #> To<#= derivedType.TypeName #>(<# WriteParameters(derivedType.GetFieldsBeyond(templateType), ParameterStyle.OptionalOrRequired, indent: 1); #>) {
	<#= derivedType.TypeName #> that = this as <#= derivedType.TypeName #>;
	if (that != null && this.GetType().IsEquivalentTo(typeof(<#= derivedType.TypeName #>))) {
<#
	var fieldsBeyond = derivedType.GetFieldsBeyond(templateType);
	if (fieldsBeyond.Any()) {
#>
		if (<#
		bool firstInSequence = true;
		PushIndent("\t\t    ");
		foreach (var field in fieldsBeyond) {
			if (!firstInSequence) { WriteLine(" && "); }
			if (field.IsRequired) {
				Write("({0} == that.{1})", field.NameCamelCase, field.NamePascalCase);
			} else {
				Write("(!{0}.IsDefined || {0}.Value == that.{1})", field.NameCamelCase, field.NamePascalCase);
			}

			firstInSequence = false;
		}

		PopIndent();
		#>) {
			return that;
		}
	<#
	} else {
	#>
		return that;
	<# } #>
}

	return <#= derivedType.TypeName #>.CreateWithIdentity(<#
		var initialFields = templateType.AllFields.Concat(templateType.RequiredIdentityField);
		WriteArguments(initialFields, indent: 2, asOptional: OptionalStyle.WhenNotRequired);
		if (fieldsBeyond.Any() && initialFields.Any()) {
			Write(",");
		}

		WriteArguments(fieldsBeyond, ArgSource.Argument, indent: 2);
		#>);
}
<#
		foreach (var ancestor in templateType.Ancestors) { #>

public override <#= derivedType.TypeName #> To<#= derivedType.TypeName #>(<# WriteParameters(derivedType.GetFieldsBeyond(ancestor), ParameterStyle.OptionalOrRequired); #>) {
	return base.To<#= derivedType.TypeName #>(<#
		WriteArguments(templateType.GetFieldsBeyond(ancestor), ArgSource.OptionalArgumentOrProperty, asOptional: OptionalStyle.WhenNotRequired);
		if (templateType.GetFieldsBeyond(ancestor).Any() && derivedType.GetFieldsBeyond(templateType).Any()) {
			Write(",");
		}

		WriteArguments(derivedType.GetFieldsBeyond(templateType), ArgSource.Argument);
		#>);
}
<#
		} // foreach ancestor type
	} // foreach derived type
}; // event handler

this.HandleRedType += (templateType, position) => {
	if (position != HandleTypePosition.WithinTypeDeclaration) {
		return;
	}

	PushIndent("\t");
	foreach (MetaType familyType in templateType.GetTypeFamily().Where(ft => !ft.IsAbstract && !ft.Equals(templateType))) {
		var commonAncestor = familyType.GetFirstCommonAncestor(templateType);
#>

public <#= familyType.TypeName #> To<#= familyType.GreenType.TypeName #>(<# WriteParameters(familyType.GetFieldsBeyond(commonAncestor), ParameterStyle.OptionalOrRequired, indent: 1); #>) {
	var newGreenNode = this.greenNode.To<#= familyType.GreenType.TypeName #>(<# WriteArguments(familyType.GetFieldsBeyond(commonAncestor), ArgSource.Argument); #>);
	var newRoot = this.root.ReplaceDescendent(this.greenNode, newGreenNode);
	return new <#= familyType.TypeName #>(newGreenNode, newRoot);
}
<#
	} // foreach familyType

	PopIndent();
};
#>