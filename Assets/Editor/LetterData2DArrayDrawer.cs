using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LetterData2DArrayDrawer : OdinAttributeDrawer<TableMatrixAttribute, LetterData[,]>
{
    private const float CELL_SIZE = 20f;
    private const float CELL_SPACING = 2f;

    protected override void DrawPropertyLayout(GUIContent label)
    {
        var property = this.Property;
        var array = (LetterData[,])property.ValueEntry.WeakSmartValue;

        if (array == null)
        {
            EditorGUILayout.HelpBox("Array is null.", MessageType.Warning);
            return;
        }

        int width = array.GetLength(0);
        int height = array.GetLength(1);

        if (label != null)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        float totalWidth = width * CELL_SIZE + (width - 1) * CELL_SPACING;
        float totalHeight = height * CELL_SIZE + (height - 1) * CELL_SPACING;

        Rect gridRect = EditorGUILayout.GetControlRect(false, totalHeight + 20);

        EditorGUI.DrawRect(new Rect(gridRect.x - 5, gridRect.y - 5, totalWidth + 10, totalHeight + 10), new Color(0.2f, 0.2f, 0.2f, 0.3f));
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Rect cellRect = new Rect(
                    gridRect.x + x * (CELL_SIZE + CELL_SPACING),
                    gridRect.y + y * (CELL_SIZE + CELL_SPACING),
                    CELL_SIZE,
                    CELL_SIZE
                );

                // Draw the letter data in the cell
                LetterData letterData = array[x, y];
                DrawLetterCell(cellRect, letterData, x, y);
            }
        }

        GUILayout.Space(10);
    }

    private void DrawLetterCell(Rect rect, LetterData letter, int x, int y)
    {
        Color bgColor = GetCellBGColor(letter);
        EditorGUI.DrawRect(rect, bgColor);

        Handles.color = Color.black;

        Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.black);

        GUIStyle textStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            normal = { textColor = GetTextColor(letter) }
        };

        string displayText = string.IsNullOrEmpty(letter.name) ? "" : letter.name;

        Rect letterRect = new Rect(rect.x, rect.y, rect.width, rect.height * 0.7f);
        Rect valueRect = new Rect(rect.x, rect.y + rect.height * 0.7f, rect.width, rect.height * 0.3f);

        EditorGUI.LabelField(letterRect, displayText, textStyle);

        if (letter.baseValue > 0)
        {
            GUIStyle valueStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                normal = { textColor = Color.grey }
            };

            EditorGUI.LabelField(valueRect, letter.baseValue.ToString(), valueStyle);
        }

        string tooltip = $"Position: ({x}, {y})\n" +
                         $"Name: {letter.name}\n" +
                         $"Value: {letter.baseValue}";

        GUI.tooltip = tooltip;
    }

    private Color GetCellBGColor(LetterData letter)
    {
        if (string.IsNullOrEmpty(letter.name))
        {
            return new Color(0.8f, 0.8f, 0.8f, 0.5f); // Light gray for empty cells
        }
        float normalizedValue = Mathf.Clamp01(letter.baseValue / 10f); // Assuming max value is 10
        return Color.Lerp(Color.white, Color.yellow, normalizedValue); // Gradient from white to yellow based on value
    }

    private Color GetTextColor(LetterData letter)
    {
        if (string.IsNullOrEmpty(letter.name))
        {
            return Color.gray; // Gray for empty cells
        }
        return Color.black; // Default text color for filled cells
    }
}