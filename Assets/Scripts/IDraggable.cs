using System.Collections;
using UnityEngine;

// TODO: Unnecessary?
public interface IDraggable
{ 
    /// <summary>
    /// Set where the draggable should go on cancel.
    /// </summary>
    public void SaveOriginalPosition();
    
    public void RestoreOriginalPosition();
    
    /// <summary>
    /// Mouse follow coroutine
    /// </summary>
    /// <returns>IEnumerator to be used with StartCoroutine()</returns>
    public IEnumerator Drag(float dragSpeed);
}