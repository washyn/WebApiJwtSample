using System.Globalization;
using Volo.Abp;

var culture = new CultureInfo("es-pe");

// OK...
void ShowForInsertDaily(DateTime start, DateTime end)
{
    if (!(start < end))
    {
        throw new Exception("La fecha de inicio debe ser antes que la fecha de finalizacion.");
    }
    while (start <= end)
    {
        var date = new DateTime(start.Year, start.Month, start.Day);
        System.Console.WriteLine($"{start.ToString(new CultureInfo("es-pe"))} {GetDay(start.DayOfWeek)} {(start.DayOfWeek.IsWeekday() ? "1" : "0")}");
        start = start.AddDays(1);
    }
}


#region Others
string GetDay(DayOfWeek dayOfWeek)
{
    var textInfo = CultureInfo.CurrentCulture.TextInfo;
    var culture = new CultureInfo("es-pe");
    var text = culture.DateTimeFormat.GetDayName(dayOfWeek);
    return textInfo.ToTitleCase(text);
}

string GetMonth(int month)
{
    Check.Range(month, nameof(month), 1, 12);
    var culture = new CultureInfo("es-pe");
    return culture.DateTimeFormat.GetMonthName(month);
}

string GetMonth3Letters(int month)
{
    Check.Range(month, nameof(month), 1, 12);
    var culture = new CultureInfo("es-pe");
    return culture.DateTimeFormat.GetMonthName(month).ToUpper().Truncate(3) ?? string.Empty;
}

string ShowMonth(int month)
{
    return culture.DateTimeFormat.GetMonthName(month).ToUpper().Truncate(3) ?? string.Empty;
}

#endregion

#region Periodos


void SprintMensual(int year)
{
    var fistDay = new DateTime(year, 1, 1);
    var endDay = new DateTime(year, 1, 1).AddYears(1).AddDays(-1);
    var counter = 1;
    while (fistDay <= endDay)
    {
        // inicio fin
        var inicio = new DateTime(year, fistDay.Month, 1);
        var fin = inicio.AddMonths(1).AddDays(-1);
        var counterDisplay = (counter++).ToString().PadLeft(2, '0');
        System.Console.WriteLine($"Sprint {counterDisplay} - {year} {ShowMonth(inicio.Month)} {inicio.ToString("d", culture)} - {fin.ToString("d", culture)}");
        fistDay = fistDay.AddMonths(1);
    }
}

void SprintBiMensual(int year)
{
    var fistDay = new DateTime(year, 1, 1);
    var endDay = new DateTime(year, 1, 1).AddYears(1).AddDays(-1);
    var counter = 1;
    while (fistDay <= endDay)
    {
        // inicio fin
        var inicio = new DateTime(year, fistDay.Month, 1);
        var fin = inicio.AddMonths(2).AddDays(-1);
        var counterDisplay = (counter++).ToString().PadLeft(2, '0');
        System.Console.WriteLine($"{counterDisplay} {ShowMonth(inicio.Month)} {ShowMonth(fin.Month)} {inicio.ToString("d", culture)} - {fin.ToString("d", culture)}");
        fistDay = fistDay.AddMonths(2);
    }
}

void SprintQuincenal(int year)
{
    var fistDay = new DateTime(year, 1, 1);
    var endDay = new DateTime(year, 1, 1).AddYears(1).AddDays(-1);
    while (fistDay <= endDay)
    {
        // inicio fin
        var inicioPrimeraQuincena = new DateTime(year, fistDay.Month, 1);
        var finPrimeraQuincena = inicioPrimeraQuincena.AddDays(14);

        var inicioSegundaQuincena = finPrimeraQuincena.AddDays(1);
        var finSegundaQuincena = fistDay.AddMonths(1).AddDays(-1);

        System.Console.WriteLine($"{ShowMonth(fistDay.Month)} I {inicioPrimeraQuincena.ToString("d", culture)} - {finPrimeraQuincena.ToString("d", culture)}");
        System.Console.WriteLine($"{ShowMonth(fistDay.Month)} II {inicioSegundaQuincena.ToString("d", culture)} - {finSegundaQuincena.ToString("d", culture)}");
        fistDay = fistDay.AddMonths(1);
    }
}


void SprintSemanal(int year)
{
    var fistDay = new DateTime(year, 1, 1);
    var endDay = new DateTime(year, 1, 1).AddYears(1).AddDays(-1);
    var counter = 1;
    while (fistDay <= endDay)
    {
        // inicio fin
        var inicio = new DateTime(year, fistDay.Month, fistDay.Day);
        var fin = inicio.AddDays(6);
        var counterDisplay = (counter++).ToString().PadLeft(2, '0');

        // si el fin es el proximo anio, usar el ultimo dia de este ano
        if (fin.Year != year)
        {
            System.Console.WriteLine($"semana {counterDisplay} {inicio.ToString("d", culture)} - {endDay.ToString("d", culture)}");
        }
        else
        {
            System.Console.WriteLine($"semana {counterDisplay} {inicio.ToString("d", culture)} - {fin.ToString("d", culture)}");
        }

        fistDay = fistDay.AddDays(7);
    }
}

#endregion


void InsertarSprint(int year, SprintType typeSprint)
{
    switch (typeSprint)
    {
        case SprintType.Senamal:
            SprintSemanal(year);
            break;
        case SprintType.Quincenal:
            SprintQuincenal(year);
            break;
        case SprintType.Mensual:
            SprintMensual(year);
            break;
        case SprintType.Bimensual:
            SprintBiMensual(year);
            break;
        default:
            break;
    }
}

InsertarSprint(2023, SprintType.Mensual);

//SprintQuincenal(2023);
//ShowForInsertDaily(DateTime.Now, DateTime.Now.AddMonths(1));


// insert por lote, 

// bimestral OK, el 1 de cada mes y el ultimo dia del mes, sin importar el dia de semana
// mensual OK, inicia el 1 de enero, y termina el ultimo dia del mes de febrero
// quincenal ... inicia el dia 1 hasta el 14, pero algunos meses tienes 28 dias y otros 31 como se deberia funcionar esto?
// semanal ... des que dia inicia ? en caso de que un mes inicie el dia miercoles o viernes

// mensual, el 1 enero y el ultimo dia, hasta diciembre, OK, daily ok -> total sprint 12
// bimensial el 1 neror ultio dia febrero Ok, dily ok -> total sprint 6
// semanal
// quincenal

enum SprintType
{
    Senamal,
    Quincenal,
    Mensual,
    Bimensual,
}