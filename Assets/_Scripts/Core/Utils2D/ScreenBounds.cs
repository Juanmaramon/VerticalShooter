namespace Core
{
    /// <summary>
    /// Screen bounds. Will clamp player movement inside screen
    /// </summary>
    [System.Serializable]
    public class ScreenBounds
    {
        public float xMin, xMax, yMin, yMax;
    }
}