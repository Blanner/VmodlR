using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassScaler : MonoBehaviour
{
    public BoxCollider classCollider;
    public UMLClass classConnections;

    /// <summary>
    /// The invisible input field that this class uses for measuring the pixel width each element takes up. 
    /// Its text field always contains the longest text among all elements currently in the class
    /// </summary>
    public InputField measurementInputField;

    private int maxLengthElementID = -1;//by default, this is the class name which has ID -1

    /// <summary>
    /// The number of elements currently displayed on every class side.
    /// This is used to resize the Class mesh.
    /// </summary>
    private int displayedElementCount = 0;

    private Transform[] classSides;
    private ClassSide exampleClasSide;

    public void Initialize(ClassSide[] classSides)
    {
        exampleClasSide = classSides[0];

        this.classSides = new Transform[classSides.Length];
        for(int i = 0; i < classSides.Length; i++)
        {
            this.classSides[i] = classSides[i].transform;
        }
        UpdateScaling();
    }

    public void ElementStringUpdated(string updatedElementText, int elementID)
    {

        //calculate the lengths of both the max and the new element text
        string currMaxText = measurementInputField.text;
        float currMaxLengthPx = measurementInputField.preferredWidth;

        measurementInputField.text = updatedElementText;
        float newTextLengthPx = measurementInputField.preferredWidth;

        if (currMaxLengthPx < newTextLengthPx)
        {
            //The new element is longer than the previous one, thus we update the longest element ID. The measurementInputField.text field was already changed to contain the new text
            maxLengthElementID = elementID;
            UpdateScaling();
        }
        else
        {
            if (elementID == maxLengthElementID)
            {
                //The max element is updated to be smaller than before, thus we have to check if it is still the longest element.
                RecalculateLongestElementText();
                UpdateScaling();
            }
            else
            {
                //revert the changed text, since the old text was longer than the new one. The max element ID stays unchanged since the old maxElement is still the longest element
                measurementInputField.text = currMaxText;
            }
        }
    }

    public void ElementAdded()
    {
        displayedElementCount++;
        UpdateScaling();
    }

    public void ElementDeleted(int elementID)
    {
        displayedElementCount--;

        if (elementID == maxLengthElementID)
        {
            RecalculateLongestElementText();
        }

        UpdateScaling();
    }

    private void UpdateScaling()
    {
        float elementCountScale = 1;
        float elementTextLengthScale = 1;

        //Calculate scale needed to fit all elements (by number)
        if (displayedElementCount <= 8)
        {
            //update class mesh scale
            elementCountScale = 1;
        }
        else
        {
            int additionalElementsCount = displayedElementCount - 8;
            //TODO: Scale x amount according to additionalElementsCount
            elementCountScale = 1 + .085f * additionalElementsCount;
        }

        //calculate scale needed to fit longest element text 
        float maxTextPixelWidth = measurementInputField.preferredWidth + 65 + 10;//65 is the Input field's right margin, 10 the left margin
        elementTextLengthScale = (Mathf.Ceil(maxTextPixelWidth / 25) * 25) / 800; // maxTextPixelWidth / 800 is the precise scale we would need. The rest rounds this value to 10-px steps

        //reset the elementLengthScale to 1 if the maximum scale needed is below 1 bacause we never want to scale a class smaller than 1
        if (elementTextLengthScale < 1)
            elementTextLengthScale = 1;


        //use the greater one of the two possible scale values as the final scale
        if (elementCountScale > elementTextLengthScale)
        {
            transform.localScale = new Vector3(elementCountScale, elementCountScale, elementCountScale);
        }
        else
        {
            transform.localScale = new Vector3(elementTextLengthScale, elementTextLengthScale, elementTextLengthScale);
        }

        classCollider.size = transform.localScale;
        //update class side canvas positions according to the new scale
        UpdateAllClassSidePositions();
        UpdateAllClassSideCanvasSizes();
        classConnections.UpdateAllConnections();
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

    private void RecalculateLongestElementText()
    {
        measurementInputField.text = GetLongestElementText(out int newLongestElementID);
        maxLengthElementID = newLongestElementID;
    }

    /// <summary>
    /// Finds the ID and text of the element in this class with the longest text length in pixels
    /// </summary>
    /// <param name="longestElementID">the found element ID</param>
    /// <returns>The text of the found element, that corresponds to the ID</returns>
    private string GetLongestElementText(out int longestElementID)
    {
        //go over all elements and return the longest one (by length in pixels)
        string longestElementText = "";
        longestElementID = -1;
        float longestElementPxLength = -1;//This will always be overwritten in the first loop, since all actual elements have a value with at least 0 length

        foreach (ClassSideElement element in exampleClasSide.GetAllElements())
        {
            measurementInputField.text = element.Value;
            float textLengthPx = measurementInputField.preferredWidth;

            if (textLengthPx > longestElementPxLength)
            {
                longestElementPxLength = textLengthPx;
                longestElementText = element.Value;
                longestElementID = element.ElementID;
            }
        }

        return longestElementText;
    }
}
