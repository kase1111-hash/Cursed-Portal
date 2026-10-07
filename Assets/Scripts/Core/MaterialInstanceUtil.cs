using UnityEngine;

/// <summary>
/// Helpers for scripts that animate material properties without editing shared material assets.
/// </summary>
public static class MaterialInstanceUtil
{
    /// <summary>
    /// Replaces <paramref name="shared"/> with one private copy in every renderer slot (on the owner and its
    /// children) that uses it, so animating the copy shows on screen without changing the asset.
    /// Returns the copy, or null when no renderer uses the material.
    /// </summary>
    public static Material InstantiateWhereUsed(Component owner, Material shared)
    {
        if (owner == null || shared == null) return null;

        Material copy = null;
        foreach (Renderer rend in owner.GetComponentsInChildren<Renderer>(true))
        {
            Material[] materials = rend.sharedMaterials;
            bool changed = false;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == shared)
                {
                    if (copy == null)
                    {
                        copy = new Material(shared);
                    }
                    materials[i] = copy;
                    changed = true;
                }
            }
            if (changed)
            {
                rend.sharedMaterials = materials;
            }
        }
        return copy;
    }
}
