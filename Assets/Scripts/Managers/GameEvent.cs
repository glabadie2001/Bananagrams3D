using UnityEngine;

public abstract class GameEvent
{
    public abstract void Run();
}

public class DragStartEvent : GameEvent
{
    private readonly IDraggable dragTarget;
    private readonly float dragSpeed;
    
    public DragStartEvent(IDraggable target, float speed)
    {
        dragTarget = target;
        dragSpeed = speed;
    }
    public override void Run()
    {
        dragTarget.SaveOriginalPosition();
        GameManager.Inst.StartCoroutine(dragTarget.Drag(dragSpeed));
    }
}

public class DragEndEvent : GameEvent
{
    private readonly IDraggable dragTarget;
    
    public DragEndEvent(IDraggable target)
    {
        dragTarget = target;
    }

    public override void Run()
    {
        
    }
}