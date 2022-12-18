/// <summary>
/// Extention methods for the epub reader service
/// /// </summary>
public static class ExtentionsMethods
{
    /// <summary>
    /// Updates elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items"></param>
    /// <param name="updateMethod"></param>
    /// <returns></returns>
    public static IEnumerable<T> TransformElements<T>(this IEnumerable<T> items, Action<T> updateMethod)
    {
        foreach (T item in items)
        {
            updateMethod(item);
        }
        return items;
    }
}
