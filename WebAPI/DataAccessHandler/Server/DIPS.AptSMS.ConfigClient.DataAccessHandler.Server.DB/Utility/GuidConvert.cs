using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DIPS.AptSMS.ConfigClient.DataAccessHandler.Server.DB.Utility
{
    public static class GuidConvert
    {
        /// <summary>
        /// Converts guid to proper string format for storing in db.
        /// </summary>
        /// <param name="value">Guid to convert.</param>
        /// <returns>32 character string representing guid. All characters are uppercase to ensure case safety.</returns>
        [SuppressMessage("DIPS Quality", "DIPS004:AvoidGuidToStringRule",
            Justification = "The implementation of ToDBString must use Guid.ToString so no one else needs to.")]
        public static string ToDBString(Guid value)
        {
            return value.ToString("N").ToUpperInvariant();
        }

        /// <summary>
        /// Converts guid from string format used in db.
        /// </summary>
        /// <param name="value">String to convert. This must be a 32 character representation of a Guid.
        /// </param>
        /// <returns>
        /// Guid converted from string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If value is null or empty this exception is raised.
        /// </exception>
        public static Guid FromDBString(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("value");
            }

            return new Guid(value);
        }

        /// <summary>
        /// Converts guid to Raw format for storage in db.
        /// </summary>
        /// <param name="value">Guid to convert.</param>
        /// <returns>Raw representation of Guid.</returns>
        public static byte[] ToRaw(Guid value)
        {
            return value.ToByteArray();
        }

        /// <summary>
        /// Converts nullable guid to Raw format for storage in db.
        /// </summary>
        /// <param name="value">Guid to convert.</param>
        /// <returns>If value is not null this method returns a raw representation 
        /// of the value; otherwise an empty byte array is returned.</returns>
        public static byte[] ToRaw(Guid? value)
        {
            if (value == null)
            {
                return new byte[0];
            }

            return ToRaw(value.Value);
        }

        /// <summary>
        /// Converts to guid from raw stored in db.
        /// </summary>
        /// <param name="value">Raw to convert to guid.</param>
        /// <returns>Guid converted from raw.</returns>
        /// <remarks>
        /// Raw in db is represented as byte[]. This method only accepts
        /// a byte[] with length 16 elements.
        /// </remarks>
        public static Guid FromRaw(byte[] value)
        {
            return new Guid(value);
        }

        /// <summary>
        /// Converts to guid from raw stored in db. The input of this 
        /// method must be a byte array. Using this method you do not need to 
        /// cast to byte[] befor converting.
        /// </summary>
        /// <param name="value">Raw to convert to Guid. Must be byte[].</param>
        /// <returns>Guid converted from raw.</returns>
        /// <remarks>
        /// Raw in db is represented as byte[]. This method only accepts
        /// a byte[] with length 16 elements.
        /// </remarks>
        public static Guid FromRaw(object value)
        {
            return FromRaw((byte[])value);
        }

        /// <summary>
        /// Converts to guid from raw stored in db. Accepts empty input.
        /// </summary>
        /// <param name="value">Raw to convert to guid. Null and empty array is also allowed.</param>
        /// <returns>
        /// If value is not null or empty Guid converted from raw is returned; otherwise null.
        /// </returns>
        public static Guid? FromNullableRaw(byte[] value)
        {
            if (value != null && value.Length != 0)
            {
                return FromRaw(value);
            }

            return null;
        }

        /// <summary>
        /// Converts to guid from raw stored in db. Accepts empty input. The input of this 
        /// method must be a byte array. Using this method you do not need to 
        /// cast to byte[] befor converting.
        /// </summary>
        /// <param name="value">Raw to convert to guid. Null and empty array is also allowed.</param>
        /// <returns>
        /// If value is not null or empty Guid converted from raw is returned; otherwise null.
        /// </returns>
        public static Guid? FromNullableRaw(object value)
        {
            return FromNullableRaw((byte[])value);
        }

        private static readonly Regex s_guidRegex = new Regex(
                    "^[A-Fa-f0-9]{32}$|" +
                    "^({|\\()?[A-Fa-f0-9]{8}-([A-Fa-f0-9]{4}-){3}[A-Fa-f0-9]{12}(}|\\))?$|" +
                    "^({)?[0xA-Fa-f0-9]{3,10}(, {0,1}[0xA-Fa-f0-9]{3,6}){2}, {0,1}({)([0xA-Fa-f0-9]{3,4}, {0,1}){7}[0xA-Fa-f0-9]{3,4}(}})$",
                    RegexOptions.Compiled);

        /// <summary>
        /// Tries to convert string to Guid. 
        /// </summary>
        /// <param name="guid">The string representation to convert to guid.</param>
        /// <param name="result">The convertionresult is stored in this parameter.</param>
        /// <returns>true if convertion is successfull; false otherwise.</returns>
        public static bool TryParse(string guid, out Guid result)
        {
            result = Guid.Empty;

            if (String.IsNullOrEmpty(guid))
            {
                return false;
            }

            Match match = s_guidRegex.Match(guid);

            if (match.Success)
            {
                result = new Guid(guid);
            }
            else
            {
                result = Guid.Empty;
            }

            return match.Success;
        }

        public static string GuidToDBString(Guid guid)
        {
            string vguid = guid.ToString();
            vguid = vguid.Replace("-", string.Empty);
            vguid = vguid.Replace("{", string.Empty);
            vguid = vguid.Replace("}", string.Empty);
            vguid = vguid.Replace("(", string.Empty);
            vguid = vguid.Replace(")", string.Empty);

            vguid = vguid.Substring(6, 2) + vguid.Substring(4, 2) + vguid.Substring(2, 2) + vguid.Substring(0, 2) + vguid.Substring(10, 2) + vguid.Substring(8, 2) + vguid.Substring(14, 2) +
                   vguid.Substring(12, 2) + vguid.Substring(16, 4) + vguid.Substring(20, 12);

            return vguid;
        }
    }
}
