using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;

    [SerializeField]
    GameRules rules;

    [SerializeField]
    Board board;

    [SerializeField]
    Bag<LetterData> reserve;
    [SerializeField]
    List<LetterData> discard;
    [SerializeField]
    List<LetterData> hand;
    public int handSize = 21;

    void Awake()
    {
        if (inst == null)
            inst = this;
        else if (inst != this)
            Destroy(this);

        reserve = new Bag<LetterData>();
        discard = new List<LetterData>();
        hand = new List<LetterData>();
    }

    void Start()
    {
        // TODO: Simple assignment rather than copy?
        foreach (var letter in rules.GetExpandedBagContents())
        {
            reserve.Add(letter);
        }

        board.Initialize(rules);
    }

    [Button("Draw Hand")]
    void DrawHand()
    {
        for (int i = hand.Count; i < handSize; i++)
        {
            if (reserve.Count == 0)
            {
                Debug.LogWarning("No more letters in reserve to draw from.");
                break;
            }
            hand.Add(reserve.Pull());
        }
    }

    [Button("Discard Hand")]
    void DiscardHand()
    {
        while(hand.Count > 0)
        {
            discard.Add(hand[0]);
            hand.RemoveAt(0);
        }
    }
}
