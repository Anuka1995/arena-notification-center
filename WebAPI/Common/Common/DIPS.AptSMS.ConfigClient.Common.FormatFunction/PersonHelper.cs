using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DIPS.AptSMS.ConfigClient.Common.FormatFunction
{
    public static class PersonHelper
    {
        public static string AddressFix(string streetAddress)
        {
            if (String.IsNullOrEmpty(streetAddress))
            {
                return streetAddress;
            }

            string formattedStreetAddress = ToTitleCaseForCurrentCulture(streetAddress);

            formattedStreetAddress = FixNameToLower("Gate", formattedStreetAddress);

            formattedStreetAddress = FixNameToLower("Gt", formattedStreetAddress);

            formattedStreetAddress = FixNameToLower("Vei", formattedStreetAddress);

            formattedStreetAddress = FixNameToLower("Veien", formattedStreetAddress);

            formattedStreetAddress = FixNameToLower("Veg", formattedStreetAddress);

            formattedStreetAddress = FixNameToLower("Vegen", formattedStreetAddress);

            formattedStreetAddress = FixNameToLower("Og", formattedStreetAddress);

            formattedStreetAddress = FixNameToLower("Ved", formattedStreetAddress);

            formattedStreetAddress = FixNameToLower("I", formattedStreetAddress);

            return formattedStreetAddress;
        }

        public static string NameFix(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return name;
            }

            string formattedName = ToTitleCaseForCurrentCulture(name);

            formattedName = FixNameToLower("Af", formattedName);

            formattedName = FixNameToLower("Der", formattedName);

            formattedName = FixNameToLower("La", formattedName);

            formattedName = FixNameToLower("Ten", formattedName);

            formattedName = FixNameToLower("Van", formattedName);

            formattedName = FixNameToLower("Von", formattedName);

            formattedName = FixCasingForSpecialPrefix("Mc", formattedName);

            formattedName = FixCasingForSpecialPrefix("Mac", formattedName);

            formattedName = SetCaseOfLettersNextToQuoteCharactersInName(formattedName);

            return formattedName;
        }

        private static string SetCaseOfLettersNextToQuoteCharactersInName(string name)
        {
            string returnValue = SetCaseOfLettersNextToBackQuoteIfPresent(name);

            returnValue = SetCaseOfLettersNextToForwardQuoteIfPresent(returnValue);

            returnValue = SetCaseOfLettersNextToSingleQuoteIfPresent(returnValue);

            return returnValue;
        }

        private static string SetCaseOfLettersNextToSingleQuoteIfPresent(string name)
        {
            char singleQuote = '\'';
            if (name.Contains(singleQuote.ToString()))
            {
                return SetCasingInNameBasedOnQuoteCharaterType(singleQuote, name);
            }

            return name;
        }

        private static string SetCaseOfLettersNextToForwardQuoteIfPresent(string name)
        {
            char forwardQuote = '´';
            if (name.Contains(forwardQuote.ToString()))
            {
                return SetCasingInNameBasedOnQuoteCharaterType(forwardQuote, name);
            }

            return name;
        }

        private static string SetCaseOfLettersNextToBackQuoteIfPresent(string name)
        {
            char backQuote = '`';
            if (name.Contains(backQuote.ToString()))
            {
                return SetCasingInNameBasedOnQuoteCharaterType(backQuote, name);
            }

            return name;
        }

        private static string SetCasingInNameBasedOnQuoteCharaterType(char quoteCharacter, string name)
        {
            char[] charactersInName = name.ToCharArray();

            for (int indexOfCurrent = 0; indexOfCurrent < charactersInName.Length; indexOfCurrent++)
            {
                var currentCharacter = charactersInName[indexOfCurrent];

                if (currentCharacter == quoteCharacter)
                {
                    FormatCasingAroundQuoteCharacter(indexOfCurrent, charactersInName);
                }
            }

            return new string(charactersInName);
        }

        private static void FormatCasingAroundQuoteCharacter(int indexOfCurrent, char[] charactersInName)
        {
            if (QuoteCharacterIsAtBeginningOfName(indexOfCurrent))
            {
                SetUppercaseToCharacterAfterAndLowercaseToCharacterBeforeQuote(indexOfCurrent, charactersInName);
            }
            else
            {
                if (QuoteCharacterHasIndexThatRequiresSpecialFormatting(indexOfCurrent, charactersInName))
                {
                    SetUppercaseToCharacterAfterAndLowercaseToCharacterBeforeQuote(indexOfCurrent, charactersInName);
                }
                else
                {
                    SetLowercaseToCharacterAfterQuote(indexOfCurrent, charactersInName);
                }
            }
        }

        private static void SetLowercaseToCharacterAfterQuote(int indexOfQuoteCharacter, char[] charactersInName)
        {
            if (indexOfQuoteCharacter < charactersInName.Length - 1)
            {
                charactersInName[indexOfQuoteCharacter + 1] = char.ToLower(charactersInName[indexOfQuoteCharacter + 1]);
            }
        }

        private static void SetUppercaseToCharacterAfterAndLowercaseToCharacterBeforeQuote(int indexOfQuoteCharacter, char[] charactersInName)
        {
            charactersInName[indexOfQuoteCharacter - 1] = char.ToLower(charactersInName[indexOfQuoteCharacter - 1]);
            if (indexOfQuoteCharacter < charactersInName.Length - 1)
            {
                charactersInName[indexOfQuoteCharacter + 1] = char.ToUpper(charactersInName[indexOfQuoteCharacter + 1]);
            }
        }

        private static bool QuoteCharacterHasIndexThatRequiresSpecialFormatting(int indexOfQuoteCharacter, char[] charactersInName)
        {
            if (indexOfQuoteCharacter > 2)
            {
                char characterToCompare = charactersInName[indexOfQuoteCharacter - 2];
                if (characterToCompare == ' ' || characterToCompare == '-' || characterToCompare == '.')
                {
                    return true;
                }
            }

            return false;
        }

        private static bool QuoteCharacterIsAtBeginningOfName(int indexOfCurrent)
        {
            return indexOfCurrent == 1;
        }

        private static string ToTitleCaseForCurrentCulture(string name)
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
        }

        private static string FixNameToLower(string replacementWord, string formattedName)
        {
            string pattern = @"\b" + replacementWord + @"\b+";
            var rgx = new Regex(pattern);
            formattedName = rgx.Replace(formattedName, replacementWord.ToLower());
            return formattedName;
        }

        private static string FixCasingForSpecialPrefix(string prefix, string nameToBeFormatted)
        {
            string[] splitNames = nameToBeFormatted.Split(' ');

            SetLetterAfterPrefixToUpper(splitNames, prefix);

            return RecreateSplittedNamesFrom(splitNames);
        }

        private static string RecreateSplittedNamesFrom(string[] splitName)
        {
            string output = string.Empty;

            if (splitName.Length == 1)
            {
                output = splitName[0];
            }
            else
            {
                output = PieceTogheterSplittedNamesWithSpaces(splitName, output);
            }

            return output;
        }

        private static string PieceTogheterSplittedNamesWithSpaces(string[] splitName, string output)
        {
            for (int i = 0; i < splitName.Length; i++)
            {
                string name = splitName[i];
                if (i == splitName.Length - 1)
                {
                    output += name;
                }
                else
                {
                    output += name + " ";
                }
            }

            return output;
        }

        private static void SetLetterAfterPrefixToUpper(string[] words, string prefix)
        {
            for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
            {
                string singleWord = words[wordIndex];
                if (singleWord.StartsWith(prefix))
                {
                    words[wordIndex] = SetLetterAfterPrefixToUpper(singleWord, prefix);
                }
            }
        }

        private static string SetLetterAfterPrefixToUpper(string singleWord, string prefix)
        {
            if (singleWord.Length > prefix.Length)
            {
                char[] charachters = singleWord.ToCharArray();
                charachters[prefix.Length] = Char.ToUpper(charachters[prefix.Length]);

                return new string(charachters);
            }

            return singleWord;
        }

        public static bool IsDead(short? mors)
        {
            if (mors == null)
            {
                return false;
            }

            return mors != 0;
        }

        public static short? MorsToShort(bool mors)
        {
            if (mors)
            {
                return 1;
            }

            return 0;
        }
    }
}