using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[System.Serializable]
public class BoardState
{
    [OdinSerialize, TableMatrix(HorizontalTitle = "Game Board", SquareCells = true)]
    public LetterData[,] tiles;

    public BoardState(int width, int height)
    {
        tiles = new LetterData[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                tiles[i, j] = new LetterData(string.Empty, 0, null);
            }
        }
    }

//#if UNITY_EDITOR
//    private static LetterData DrawTile(Rect rect, LetterData value)
//    {
//        Color bgColor = string.IsNullOrEmpty(value.name) ? new Color(0.8f, 0.8f, 0.8f, 0.3f) : new Color(0.9f, 0.95f, 1f, 0.8f);
//        EditorGUI.DrawRect(rect, bgColor);

//        EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), Color.black);
//        EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), Color.black);
//        EditorGUI.DrawRect(new Rect(rect.x + rect.width - 1, rect.y, 1, rect.height), Color.black);

//        Rect contentRect = new Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height - 4);

//        float letterHeight = contentRect.height * 0.6f;
//        float valueHeight = contentRect.height * 0.4f;

//        Rect letterRect = new Rect(contentRect.x, contentRect.y, contentRect.width, letterHeight);
//        Rect valueRect = new Rect(contentRect.x, contentRect.y + letterHeight, contentRect.width, valueHeight);

//        GUIStyle letterStyle = new GUIStyle(GUI.skin.label)
//        {
//            alignment = TextAnchor.MiddleCenter,
//            fontSize = 14,
//            fontStyle = FontStyle.Bold,
//            normal = { textColor = string.IsNullOrEmpty(value.name) ? Color.grey : Color.black }
//        };

//        string displayLetter = string.IsNullOrEmpty(value.name) ? "" : value.name.ToUpper();
//        GUI.Label(letterRect, displayLetter, letterStyle);

//        if (value.baseValue > 0)
//        {
//            GUIStyle valueStyle = new GUIStyle(GUI.skin.label)
//            {
//                alignment = TextAnchor.MiddleCenter,
//                fontSize = 9,
//                normal = { textColor = Color.grey }
//            };
//        }

//        //if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
//        //{
//        //    var window = EditorWindow.GetWindow<LetterDataEditorWindow>();
            
//        //}

//        return value;
//    }
//#endif

    public void Clear()
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int j = 0; j < tiles.GetLength(1); j++)
            {
                tiles[i, j] = new LetterData(string.Empty, 0, null);
            }
        }
    }
}

public class Board : SerializedMonoBehaviour
{
    //[SerializeField, InlineProperty, HideLabel]
    [NonSerialized, OdinSerialize]
    public BoardState state = new BoardState(15, 15);

    private void Awake()
    {
        state = new BoardState(15, 15);
        Debug.Log(state.tiles.GetLength(0) + "x" + state.tiles.GetLength(1) + " board initialized.");
    }

    [Button("Init Board")]
    public void Initialize(GameRules rules)
    {
        for (int x = 0; x < state.tiles.GetLength(0); x++)
        {
            for (int y = 0; y < state.tiles.GetLength(1); y++)
            {
                state.tiles[x, y] = new LetterData(string.Empty, 0, null);
                GameObject tile = Instantiate(rules.tilePrefab, transform);
                tile.transform.localPosition = new Vector3(x * 0.5f, 0, y * 0.5f);
            }
        }
    }

    void Draw()
    {
        // STUB
    }

    void Clear()
    {
        state.Clear();
        Draw();
    }
}

