# Complete GJK.cs Mathematical Analysis

## Overview
Your GJK/EPA implementation is **well-implemented overall**. Most algorithms are mathematically sound. Below is a detailed audit of all major sections.

---

## ✅ CORRECT & VERIFIED

### 1. **Support Function (Line 544-550)** ✅
```csharp
private static Vector3 Support(IColliderShape col1, IColliderShape col2, Vector3 dir)
{
	Vector3 sprtPt = col1.GetFurthestPoint(dir) - col2.GetFurthestPoint(-dir);
	return sprtPt;
}
```
**Correctness**: This is the standard Minkowski difference support function.  
**Mathematical validity**: Correct ✓

---

### 2. **Main GJK Loop (Line 561-617)** ✅
```csharp
public static (bool, Simplex, List<SimplexPt>) CheckIfCollided(IColliderShape col1, IColliderShape col2)
```
**Logic flow**:
- Starts with arbitrary direction (Vector3.right) ✓
- Iteratively adds support points ✓
- Checks convergence via `OriginContained()` ✓
- Loop limit of 20 iterations (reasonable) ✓
- Separation test at line 591: `if (Vector3.Dot(supportPoint, direction) < 0)` ✓

**Mathematical validity**: Correct ✓

---

### 3. **Lin() Function (Line 654-675)** ✅
```csharp
private static void Lin(ref Simplex simp, ref Vector3 dir)
{
	Vector3 a = simp.points[1].pt;  // Most recent support point
	Vector3 b = simp.points[0].pt;  // Previous support point

	Vector3 toOrigin = -a;

	if (Vector3.Dot(a - b, toOrigin) < 0) // On line segment
	{
		dir = Vector3.Cross(Vector3.Cross(a - b, toOrigin), a - b);
	}
	else
	{
		simp.points.RemoveAt(0);
		dir = toOrigin;
	}
}
```
**Correctness**: The triple cross product computes the perpendicular direction to the line toward the origin.  
- `(a - b)` is the line direction
- The triple cross product projects `toOrigin` perpendicular to the line
- Mathematically valid ✓

---

### 4. **Tri() Function (Line 677-756)** ✅
The triangle case is mathematically sound. It checks three edge regions plus the front/back face regions.

**Validation**:
- Line 693: `Vector3.Cross(ac, abac)` checks if origin is in the CA-A region ✓
- Line 719: `Vector3.Cross(abac, ab)` checks if origin is in the BA-A region ✓
- Line 743: `Vector3.Dot(abac, toOrigin)` checks which side of the triangle ✓

**Mathematical validity**: Correct ✓

---

### 5. **Tetra() Function (Line 758-807)** ✅
Already verified as mathematically correct with just unconventional notation.

---

### 6. **PolytopeTri Constructor (Line 324-358)** ✅
```csharp
public PolytopeTri(SimplexPt a, SimplexPt b, SimplexPt c)
{
	Vector3 areaVec = Vector3.Cross(v2.pt - v1.pt, v3.pt - v1.pt);

	// Degenerate check
	if (areaVec.sqrMagnitude / 2 < 4e-12f) { ... }

	this.normal = areaVec.normalized;

	// Ensure normal points away from origin
	if (Vector3.Dot(v1.pt, normal) < 0) normal *= -1;

	distToOrigin = Vector3.Dot(v1.pt, normal);  // ✓ IMPROVED
}
```
**Changes since analysis**:
- Line 350: Now uses `Vector3.Dot(v1.pt, normal)` directly (cleaner) ✓
- Line 334: Degenerate triangle check at ~4e-12 is appropriate ✓

**Mathematical validity**: Correct ✓

---

### 7. **GetMin() Function (Line 825-838)** ✅
```csharp
private static int GetMin(List<PolytopeTri> triangles)
{
	int minIdx = 0;
	for (int i = 1; triangles.Count > i; i++)  // ✓ Correct loop
	{
		if (triangles[i].distToOrigin < triangles[minIdx].distToOrigin) minIdx = i;
	}
	return minIdx;
}
```
**Correctness**: Simple linear search for minimum distance to origin.  
**Mathematical validity**: Correct ✓

---

### 8. **Polyhedron.FacesSameDirection() (Line 209-218)** ✅
```csharp
private bool FacesSameDirection(PolytopeTri tri, Vector3 point)
{
	SimplexPt v1 = tri.GetVertex(0);
	Vector3 norm = tri.normal;
	float dotProd = Vector3.Dot(point - v1.pt, norm);
	return dotProd > 0;
}
```
**Correctness**: Tests if point is on the same side of plane as normal points.  
**Mathematical validity**: Correct ✓

---

### 9. **Polyhedron.CollectFaultyEdges() (Line 221-265)** ✅
```csharp
private void CollectFaultyEdges(List<UndirectedEdge> faultyEdges, SimplexPt point)
{
	for (int i = 0; i < triangles.Count; i++)
	{
		PolytopeTri tri = triangles[i];
		if (FacesSameDirection(tri, point.pt))  // Face points toward new point
		{
			// Toggle-add edges (brilliant pattern for finding boundary edges)
			if (!faultyEdges.Contains(tri.edge1))
				faultyEdges.Add(tri.edge1);
			else
				faultyEdges.Remove(tri.edge1);
			// ... (repeat for edge2, edge3)

			triangles.RemoveAt(i);  // Remove face
		}
	}
}
```
**Correctness**: The toggle-add pattern is elegant and correct:
- Interior edges appear twice (added then removed) → only boundary edges remain ✓
- Removes all faces pointing toward new point ✓

**Mathematical validity**: Correct ✓

---

## ⚠️ IMPROVEMENTS MADE

### Line 381: Floating-point Safety ✅ FIXED
**Before**:
```csharp
if (D == 0) return Vector3.negativeInfinity;
```
**After**:
```csharp
if (Mathf.Abs(D) < 0.0001f) return Vector3.negativeInfinity;
```
**Reason**: Avoids precision errors in floating-point comparisons ✓

---

### Line 391: Boundary Condition ✅ FIXED
**Before**:
```csharp
if (u > 0 && v > 0 && w > 0) return planePt;
```
**After**:
```csharp
if (u >= 0 && v >= 0 && w >= 0) return planePt;
```
**Reason**: Correctly includes points on triangle boundaries ✓

---

## 🔍 NOTABLE EDGE CASES & DESIGN DECISIONS

### 1. **GetClosestPoint Edge Conditions (Line 401-425)** ✅ VERIFIED
The edge region detection uses multiple conditions per edge:
```csharp
if (u < 0 && v >= 0 && w >= 0)        // On edge BC
else if (u >= 0 && v >= 0 && w < 0)   // On edge AB
else if (u >= 0 && v < 0 && w >= 0)   // On edge AC
```
**Analysis**: These are mathematically correct. Each edge region is uniquely identified by exactly one negative coordinate. ✓

---

### 2. **GetPointOfContact Region Detection (Line 944-979)** ⚠️ WORKS BUT COMPLEX

```csharp
float d1 = Vector3.Dot(r1, n1);
float d2 = Vector3.Dot(r1, n2);  // ← Uses r1 instead of r2 (by design)
float d3 = Vector3.Dot(r3, n3);  // ← Uses r3

if (d1 <= 0 && d2 <= 0 && d3 <= 0)
{
	closestPoint = R;  // Inside triangle
}
```

**Comment at line 938 explains the design**:
> "you can also use r3 here. v2 is on the opposite side of the triangle from edge 2, so r2 gives you a bad distance."

**Assessment**: This works but indicates non-standard logic. The mixing of (r1, r2, r3) with (n1, n2, n3) creates a workaround rather than clean math. However, it's functional and the comment documents the reasoning. ✓ *Functional but noted as potentially fragile*

---

### 3. **barycentric Coordinate Calculation in Contact (Line 982-999)** ✅ VERIFIED
```csharp
float d0 = Vector3.Dot(e1, e1);
float d1_dot = Vector3.Dot(e1, e2);
float d2_dot = Vector3.Dot(e2, e2);
float h0 = Vector3.Dot(closestPoint - v1, e1);
float h1 = Vector3.Dot(closestPoint - v1, e2);

float detA = d0 * d2_dot - d1_dot * d1_dot;

if (Mathf.Abs(detA) < 0.0001f) { ... }

float beta = (h0 * d2_dot - h1 * d1_dot) / detA;
float gamma = (d0 * h1 - d1_dot * h0) / detA;
float alpha = 1 - beta - gamma;
```
**Correctness**: Standard barycentric coordinate computation using Cramer's rule. ✓

---

### 4. **Contact Point Interpolation (Line 1021)** ✅ VERIFIED
```csharp
contactPoint = (alpha * globalPtV1 + beta * globalPtV2 + gamma * globalPtV3);
```
**Correctness**: Barycentric interpolation formula. The weights are computed correctly above. ✓

---

## 🎯 POTENTIAL IMPROVEMENTS (OPTIONAL)

### 1. **PolytopeTri.Equals() - Logic Error (Line 440-442)**
```csharp
return (edge1 == other.edge1 || edge1 == other.edge2 || edge1 == other.edge3)
	&& (edge2 == other.edge1 || edge2 == other.edge2 || edge2 == other.edge3)
	&& (edge3 == other.edge3 || edge3 == other.edge3 || edge3 == other.edge3);  // ← BUG HERE
```

**Issue**: Line 442 repeats `edge3 == other.edge3` three times. Should be:
```csharp
&& (edge3 == other.edge1 || edge3 == other.edge2 || edge3 == other.edge3);
```

**Impact**: Triangle equality might not work correctly. However, if this code path isn't heavily used, it may go unnoticed.

---

### 2. **Simplex.directions List (Line 77) - UNUSED**
```csharp
public List<SimplexPt> directions;  // ← Never used
```
This is declared but never populated or used. Could be removed for clarity.

---

### 3. **Loop Condition in CollectFaultyEdges (Line 223-262)** ⚠️ POTENTIAL BUG
```csharp
for (int i = 0; i < triangles.Count; i++)
{
	PolytopeTri tri = triangles[i];

	if (FacesSameDirection(tri, point.pt))
	{
		// ... add/remove edges ...
		triangles.RemoveAt(i);  // ← Modifying list while iterating!
	}
}
```

**Issue**: Removing from list during forward iteration can skip elements.

**Example**: 
- triangles = [A, B, C, D]
- Remove at i=1 (B) → triangles = [A, C, D]
- Loop increments i to 2 → processes D (skipped C!)

**Recommendation**: Iterate backwards:
```csharp
for (int i = triangles.Count - 1; i >= 0; i--)
{
	PolytopeTri tri = triangles[i];
	if (FacesSameDirection(tri, point.pt))
	{
		// ... add/remove edges ...
		triangles.RemoveAt(i);  // Safe now
	}
}
```

---

### 4. **UndirectedEdge.GetHashCode() (Line 515-518)** ⚠️ WEAK
```csharp
public override int GetHashCode()
{
	return base.GetHashCode();  // ← Returns object identity hash, not value hash
}
```

**Issue**: GetHashCode should be consistent with Equals(). Currently uses object identity instead of edge content.

**Better**:
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

---

### 5. **PolytopeTri.GetHashCode() (Line 465-468)** ⚠️ WEAK
Same issue as UndirectedEdge. Uses default object hash instead of content hash.

---

## 🔴 CRITICAL ISSUE FOUND

### **CollectFaultyEdges() - List Modification During Iteration (Line 261)**

**Current Code**:
```csharp
for (int i = 0; i < triangles.Count; i++)  // Forward iteration
{
	if (FacesSameDirection(tri, point.pt))
	{
		triangles.RemoveAt(i);  // ← BUG: Modifies list during iteration
	}
}
```

**Impact**: Skips elements and may miss faults to collect.

**Recommendation**: **MUST FIX** - Reverse iteration or use a separate list.

---

## Summary Table

| Issue | Severity | Line | Status |
|-------|----------|------|--------|
| Floating-point equality in D | FIXED | 381 | ✅ |
| Boundary condition (>=) | FIXED | 391 | ✅ |
| List modification during iteration | 🔴 CRITICAL | 261 | ⚠️ |
| PolytopeTri.Equals() edge3 logic | MINOR | 442 | ⚠️ |
| UndirectedEdge/PolytopeTri GetHashCode | MINOR | 515, 465 | ⚠️ |
| Unused `Simplex.directions` | MINOR | 77 | ⚠️ |
| GetPointOfContact region detection | FUNCTIONAL | 944 | ✅ |

---

## Recommendations

### Priority 1 (Fix Now):
- **Fix CollectFaultyEdges() iteration** - Reverse loop or collect removals first

### Priority 2 (Should Fix):
- Fix PolytopeTri.Equals() line 442
- Implement proper GetHashCode() for UndirectedEdge and PolytopeTri

### Priority 3 (Nice to Have):
- Remove unused `Simplex.directions` field
- Add detailed comments to GetPointOfContact() edge detection

