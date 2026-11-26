using System.Text;

namespace EgyptianId;

/// <summary>Formatting helpers for national ID numbers.</summary>
public static class NationalIdFormatter
{
    /// <summary>Hides all but the last <paramref name="visibleDigits"/> digits. Safe for logs and UI.</summary>
    public static string Mask(string? number, int visibleDigits = 4, char maskChar = '*')
    {
        var n = NationalIdValidator.Normalize(number);
        var visible = Math.Clamp(visibleDigits, 0, n.Length);
        return new string(maskChar, n.Length - visible) + n.Substring(n.Length - visible);
    }

    /// <summary>Groups a 14-digit number by field: C YYMMDD GG SSSS K. Other lengths are returned normalized.</summary>
    public static string Format(string? number)
    {
        var n = NationalIdValidator.Normalize(number);
        if (n.Length != 14)
        {
            return n;
        }

        return $"{n[0]} {n.Substring(1, 6)} {n.Substring(7, 2)} {n.Substring(9, 4)} {n[13]}";
    }

    /// <summary>Converts ASCII digits to Arabic-Indic digits (٠١٢٣٤٥٦٧٨٩). Other characters are kept.</summary>
    public static string ToArabicIndicDigits(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            sb.Append(c >= '0' && c <= '9' ? (char)('\u0660' + (c - '0')) : c);
        }

        return sb.ToString();
    }
}
