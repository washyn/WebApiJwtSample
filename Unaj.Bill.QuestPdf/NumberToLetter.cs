using System;
using System.Globalization;

namespace QuestPDF.Invoice;

public static class NumberLetter
{
    #region Privates

    private static readonly string[] UNIDADES = new string[]
    {
        "", "un ", "dos ", "tres ", "cuatro ", "cinco ", "seis ", "siete ", "ocho ", "nueve ", "diez ", "once ",
        "doce ", "trece ", "catorce ", "quince ", "dieciseis ", "diecisiete ", "dieciocho ", "diecinueve ",
        "veinte "
    };

    private static readonly string[] DECENAS = new string[]
    {
        "venti", "treinta ", "cuarenta ", "cincuenta ", "sesenta ", "setenta ", "ochenta ", "noventa ", "cien "
    };

    private static readonly string[] CENTENAS = new string[]
    {
        "ciento ", "doscientos ", "trescientos ", "cuatrocientos ", "quinientos ", "seiscientos ", "setecientos ",
        "ochocientos ", "novecientos "
    };

    private static string ConvertGroup(string n)
    {
        var output = "";

        if (n == "100")
        {
            output = ("cien ");
        }
        else if (n[0] != '0')
        {
            output = (CENTENAS[int.Parse(n[0].ToString()) - 1]);
        }

        var k = Convert.ToInt32(n.Substring(1));
        if (k <= 20)
        {
            output = output + (UNIDADES[k]);
        }
        else
        {
            if ((k > 30) && (n[2] != '0'))
            {
                output = output +
                         ($"{DECENAS[int.Parse(n[1].ToString()) - 2]}y {UNIDADES[int.Parse(n[2].ToString())]}");
            }
            else
            {
                output = output + ($"{DECENAS[int.Parse(n[1].ToString()) - 2]}{UNIDADES[int.Parse(n[2].ToString())]}");
            }
        }

        return output;
    }

    #endregion

    #region publics

    public static string ConvertToLetter(decimal number, string currency = "", bool format = false)
    {
        var base_number = number;
        var base_number2 = string.Empty;
        var decNumberStr = "00";
        var converted = "";
        var decimales = "";

        if ((base_number < 0) || (base_number > 999999999))
        {
            return "No es posible convertir el numero en letras";
        }

        var culturePeru = new CultureInfo("es-pe");

        var div_decimales = base_number.ToString("F2", culturePeru).Split(".");

        if (div_decimales.Length > 1)
        {
            base_number2 = div_decimales[0];
            decNumberStr = div_decimales[1].PadRight(2, '0');

            if (decNumberStr.Length == 2)
            {
                var decNumberStrFill = decNumberStr.PadLeft(9, '0');
                var decCientos = decNumberStrFill.Substring(6);
                decimales = ConvertGroup(decCientos);
            }
        }

        var numberStr = base_number2;
        var numberStrFill = numberStr.PadLeft(9, '0');
        var millones = numberStrFill.Substring(0, 3);
        var miles = numberStrFill.Substring(3, 3);
        var cientos = numberStrFill.Substring(6);

        if (long.Parse(millones) > 0)
        {
            if (millones == "001")
            {
                converted = converted + "un millon ";
            }
            else if (long.Parse(millones) > 0)
            {
                converted = converted + $"{ConvertGroup(millones)}millones ";
            }
        }

        if (long.Parse(miles) > 0)
        {
            if (miles == "001")
            {
                converted = converted + "mil ";
            }
            else if (long.Parse(miles) > 0)
            {
                converted = $"{ConvertGroup(miles)}mil ";
            }
        }

        if (long.Parse(cientos) > 0)
        {
            if (cientos == "001")
            {
                converted = converted + "un ";
            }
            else if (long.Parse(cientos) > 0)
            {
                converted = converted + $"{ConvertGroup(cientos)} ";
            }
        }

        if (long.Parse(base_number2) == 0)
        {
            converted = "Cero ";
        }

        var valor_convertido = string.Empty;

        if (format)
        {
            if (string.IsNullOrEmpty(decimales))
            {
                valor_convertido = number.ToString("F2", culturePeru) + " (" + converted.ToPascalCase() + "00/100 " +
                                   currency + ")";
            }
            else
            {
                valor_convertido = number.ToString("F2", culturePeru) + " (" + converted.ToPascalCase() + decNumberStr +
                                   "/100 " + currency + ")";
            }
        }
        else
        {
            if (string.IsNullOrEmpty(decimales))
            {
                valor_convertido = converted.ToPascalCase() + "con " + "00/100 " + currency;
            }
            else
            {
                valor_convertido = converted.ToPascalCase() + "con " + decNumberStr + "/100 " + currency;
            }
        }

        return valor_convertido;
    }

    #endregion
}

