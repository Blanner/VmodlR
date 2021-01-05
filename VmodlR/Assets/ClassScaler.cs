using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassScaler : MonoBehaviour
{
    public BoxCollider classCollider;
    public UMLClass classConnections;



    /// <summary>
    /// The number of elements currently displayed on every class side.
    /// This is used to resize the Class mesh.
    /// </summary>
    private int displayedElementCount = 0;

    private Transform[] classSides;


    public void Initialize(ClassSide[] classSides)
    {
        this.classSides = new Transform[classSides.Length];
        for(int i = 0; i < classSides.Length; i++)
        {
            this.classSides[i] = classSides[i].transform;
        }
        UpdateSizing();
    }

    public void ElementAdded()
    {
        displayedElementCount++;
        UpdateSizing();
    }

    public void ElementDeleted()
    {
        displayedElementCount--;
        UpdateSizing();
    }

    private void UpdateSizing()
    {
        //TODO: Get maximum character count from field
        if (displayedElementCount <= 8)
        {
            //update class mesh scale
            transform.localScale = new Vector3(1, 1, 1);
            classCollider.size = transform.localScale;
            //update class Side canvas positions according to the new scale
            UpdateAllClassSidePositions();
            UpdateAllClassSideCanvasSizes();
            classConnections.UpdateAllConnections();
        }
        else
        {
            int additionalElementsCount = displayedElementCount - 8;
            //TODO: Scale x amount according to additionalElementsCount
            float baseScale = 1 + .085f * additionalElementsCount;
            transform.localScale = new Vector3(baseScale, baseScale, baseScale);
            classCollider.size = transform.localScale;
            //update class side canvas positions according to the new scale
            UpdateAllClassSidePositions();
            UpdateAllClassSideCanvasSizes();
            classConnections.UpdateAllConnections();
        }
    }

    private Vector3 CalculateBasePosition(Vector3 previousPosition)
    {
        //Debug.Log($"Prev Class Side pos: {previousPosition}");
        Vector3 basePosition = new Vector3();
        //round the value to -1 if negative, 0 if it is 0, or 1 of it is positive non-zero
        basePosition.x = (previousPosition.x < 0) ? -1 : Mathf.CeilToInt(Mathf.Clamp(previousPosition.x, 0, 1));
        basePosition.y = (previousPosition.y < 0) ? -1 : Mathf.CeilToInt(Mathf.Clamp(previousPosition.y, 0, 1));
        basePosition.z = (previousPosition.z < 0) ? -1 : Mathf.CeilToInt(Mathf.Clamp(previousPosition.z, 0, 1));
        //Debug.Log($"Base Class Side pos: {basePosition}");
        return basePosition;
    }

    private void UpdateAllClassSideCanvasSizes()
    {
        foreach (Transform classSide in classSides)
        {
            RectTransform canvasTransform = classSide.GetComponent<RectTransform>();
            //Transform is a cube, so all local Scales are equal.
            canvasTransform.sizeDelta = new Vector2(transform.localScale.x * 800, transform.localScale.x * 800);
        }
    }

    private void UpdateAllClassSidePositions()
    {
        foreach (Transform classSide in classSides)
        {
            Vector3 basePosition = CalculateBasePosition(classSide.localPosition);
            UpdateClassSidePosition(classSide, basePosition, transform.localScale);
        }
    }

    /// <summary>
    /// Updates a class side's positioning according to the updated scale of the class mesh
    /// </summary>
    private void UpdateClassSidePosition(Transform classSide, Vector3 basePosition, Vector3 updatedClassScale)
    {
        Vector3 updatedPos = new Vector3();
        updatedPos.x = basePosition.x * (updatedClassScale.x / 2 + 0.01f);
        updatedPos.y = basePosition.y * (updatedClassScale.y / 2 + 0.01f);
        updatedPos.z = basePosition.z * (updatedClassScale.z / 2 + 0.01f);
        classSide.localPosition = updatedPos;
    }
}
