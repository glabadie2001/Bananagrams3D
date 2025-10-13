using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// Central game coordinator that manages game state and coordinates between systems.
/// Handles tile placement validation, hand management, and state transitions.
/// TODO: Consider splitting into separate GameState and GameLogic classes for better SRP.
/// </summary>
public class EventManager : MonoBehaviour
{
    
    public static EventManager Inst;

    [SerializeField] private int eventsPerTick;
    [SerializeReference] private Queue<GameEvent> eventQueue = new Queue<GameEvent>();
    
    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(this);
    }

    private void Update()
    {
        for (int i = 0; i < eventsPerTick && eventQueue.Count > 0; i++)
        {
            eventQueue.Dequeue().Run();
        }
    }

    public void Enqueue(GameEvent e)
    {
        Debug.Log($"Enqueue: {e}");
        eventQueue.Enqueue(e);
    }
}
