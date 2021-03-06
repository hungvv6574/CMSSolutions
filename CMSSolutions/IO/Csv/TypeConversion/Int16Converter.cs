﻿using System.Globalization;

namespace CMSSolutions.IO.Csv.TypeConversion
{
    /// <summary>
    /// Converts an Int16 to and from a string.
    /// </summary>
    public class Int16Converter : DefaultTypeConverter
    {
        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="culture">The culture used when converting.</param>
        /// <param name="text">The string to convert to an object.</param>
        /// <returns>The object created from the string.</returns>
        public override object ConvertFromString(CultureInfo culture, string text)
        {
            short s;
            if (short.TryParse(text, NumberStyles.Integer, culture, out s))
            {
                return s;
            }

            return base.ConvertFromString(culture, text);
        }

        /// <summary>
        /// Determines whether this instance [can convert from] the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can convert from] the specified type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvertFrom(System.Type type)
        {
            // We only care about strings.
            return type == typeof(string);
        }
    }
}