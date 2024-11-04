namespace ARKitect.Geometry
{
    public static class SmoothingGroup
    {
        /// <summary>
        /// Faces with smoothingGroup = 0 are hard edges. Historically negative values were sometimes also written as hard edges.
        /// </summary>
        internal const int smoothingGroupNone = 0;

        /// <summary>
        /// Smoothing groups 1-24 are smooth.
        /// </summary>
        internal const int smoothRangeMin = 1;

        /// <summary>
        /// Smoothing groups 1-24 are smooth.
        /// </summary>
        internal const int smoothRangeMax = 24;

        /// <summary>
        /// Smoothing groups 25-42 are hard. Note that this is obsolete, and generally hard faces should be marked smoothingGroupNone.
        /// </summary>
        internal const int hardRangeMin = 25;

        /// <summary>
        /// Smoothing groups 25-42 are hard. Note that this is soon to be obsolete, and generally hard faces should be marked smoothingGroupNone.
        /// </summary>
        internal const int hardRangeMax = 42;
    }

}
