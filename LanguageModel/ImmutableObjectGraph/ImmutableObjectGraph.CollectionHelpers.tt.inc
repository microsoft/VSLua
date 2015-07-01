<#
this.HandleType += (templateType, position) => {
	switch (position) {
		case HandleTypePosition.WithinTypeDeclaration:
			if (templateType.IsRecursiveParent) {
				var recursiveField = templateType.RecursiveField;
#>

protected <#= recursiveField.ElementTypeName #> SyncImmediateChildToCurrentVersion(<#= recursiveField.ElementTypeName #> child) {
	<#= recursiveField.ElementTypeName #> currentValue;
	if (!this.TryFindImmediateChild(child.<#= templateType.RequiredIdentityField.NamePascalCase #>, out currentValue)) {
		throw new System.ArgumentException();
	}

	return currentValue;
}
<#
			} // if IsRecursiveParent

			break;
		default:
			break;
	} // switch
};

this.HandleField += (templateType, field) => {
	bool isNew = !field.DeclaringType.Equals(templateType);
	string keywords = isNew ? "new " : string.Empty;
	if (field.IsCollection) {
		var distinguisher = field.Distinguisher;
		string suffix = distinguisher != null ? distinguisher.CollectionModifierMethodSuffix : null;
		string plural = suffix != null ? (this.PluralService.Singularize(field.NamePascalCase) + this.PluralService.Pluralize(suffix)) : field.NamePascalCase;
		string singular = this.PluralService.Singularize(field.NamePascalCase) + suffix;
#>

/// <summary>Replaces the elements of the <#= field.NamePascalCase #> collection with the specified collection.</summary>
public <#= keywords #><#= templateType.TypeName #> With<#= plural #>(params <#= field.ElementTypeName #>[] values) {
<#		if (isNew) { #>
	return (<#= templateType.TypeName #>)base.With<#= plural #>(values);
<#		} else { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.ResetContents(values));
<#		} #>
}

/// <summary>Replaces the elements of the <#= field.NamePascalCase #> collection with the specified collection.</summary>
public <#= keywords #><#= templateType.TypeName #> With<#= plural #>(System.Collections.Generic.IEnumerable<<#= field.ElementTypeName #>> values) {
<#		if (isNew) { #>
	return (<#= templateType.TypeName #>)base.With<#= plural #>(values);
<#		} else { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.ResetContents(values));
<#		} #>
}

/// <summary>Adds the specified elements from the <#= field.NamePascalCase #> collection.</summary>
public <#= keywords #><#= templateType.TypeName #> Add<#= plural #>(System.Collections.Generic.IEnumerable<<#= field.ElementTypeName #>> values) {
<#		if (isNew) { #>
	return (<#= templateType.TypeName #>)base.Add<#= plural #>(values);
<#		} else { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.AddRange(values));
<#		} #>
}

/// <summary>Adds the specified elements from the <#= field.NamePascalCase #> collection.</summary>
public <#= keywords #><#= templateType.TypeName #> Add<#= plural #>(params <#= field.ElementTypeName #>[] values) {
<#		if (isNew) { #>
	return (<#= templateType.TypeName #>)base.Add<#= plural #>(values);
<#		} else { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.AddRange(values));
<#		} #>
}

/// <summary>Adds the specified element from the <#= field.NamePascalCase #> collection.</summary>
public <#= keywords #><#= templateType.TypeName #> Add<#= singular #>(<#= field.ElementTypeName #> value) {
<#		if (isNew) { #>
	return (<#= templateType.TypeName #>)base.Add<#= singular #>(value);
<#		} else { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.Add(value));
<#		} #>
}

/// <summary>Removes the specified elements from the <#= field.NamePascalCase #> collection.</summary>
public <#= keywords #><#= templateType.TypeName #> Remove<#= plural #>(System.Collections.Generic.IEnumerable<<#= field.ElementTypeName #>> values) {
<#		if (isNew) { #>
	return (<#= templateType.TypeName #>)base.Remove<#= plural #>(values);
<#		} else {
			if (field.IsRecursiveCollection) { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.RemoveRange(values.Select(v => this.SyncImmediateChildToCurrentVersion(v))));
<#			} else { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.RemoveRange(values));
<#			}
		} #>
}

/// <summary>Removes the specified elements from the <#= field.NamePascalCase #> collection.</summary>
public <#= keywords #><#= templateType.TypeName #> Remove<#= plural #>(params <#= field.ElementTypeName #>[] values) {
<#		if (isNew) { #>
	return (<#= templateType.TypeName #>)base.Remove<#= plural #>(values);
<#		} else {
			if (field.IsRecursiveCollection) { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.RemoveRange(values.Select(v => this.SyncImmediateChildToCurrentVersion(v))));
<#			} else { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.RemoveRange(values));
<#			}
		} #>
}

/// <summary>Removes the specified element from the <#= field.NamePascalCase #> collection.</summary>
public <#= keywords #><#= templateType.TypeName #> Remove<#= singular #>(<#= field.ElementTypeName #> value) {
<#		if (isNew) { #>
	return (<#= templateType.TypeName #>)base.Remove<#= singular #>(value);
<#		} else {
			if (field.IsRecursiveCollection) { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.Remove(this.SyncImmediateChildToCurrentVersion(value)));
<#			} else { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.Remove(value));
<#			}
		} #>
}

/// <summary>Clears all elements from the <#= field.NamePascalCase #> collection.</summary>
public <#= keywords #><#= templateType.TypeName #> Remove<#= plural #>() {
<#		if (isNew) { #>
	return (<#= templateType.TypeName #>)base.Remove<#= plural #>();
<#		} else { #>
	return this.With(<#= field.NameCamelCase #>: this.<#=field.NamePascalCase#>.Clear());
<#		} #>
}

<#
	} // if field type is a collection
}; // green field event handler

this.HandleRedField += (templateType, redField) => {
	if (redField.IsCollection) {
		var distinguisher = redField.Distinguisher;
		string suffix = distinguisher != null ? distinguisher.CollectionModifierMethodSuffix : null;
		string plural = suffix != null ? (this.PluralService.Singularize(redField.NamePascalCase) + this.PluralService.Pluralize(suffix)) : redField.NamePascalCase;
		string singular = this.PluralService.Singularize(redField.NamePascalCase) + suffix;
		var fieldTypes = new List<MetaType.MetaField> { redField };
		if (redField.GreenField.TypeName != redField.TypeName) {
			fieldTypes.Add(redField.GreenField);
		}

		foreach (var field in fieldTypes) {
			var collectionArgumentField = field.IsRecursiveCollection ? field : field.GreenField;
			var translateOne = field.GreenField.TypeName != field.TypeName ? "." + field.GreenField.ElementTypeName : "";
			var translateCollection = (field.IsRecursiveCollection && field.GreenField.TypeName != field.TypeName) ? (".Select(r => r" + translateOne + ")") : "";
#>

/// <summary>Replaces the elements of the <#= field.NamePascalCase #> collection with the specified collection.</summary>
public <#= templateType.TypeName #> With<#= plural #>(params <#= collectionArgumentField.ElementTypeName #>[] values) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.With<#= plural #>(values<#= translateCollection #>);
	return this.NewSpine(mutatedLeaf);
}

/// <summary>Replaces the elements of the <#= field.NamePascalCase #> collection with the specified collection.</summary>
public <#= templateType.TypeName #> With<#= plural #>(System.Collections.Generic.IEnumerable<<#= collectionArgumentField.ElementTypeName #>> values) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.With<#= plural #>(values<#= translateCollection #>);
	return this.NewSpine(mutatedLeaf);
}

/// <summary>Adds the specified elements from the <#= field.NamePascalCase #> collection.</summary>
public <#= templateType.TypeName #> Add<#= plural #>(System.Collections.Generic.IEnumerable<<#= field.ElementTypeName #>> values) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.Add<#= plural #>(values<#= translateCollection #>);
	return this.NewSpine(mutatedLeaf);
}

/// <summary>Adds the specified elements from the <#= field.NamePascalCase #> collection.</summary>
public <#= templateType.TypeName #> Add<#= plural #>(params <#= field.ElementTypeName #>[] values) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.Add<#= plural #>(values<#= translateCollection #>);
	return this.NewSpine(mutatedLeaf);
}
<#
	if (field.IsRecursiveCollection) {
#>

/// <summary>Adds the specified element from the <#= field.NamePascalCase #> collection.</summary>
public ParentedRecursiveType<<#= templateType.TypeName #>, <#= templateType.RecursiveTypeFromFamily.TypeName #>> Add<#= singular #>(<#= field.ElementTypeName #> value) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.Add<#= singular #>(value<#= translateOne #>);
	var newParent = this.NewSpine(mutatedLeaf);
	var newChild = new <#= templateType.RecursiveTypeFromFamily.TypeName #>(value<#= translateOne #>, newParent.Root<#="." + templateType.RecursiveParent.GreenType.TypeName #>);
	return new ParentedRecursiveType<<#= templateType.TypeName #>, <#= templateType.RecursiveTypeFromFamily.TypeName #>>(newChild, newParent);
}
<#
	} else {
#>

/// <summary>Adds the specified element from the <#= field.NamePascalCase #> collection.</summary>
public <#= templateType.TypeName #> Add<#= singular #>(<#= field.ElementTypeName #> value) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.Add<#= singular #>(value<#= translateOne #>);
	return this.NewSpine(mutatedLeaf);
}
<#
	}
#>

/// <summary>Removes the specified elements from the <#= field.NamePascalCase #> collection.</summary>
public <#= templateType.TypeName #> Remove<#= plural #>(System.Collections.Generic.IEnumerable<<#= field.ElementTypeName #>> values) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.Remove<#= plural #>(values<#= translateCollection #>);
	return this.NewSpine(mutatedLeaf);
}

/// <summary>Removes the specified elements from the <#= field.NamePascalCase #> collection.</summary>
public <#= templateType.TypeName #> Remove<#= plural #>(params <#= field.ElementTypeName #>[] values) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.Remove<#= plural #>(values<#= translateCollection #>);
	return this.NewSpine(mutatedLeaf);
}

/// <summary>Removes the specified element from the <#= field.NamePascalCase #> collection.</summary>
public <#= templateType.TypeName #> Remove<#= singular #>(<#= field.ElementTypeName #> value) {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.Remove<#= singular #>(value<#= translateOne #>);
	return this.NewSpine(mutatedLeaf);
}
<#
		}
#>

/// <summary>Clears all elements from the <#= redField.NamePascalCase #> collection.</summary>
public <#= templateType.TypeName #> Remove<#= plural #>() {
	this.ThrowIfDefault();
	var mutatedLeaf = this.greenNode.Remove<#= plural #>();
	return this.NewSpine(mutatedLeaf);
}
<#	} // if field type is a collection
}; // red field event handler
#>