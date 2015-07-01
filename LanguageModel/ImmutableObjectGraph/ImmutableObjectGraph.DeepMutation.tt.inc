<#
this.HandleType += (templateType, position) => {
	if (position != HandleTypePosition.WithinTypeDeclaration) {
		return;
	}

	if (templateType.IsRecursive) {
#>

public <#= templateType.TypeName #> AddDescendent(<#= templateType.RecursiveType.TypeName #> value, <#= templateType.RecursiveParent.TypeName #> parent) {
	var spine = this.GetSpine(parent);
	var newParent = parent.Add<#= templateType.RecursiveField.NamePascalCase #>(value);
	var newSpine = System.Collections.Immutable.ImmutableStack.Create(value, newParent);
	return (<#= templateType.TypeName #>)this.ReplaceDescendent(spine, newSpine, spineIncludesDeletedElement: false).Peek();
}

public <#= templateType.TypeName #> RemoveDescendent(<#= templateType.RecursiveType.TypeName #> value) {
	var spine = this.GetSpine(value);
	var spineList = spine.ToList();
	var parent = (<#= templateType.RecursiveParent.TypeName #>)spineList[spineList.Count - 2];
	var newParent = parent.Remove<#= templateType.RecursiveField.NamePascalCase #>(spineList[spineList.Count - 1]);

	var newSpine = System.Collections.Immutable.ImmutableStack.Create((<#= templateType.RecursiveType.TypeName #>)newParent);
	return (<#= templateType.RecursiveParent.TypeName #>)this.ReplaceDescendent(spine, newSpine, spineIncludesDeletedElement: true).Peek();
}

/// <summary>Replaces one node with a modified version of itself (same identity) among this node's descendents</summary>
public <#= templateType.TypeName #> ReplaceDescendent(<#= templateType.RecursiveType.TypeName #> updatedNode) {
	var spine = this.GetSpine(updatedNode.Identity);

	if (spine.IsEmpty) {
		// The descendent was not found.
		throw new System.ArgumentException("Old value not found");
	}

	return (<#= templateType.TypeName #>)this.ReplaceDescendent(spine, System.Collections.Immutable.ImmutableStack.Create(updatedNode), spineIncludesDeletedElement: false).Peek();
}

/// <summary>Replaces one node with another node that may have a different identity.</summary>
public <#= templateType.TypeName #> ReplaceDescendent(<#= templateType.RecursiveType.TypeName #> current, <#= templateType.RecursiveType.TypeName #> replacement) {
	var spine = this.GetSpine(current);

	if (spine.IsEmpty) {
		// The descendent was not found.
		throw new System.ArgumentException("Old value not found");
	}

	return (<#= templateType.TypeName #>)this.ReplaceDescendent(spine, System.Collections.Immutable.ImmutableStack.Create(replacement), spineIncludesDeletedElement: false).Peek();
}

private System.Collections.Immutable.ImmutableStack<<#= templateType.RecursiveType.TypeName #>> ReplaceDescendent(System.Collections.Immutable.ImmutableStack<<#= templateType.RecursiveType.TypeName #>> spine, System.Collections.Immutable.ImmutableStack<<#= templateType.RecursiveType.TypeName #>> replacementStackTip, bool spineIncludesDeletedElement) {
	Debug.Assert(this == spine.Peek());
	var remainingSpine = spine.Pop();
	if (remainingSpine.IsEmpty || (spineIncludesDeletedElement && remainingSpine.Pop().IsEmpty)) {
		// This is the instance to be changed.
		return replacementStackTip;
	}

	System.Collections.Immutable.ImmutableStack<<#= templateType.RecursiveType.TypeName #>> newChildSpine;
	var child = remainingSpine.Peek();
	var recursiveChild = child as <#= templateType.RecursiveParent.TypeName #>;
	if (recursiveChild != null) {
		newChildSpine = recursiveChild.ReplaceDescendent(remainingSpine, replacementStackTip, spineIncludesDeletedElement);
	} else {
		Debug.Assert(remainingSpine.Pop().IsEmpty); // we should be at the tail of the stack, since we're at a leaf.
		Debug.Assert(this.<#= templateType.RecursiveField.NamePascalCase #>.Contains(child));
		newChildSpine = replacementStackTip;
	}

	var newChildren = this.<#= templateType.RecursiveField.NamePascalCase #>.Replace(child, newChildSpine.Peek());
	var newSelf = this.With<#= templateType.RecursiveField.NamePascalCase #>(newChildren);
	if (newSelf.lookupTable == lookupTableLazySentinal && this.lookupTable != null && this.lookupTable != lookupTableLazySentinal) {
		// Our newly mutated self wants a lookup table. If we already have one we can use it,
		// but it needs to be fixed up given the newly rewritten spine through our descendents.
		newSelf.lookupTable = this.FixupLookupTable(ImmutableDeque.Create(newChildSpine), ImmutableDeque.Create(remainingSpine));
		newSelf.ValidateInternalIntegrityDebugOnly();
	}

	return newChildSpine.Push(newSelf);
}

/// <summary>
/// Produces a fast lookup table based on an existing one, if this node has one, to account for an updated spine among its descendents.
/// </summary>
/// <param name="updatedSpine">
/// The spine of this node's new descendents' instances that are created for this change.
/// The head is an immediate child of the new instance for this node.
/// The tail is the node that was added or replaced.
/// </param>
/// <param name="oldSpine">
/// The spine of this node's descendents that have been changed in this delta.
/// The head is an immediate child of this instance.
/// The tail is the node that was removed or replaced.
/// </param>
/// <returns>An updated lookup table.</returns>
private <#= templateType.NodeLookupTableTypeName #> FixupLookupTable(ImmutableObjectGraph.ImmutableDeque<<#= templateType.RecursiveType.TypeName #>> updatedSpine, ImmutableObjectGraph.ImmutableDeque<<#= templateType.RecursiveType.TypeName #>> oldSpine) {
	if (this.lookupTable == null || this.lookupTable == lookupTableLazySentinal) {
		// We don't already have a lookup table to base this on, so leave it to the new instance to lazily construct.
		return lookupTableLazySentinal;
	}

	if ((updatedSpine.IsEmpty && oldSpine.IsEmpty) ||
		(updatedSpine.Count > 1 && oldSpine.Count > 1 && System.Object.ReferenceEquals(updatedSpine.PeekHead(), oldSpine.PeekHead()))) {
		// No changes were actually made.
		return this.lookupTable;
	}

	var lookupTable = this.lookupTable.ToBuilder();

	// Classify the kind of change that has just occurred.
	var oldSpineTail = oldSpine.PeekTail();
	var newSpineTail = updatedSpine.PeekTail();
	ChangeKind changeKind;
	bool childrenChanged = false;
	if (updatedSpine.Count == oldSpine.Count) {
		changeKind = ChangeKind.Replaced;
		var oldSpineTailRecursive = oldSpineTail as <#= templateType.RecursiveParent.TypeName #>;
		var newSpineTailRecursive = newSpineTail as <#= templateType.RecursiveParent.TypeName #>;
		if (oldSpineTailRecursive != null || newSpineTailRecursive != null) {
			// Children have changed if either before or after type didn't have a children property,
			// or if both did, but the children actually changed.
			childrenChanged = oldSpineTailRecursive == null || newSpineTailRecursive == null
				|| !System.Object.ReferenceEquals(oldSpineTailRecursive.<#= templateType.RecursiveField.NamePascalCase #>, newSpineTailRecursive.<#= templateType.RecursiveField.NamePascalCase #>);
		}
	} else if (updatedSpine.Count > oldSpine.Count) {
		changeKind = ChangeKind.Added;
	} else // updatedSpine.Count < oldSpine.Count
	{
		changeKind = ChangeKind.Removed;
	}

	// Trim the lookup table of any entries for nodes that have been removed from the tree.
	if (childrenChanged || changeKind == ChangeKind.Removed) {
		// We need to remove all descendents of the old tail node.
		lookupTable.RemoveRange(oldSpineTail.GetSelfAndDescendents().Select(n => n.Identity));
	} else if (changeKind == ChangeKind.Replaced && oldSpineTail.Identity != newSpineTail.Identity) {
		// The identity of the node was changed during the replacement.  We must explicitly remove the old entry
		// from our lookup table in this case.
		lookupTable.Remove(oldSpineTail.Identity);

		// We also need to update any immediate children of the old spine tail
		// because the identity of their parent has changed.
		var oldSpineTailRecursive = oldSpineTail as <#= templateType.RecursiveParent.TypeName #>;
		if (oldSpineTailRecursive != null) {
			foreach (var child in oldSpineTailRecursive) {
				lookupTable[child.Identity] = new System.Collections.Generic.KeyValuePair<<#= templateType.RecursiveType.TypeName #>, <#= templateType.RequiredIdentityField.TypeName #>>(child, newSpineTail.Identity);
			}
		}
	}

	// Update our lookup table so that it includes (updated) entries for every member of the spine itself.
	<#= templateType.RecursiveType.TypeName #> parent = this;
	foreach (var node in updatedSpine) {
		// Remove and add rather than use the Set method, since the old and new node are equal (in identity) therefore the map class will
		// assume no change is relevant and not apply the change.
		lookupTable.Remove(node.Identity);
		lookupTable.Add(node.Identity, new System.Collections.Generic.KeyValuePair<<#= templateType.RecursiveType.TypeName #>, <#= templateType.RequiredIdentityField.TypeName #>>(node, parent.Identity));
		parent = node;
	}

	// There may be children on the added node that we should include.
	if (childrenChanged || changeKind == ChangeKind.Added) {
		var recursiveParent = parent as <#= templateType.RecursiveParent.TypeName #>;
		if (recursiveParent != null) {
			recursiveParent.ContributeDescendentsToLookupTable(lookupTable);
		}
	}

	return lookupTable.ToImmutable();
}

/// <summary>
/// Validates this node and all its descendents <em>only in DEBUG builds</em>.
/// </summary>
[Conditional("DEBUG")]
private void ValidateInternalIntegrityDebugOnly() {
	this.ValidateInternalIntegrity();
}

/// <summary>
/// Validates this node and all its descendents.
/// </summary>
protected internal void ValidateInternalIntegrity() {
	// Each node id appears at most once.
	var observedIdentities = new System.Collections.Generic.HashSet<<#= templateType.RequiredIdentityField.TypeName #>>();
	foreach (var node in this.GetSelfAndDescendents()) {
		if (!observedIdentities.Add(node.Identity)) {
			throw new RecursiveChildNotUniqueException(node.Identity);
		}
	}

	// The lookup table (if any) accurately describes the contents of this tree.
	if (this.lookupTable != null && this.lookupTable != lookupTableLazySentinal) {
		// The table should have one entry for every *descendent* of this node (not this node itself).
		int expectedCount = this.GetSelfAndDescendents().Count() - 1;
		int actualCount = this.lookupTable.Count;
		if (actualCount != expectedCount) {
			throw new System.ApplicationException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Expected {0} entries in lookup table but found {1}.", expectedCount, actualCount));
		}

		this.ValidateLookupTable(this.lookupTable);
	}
}

/// <summary>
/// Validates that the contents of a lookup table are valid for all descendent nodes of this node.
/// </summary>
/// <param name="lookupTable">The lookup table being validated.</param>
private void ValidateLookupTable(<#= templateType.NodeLookupTableTypeName #> lookupTable) {
	const string ErrorString = "Lookup table integrity failure.";

	foreach (var child in this.<#= templateType.RecursiveField.NamePascalCase #>) {
		var entry = lookupTable[child.Identity];
		if (!object.ReferenceEquals(entry.Key, child)) {
			throw new System.ApplicationException(ErrorString);
		}

		if (entry.Value != this.Identity) {
			throw new System.ApplicationException(ErrorString);
		}

		var recursiveChild = child as <#= templateType.RecursiveParent.TypeName #>;
		if (recursiveChild != null) {
			recursiveChild.ValidateLookupTable(lookupTable);
		}
	}
}
<#
	}
}; // event handler
#>