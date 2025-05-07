
public interface ISortUIController
{
    /// <summary>
    /// Stops all coroutines and resets any state that shouldn’t persist
    /// when switching algorithms.
    /// </summary>
    void StopSorting();
}
