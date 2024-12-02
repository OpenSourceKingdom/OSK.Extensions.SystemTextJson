using System.Text.Json;

namespace OSK.Extensions.SystemTextJson.Common
{
    public static class Utf8JsonReaderExtensions
    {
        /// <summary>
        /// Attempts to find a property that matches the provided property name and returns the associated value
        /// </summary>
        /// <param name="reader">The reader to read the json data from</param>
        /// <param name="propertyName">The property name to seek</param>
        /// <param name="copiedReader">The copied Utf8JsonReader at the point where the property was found, if it exists.</param>
        /// <returns>True if the property is found, false if not.</returns>
        /// <remarks>
        /// Note 1: The reader that is used is not directly consumed as a copy is made prior to seek for the property.
        /// This allows deserialization to continue as normal.
        /// <br/><br/>
        /// Note 2: The returned reader will be at the point of the property value for the consumer to read, if found.
        /// </remarks>
        public static bool TryFindPropertyValue(this Utf8JsonReader reader, string propertyName,
            out Utf8JsonReader copiedReader)
        {
            // Copy original reader to prevent undesired token consumption
            copiedReader = reader;
            if (copiedReader.TokenType != JsonTokenType.StartObject)
            {
                return false;
            }

            var startingDepth = copiedReader.CurrentDepth;
            while (copiedReader.TokenType != JsonTokenType.EndObject || copiedReader.CurrentDepth != startingDepth)
            {
                copiedReader.Read();
                switch (copiedReader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        var currentPropertyName = copiedReader.GetString();
                        if (currentPropertyName == propertyName)
                        {
                            // Let the consumer read the current property value as they desire
                            copiedReader.Read();
                            return true;
                        }
                        break;
                }

                copiedReader.Skip();
            }

            return false;
        }
    }
}
