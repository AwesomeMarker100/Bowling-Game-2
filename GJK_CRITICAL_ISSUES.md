# GJK.cs - Critical Issues & Fixes

## 🔴 CRITICAL BUG - List Modification During Iteration

**Location**: Line 221-265 in `CollectFaultyEdges()`

**Current Code**:
```csharp
for (int i = 0; i < triangles.Count; i++)
{
	PolytopeTri tri = triangles[i];

	if (FacesSameDirection(tri, point.pt))
	{
		// Toggle-add edges...
		triangles.RemoveAt(i);  // ← BUG: Modifies list during forward iteration!
	}
}
```

**Problem**: Removing an item shifts remaining items, causing iteration to skip elements.

**Example**:
```
triangles = [TRI_A, TRI_B, TRI_C, TRI_D]
i=0: Process TRI_A
i=1: Remove TRI_B → [TRI_A, TRI_C, TRI_D]
i=2: Now pointing at TRI_D, but TRI_C was skipped!
```

**Fix**:
```csharp
// Option 1: Reverse iteration (safe)
for (int i = triangles.Count - 1; i >= 0; i--)
{
	PolytopeTri tri = triangles[i];

	if (FacesSameDirection(tri, point.pt))
	{
		// Toggle-add edges...
		triangles.RemoveAt(i);  // ✓ Safe now
	}
}

// Option 2: Collect removals first
List<int> indicesToRemove = new List<int>();
for (int i = 0; i < triangles.Count; i++)
{
	if (FacesSameDirection(triangles[i], point.pt))
	{
		// Toggle-add edges...
		indicesToRemove.Add(i);
	}
}
for (int i = indicesToRemove.Count - 1; i >= 0; i--)
{
	triangles.RemoveAt(indicesToRemove[i]);
}
```

---

## 🟡 MINOR BUG - PolytopeTri.Equals() Line 442

**Current**:
```csharp
return (edge1 == other.edge1 || edge1 == other.edge2 || edge1 == other.edge3)
	&& (edge2 == other.edge1 || edge2 == other.edge2 || edge2 == other.edge3)
	&& (edge3 == other.edge3 || edge3 == other.edge3 || edge3 == other.edge3);  // ← Copy-paste error
```

**Fix**:
```csharp
return (edge1 == other.edge1 || edge1 == other.edge2 || edge1 == other.edge3)
	&& (edge2 == other.edge1 || edge2 == other.edge2 || edge2 == other.edge3)
	&& (edge3 == other.edge1 || edge3 == other.edge2 || edge3 == other.edge3);
```

---

## 🟡 MINOR BUG - GetHashCode() Implementations

Both `UndirectedEdge` (line 515) and `PolytopeTri` (line 465) use the default object hash.

**Current**:
```csharp
public override int GetHashCode()
{
	return base.GetHashCode();  // ← Returns object identity hash
}
```

**Fix for UndirectedEdge**:
```csharp
public override int GetHashCode()
{
	unchecked
	{
		int hash = 17;
		hash = hash * 31 + v1.GetHashCode();
		hash = hash * 31 + v2.GetHashCode();
		return hash;
	}
}
```

**Fix for PolytopeTri**:
```csharp
public override int GetHashCode()
{
	unchecked
	{
		int hash = 17;
		hash = hash * 31 + v1.GetHashCode();
		hash = hash * 31 + v2.GetHashCode();
		hash = hash * 31 + v3.GetHashCode();
		return hash;
	}
}
```

---

## 📋 What's Correct

✅ All GJK simplex logic (Lin, Tri, Tetra)  
✅ EPA algorithm and polytope expansion  
✅ Support function  
✅ Barycentric coordinate calculations  
✅ Floating-point safety checks (already improved)  
✅ Boundary condition checks (already improved)  

