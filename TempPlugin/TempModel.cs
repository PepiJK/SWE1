using System;

namespace TempPlugin
{
    /// <summary>
    /// TempModel class that represents the table in the database.
    /// </summary>
    public class TempModel
    {
        /// <summary>
        /// Unique id of the entity.
        /// </summary>
        public int? Id { get; set; }
        
        /// <summary>
        /// DateTime when the temperature has been measured.
        /// </summary>
        public DateTime DateTime { get; set; }
        
        /// <summary>
        /// Value of the temperature in Celsius.
        /// </summary>
        public float Value { get; set; }
    }
}