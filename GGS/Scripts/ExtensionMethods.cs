using Microsoft.AspNetCore.Html;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GGS.Scripts
{
    public delegate K CallbackFn<T, K>(T value);

    public class SelectInnerContent
    {
        public SelectOptionGroup[] Groups { get; set; } = new SelectOptionGroup[0];
        public SelectOption[] OrphanOptions { get; set; } = new SelectOption[0];
    }

    public class SelectOptionGroup
    {
        public SelectOption[] Options { get; set; } = new SelectOption[0];
        public string Label { get; set; } = "";
    }

    public class SelectOption
    {
        public string Value { get; set; } = "";
        public string Description { get; set; } = "";
        public bool Selected { get; set; } = false;
        public bool Disabled { get; set; } = false;
    }

    public static class ExtensionMethods
    {
        public static SelectInnerContent ToSelectInnerContent<T>
        (
            this IEnumerable<T> enumerable,
            CallbackFn<T, SelectOption> valueToOptionFn,
            CallbackFn<T, string> valueToGroupLabelFn
        ) 
        {
            T[] enumerableAsArray = enumerable.ToArray();

            var groups = enumerableAsArray.GroupBy(value => valueToGroupLabelFn(value));

            var orphanOptions = new List<SelectOption>();
            var optGroups = new List<SelectOptionGroup>();

            foreach (var group in groups)
            {
                var options = new List<SelectOption>();

                foreach (var enumerableValue in group)
                {
                    options.Add(valueToOptionFn(enumerableValue));
                }

                if (string.IsNullOrEmpty(group.Key))
                {
                    // Significa que estas son las <option> que van solas
                    orphanOptions = options;
                }
                else
                {
                    // Significa que es un <optgroup>
                    var optGroup = new SelectOptionGroup()
                    {
                        Label = group.Key,
                        Options = options.ToArray()
                    };

                    optGroups.Add(optGroup);
                }
            }

            var dropdown = new SelectInnerContent()
            {
                Groups = optGroups.ToArray(),
                OrphanOptions = orphanOptions.ToArray()
            };

            return dropdown;
        }

        public static SelectInnerContent ToSelectInnerContent<T>
        (
           this IEnumerable<T> enumerable,
           CallbackFn<T, SelectOption> valueToOptionFn
        )
        {
            return enumerable.ToSelectInnerContent(valueToOptionFn, value => "");
        }

        public static HtmlString ToSelectInnerContentHtmlString<T>
        (
            this IEnumerable<T> enumerable,
            CallbackFn<T, SelectOption> valueToOptionFn,
            CallbackFn<T, string> valueToKeyFn
        )
        {
            var dropdown = enumerable.ToSelectInnerContent(valueToOptionFn, valueToKeyFn);
            string selectString = $"<option value=\"\"></option>";

            foreach (var optGroup in dropdown.Groups)
            {
                selectString += $"<optgroup label=\"{optGroup.Label}\">";

                foreach (var option in optGroup.Options)
                {
                    selectString += $"<option value=\"{option.Value}\"{(option.Selected ? " selected" : "")}{(option.Disabled ? " disabled" : "")}>{option.Description}</option>";
                }

                selectString += "</optgroup>";
            }

            foreach (var option in dropdown.OrphanOptions)
            {
                selectString += $"<option value=\"{option.Value}\"{(option.Selected ? " selected" : "")}{(option.Disabled ? " disabled" : "")}>{option.Description}</option>";
            }

            return new HtmlString(selectString);
        }

        public static HtmlString ToSelectInnerContentHtmlString<T>
        (
            this IEnumerable<T> enumerable,
            CallbackFn<T, SelectOption> valueToOptionFn
        )
        {
            return enumerable.ToSelectInnerContentHtmlString(
                valueToOptionFn,
                value => ""
            );
        }

        /// <summary>
        /// Verifica que el string dado siga el formato de "yyyy-MM-dd"
        /// </summary>
        /// <param name="dateAsString"></param>
        /// <returns></returns>
        public static bool IsValidInputDate(this string dateAsString)
        {
            var inputDateRegex = new Regex(@"^\d{4}-\d{2}-\d{2}$");
            return inputDateRegex.IsMatch(dateAsString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToInputString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Recibe un string proveniente de los query params, si el string es null, regresa la fecha de hoy
        /// sumandole los días proporcionados en el parámetro daysToAdd,
        /// si el string existe, verifica si se encuentra en formato "yyyy-MM-dd", en caso de ser válido,
        /// regresa el string, en caso de no serlo, regresa un string vacío
        /// </summary>
        /// <param name="dateAsString">Fecha que provinene del query param</param>
        /// <param name="daysToAdd">Días a agregar a la fecha</param>
        /// <returns></returns>
        public static string ToDateFilterInput(this string? dateAsString, int daysToAdd)
        {
            if (string.IsNullOrEmpty(dateAsString)) return DateTime.Now.AddDays(daysToAdd).ToInputString();
            if (!dateAsString.IsValidInputDate()) return "";
            return dateAsString;
        }

        public static DateTime? ToDateTime(this string? dateAsString)
        {
            if (string.IsNullOrEmpty(dateAsString)) return null;
            if (!dateAsString.IsValidInputDate()) return null;
            return DateTime.ParseExact(dateAsString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static string ToListFormat(this IEnumerable<string> enumerable, string unionString, bool useOxfordComma = true)
        {
            var enumerableAsArray = enumerable.ToArray();
            if (enumerableAsArray == null || enumerableAsArray.Length == 0) return string.Empty;          
            if (enumerableAsArray.Length == 1) return enumerableAsArray[0];

            string resultado = string.Join(", ", new ArraySegment<string>(enumerableAsArray, 0, enumerableAsArray.Length - 1));
            resultado += $"{(useOxfordComma ? "," : "")} {unionString} {enumerableAsArray[enumerableAsArray.Length - 1]}";

            return resultado;
        }

        public static IEnumerable<int> Sort(this IEnumerable<int> enumerable)
        {
            return enumerable.OrderBy(x => x);
        }

        public static string ToCapitalizedCase(this string text)
        {
            var words = text.Split(' ');
            var capitalizedWords = words.Select(word => word[0].ToString().ToUpper() + word.Substring(1));
            return string.Join(' ', capitalizedWords);
        }

        public static string ToMedicalString(this DateTime date)
        {
            var na = "N/A";
            if (date.Ticks == 0) return na;

            CultureInfo culture = new CultureInfo("es-MX");

            string mmmMonth = date.ToString("MMM", culture);
            string fixedMonth = mmmMonth.ToCapitalizedCase().Substring(0, mmmMonth.Length - 1);

            string formattedDate = date.ToString("MMM-dd-yyyy", culture).Replace(mmmMonth, fixedMonth);

            return formattedDate;
        }
    }
}
