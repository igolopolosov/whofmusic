namespace WarehouseOfMusic.Converters
{
    /// <summary>
    /// Provides methods to calculation time periods
    /// </summary>
    public static class TimePeriodsConverter
    {
        /// <summary>
        /// 1 second= 1000 milliseconds
        /// </summary>
        private const int Second = 1000;

        /// <summary>
        /// 1 minute = 60 seconds
        /// </summary>
        private const int Minute = 60 * Second;
        
        /// <summary>
        /// Convert temp to step period
        /// </summary>
        public static int GetStepPeriod(int tempo)
        {
            return Minute / (tempo * 4);
        }
    }
}