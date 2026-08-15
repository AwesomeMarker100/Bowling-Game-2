# GJK.cs - Mathematical Correctness Summary

## Quick Reference: Issues by Category

### 🔴 CRITICAL (Must Fix)

#### 1. GetClosestPoint() - Barycentric Boundary Logic (Line 389)
```
CURRENT (WRONG):
if (u > 0 && u < 1 && v > 0 && v < 1 && w > 0 && w < 1) return planePt;

PROBLEM: Since u + v + w = 1, all three cannot be strictly between 0 and 1 simultaneously.
This condition is NEVER true for interior points.

SHOULD BE:
if (u >= 0 && v >= 0 && w >= 0) return planePt;
or simply: if (u >= 0 && v >= 0) return planePt;  // w = 1 - u - v
```

**Impact**: All interior triangle points are misclassified, causing incorrect closest point calculations.

---

#### 2. Simplex.AddPoint() & RemovePoint() - Incomplete (Lines 81-89)
```
CURRENT (INCOMPLETE):
public void AddPoint(SimplexPt pt) {
	if (points.Contains(pt)) { Debug.LogException(...); return; }
	// NOTHING! Points never added
}

SHOULD BE:
public void AddPoint(SimplexPt pt) {
	if (points.Contains(pt)) { Debug.LogException(...); return; }
	points.Add(pt);  // <- MISSING LINE
}

public void RemovePoint(SimplexPt pt) {
	if (!points.Contains(pt)) { Debug.LogException(...); return; }
	points.Remove(pt);  // <- MISSING LINE
}
```

**Impact**: If AddPoint ever gets called (instead of direct simp.points.Add), simplex never updates. Silent failure.

---

### 🟠 MAJOR (Should Fix)

#### 3. GetClosestPoint() - Edge Region Conditions (Lines 399-423)

**Current Issues**:
```
Line 399: if (u < 0 && v > 0 && w < 1)     // w < 1 is wrong
Line 407: else if (u > 0 && v < 1 && w < 0) // v < 1 is wrong  
Line 416: else if(u > 0 && v < 0 && w < 1)  // w < 1 is wrong
```

**Problem**: These use `< 1` instead of checking signs. Since u + v + w = 1:
- For edge BC (u < 0): Need v ≥ 0 AND w ≥ 0 (not w < 1)
- For edge AB (w < 0): Need u ≥ 0 AND v ≥ 0 (not v < 1)
- For edge AC (v < 0): Need u ≥ 0 AND w ≥ 0 (not w < 1)

**Correct Logic**:
```csharp
if (u < 0 && v >= 0 && w >= 0)       // On edge BC
else if (u >= 0 && v >= 0 && w < 0)  // On edge AB
else if (u >= 0 && v < 0 && w >= 0)  // On edge AC
```

---

#### 4. Tetra() - Normal Vector Calculations (Lines 765-767) - ✅ VERIFIED CORRECT

**Current**:
```csharp
Vector3 abc = Vector3.Cross(a - b, a - c);  // Unconventional notation
Vector3 acd = Vector3.Cross(a - c, a - d);
Vector3 abd = Vector3.Cross(a - d, a - b);
```

**Status**: MATHEMATICALLY CORRECT, just unconventional notation.

**Proof**: The identity `(a - b) × (a - c) = (b - a) × (c - a)` proves these produce identical normals to the standard form:
```csharp
// Standard textbook form:
Vector3 abc = Vector3.Cross(b - a, c - a);

// Your form (mathematically equivalent):
Vector3 abc = Vector3.Cross(a - b, a - c);  // Just reordered
```

**Verdict**: Since your GJK/EPA works perfectly, this is proven correct. The rotating pattern is just a stylistic choice. No changes needed.

The `//confirmed` comment on line 766 correctly indicates this was already verified.

---

#### 5. Lin() - Triple Cross Product Clarity (Line 663)

**Current**:
```csharp
dir = Vector3.Cross(Vector3.Cross(a - b, toOrigin), a - b);
```

**Better Style** (clearer):
```csharp
Vector3 line = a - b;
dir = Vector3.Cross(line, Vector3.Cross(line, toOrigin));
// This computes: projection of toOrigin perpendicular to line
```

**Or simpler** (if the goal is to project toOrigin onto plane perpendicular to line):
```csharp
Vector3 lineDir = (a - b).normalized;
Vector3 perpendicular = toOrigin - Vector3.Dot(toOrigin, lineDir) * lineDir;
dir = perpendicular.normalized;
```

---

### 🟡 MODERATE (Could Improve)

#### 6. GetPointOfContact() - Edge Normal Workaround (Lines 912-940)

**Current Code**:
```csharp
Vector3 n1 = Vector3.Cross(e1, nPlane).normalized;
if (Vector3.Dot(n1, v3 - v1) < 0) n1 *= -1;

Vector3 n2 = Vector3.Cross(e2, nPlane).normalized;
if (Vector3.Dot(n2, v2 - v1) < 0) n2 *= -1;  // Comment says "r2 gives bad distance"

Vector3 n3 = Vector3.Cross(e3, nPlane).normalized;
if (Vector3.Dot(n3, v1 - v2) < 0) n3 *= -1;
```

**Problem**: Line 937 uses `r1` instead of `r2` because "r2 is on opposite side of triangle." This suggests the edge normal orientation logic is fragile.

**Better Approach**: Use closest point on edge directly:
```csharp
// For each edge, compute closest point on that edge to origin projection
float d1 = (closestPoint is on edge e1) ? 0 : Vector3.Dot(closestPoint - closestPointOnE1, n1);
float d2 = (closestPoint is on edge e2) ? 0 : Vector3.Dot(closestPoint - closestPointOnE2, n2);
float d3 = (closestPoint is on edge e3) ? 0 : Vector3.Dot(closestPoint - closestPointOnE3, n3);
```

---

#### 7. SimplexPt Tolerance (Line 35)

**Current**:
```csharp
MinoMath.VApproximately(pt, b.pt, 0.005f) && MinoMath.VApproximately(dir, b.dir, 0.005f)
```

**Issue**: Tolerance of 0.005f vs. supportThreshold = 0.00001f creates inconsistency.

**Suggestion**: Define tolerance as class constant:
```csharp
private const float SIMPLEX_TOLERANCE = 0.005f;
public override bool Equals(object obj) => 
	obj is SimplexPt b && MinoMath.VApproximately(pt, b.pt, SIMPLEX_TOLERANCE) 
					   && MinoMath.VApproximately(dir, b.dir, SIMPLEX_TOLERANCE);
```

---

## Test Cases for Validation

To verify fixes, test with:

1. **Interior Point Test**: Point clearly inside triangle should return planePt
   - Create triangle in XY plane, test point above center
   - Expected: GetClosestPoint returns the plane projection

2. **Edge Point Test**: Point on triangle edge should return edge point
   - Create triangle, project point onto edge
   - Expected: Correct edge position returned
   
3. **Vertex Point Test**: Point at/near vertex should return vertex
   - Create triangle, test at v1, v2, v3
   - Expected: Corresponding vertex returned

4. **Tetrahedron Orientation Test**: Verify all face normals point outward
   - Create simple tetrahedron
   - Check each face normal dot product with (origin - faceCenter)
   - Expected: All positive (normals point away from origin inside tetrahedron)

---

## File-by-File Fixes Needed

| Line Range | Method | Fix Type | Urgency |
|-----------|--------|----------|---------|
| 81-89 | AddPoint/RemovePoint | Add missing implementation | CRITICAL |
| 389 | GetClosestPoint (inside check) | Fix boundary condition | CRITICAL |
| 399-423 | GetClosestPoint (edge logic) | Fix edge conditions | MAJOR |
| 663 | Lin() | Clarify/simplify | MAJOR |
| 912-940 | GetPointOfContact | Refactor edge normal logic | MODERATE |
| 35 | SimplexPt.Equals | Add constant tolerance | MINOR |

