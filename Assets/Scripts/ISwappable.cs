/// <summary>
/// Interface for types that need to be notified when swapped between GameZones.
/// Enables automatic owner tracking and other swap-related behavior.
/// </summary>
/// <typeparam name="T">The type of item being swapped (typically the implementing type itself)</typeparam>
public interface ISwappable<T>
{
    /// <summary>
    /// Called when this item is swapped to a new GameZone.
    /// Use this to update ownership, references, or perform other swap-related logic.
    /// </summary>
    /// <param name="newZone">The GameZone this item was swapped into</param>
    void OnSwappedTo(GameZone<T> newZone);
}
