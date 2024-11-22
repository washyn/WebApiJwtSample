using System;
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
    var endDay = new DateTime(year, 1, 1).AddYears(1).AddMonths(-1);
    var counter = 1;
    while (fistDay <= endDay)
    {
        // inicio fin
        var inicio = new DateTime(year, fistDay.Month, 1);
        var fin = inicio.AddMonths(1).AddDays(-1);
        var counterDisplay = (counter++).ToString().PadLeft(2, '0');
        var monthNameTreeLetters = new CultureInfo("es-pe").DateTimeFormat.GetMonthName(inicio.Month).ToUpper().Truncate(3);
        System.Console.WriteLine($"Sprint {counterDisplay} - {year} {monthNameTreeLetters} {inicio} - {fin}");
        fistDay = fistDay.AddMonths(1);
    }
}

void SprintBiMensual(int year)
{
    var fistDay = new DateTime(year, 1, 1);
    var endDay = new DateTime(year, 1, 1).AddYears(1).AddMonths(-1);
    var counter = 1;
    while (fistDay <= endDay)
    {
        // inicio fin
        var inicio = new DateTime(year, fistDay.Month, 1);
        var fin = inicio.AddMonths(2).AddDays(-1);
        var counterDisplay = (counter++).ToString().PadLeft(2, '0');
        var displayMonthTreeLettersInicio = new CultureInfo("es-pe").DateTimeFormat.GetMonthName(inicio.Month).ToUpper().Truncate(3);
        var displayMonthTreeLettersFin = new CultureInfo("es-pe").DateTimeFormat.GetMonthName(fin.Month).ToUpper().Truncate(3);

        System.Console.WriteLine($"{counterDisplay} {displayMonthTreeLettersInicio} {displayMonthTreeLettersFin} {inicio} - {fin}");
        fistDay = fistDay.AddMonths(2);
    }
}

void SprintQuincenal(int year)
{
    var fistDay = new DateTime(year, 1, 1);
    var endDay = new DateTime(year, 1, 1).AddYears(1).AddMonths(-1);
    var counter = 1;
    while (fistDay <= endDay)
    {
        // inicio fin
        var inicioPrimeraQuincena = new DateTime(year, fistDay.Month, 1);
        var finPrimeraQuincena = inicioPrimeraQuincena.AddDays(14);

        var inicioSegundaQuincena = finPrimeraQuincena.AddDays(1);
        var finSegundaQuincena = fistDay.AddMonths(1).AddDays(-1);

        var counterDisplay1 = (counter).ToString().PadLeft(2, '0');
        var counterDisplay2 = (counter + 1).ToString().PadLeft(2, '0');
        var displayMonthTreeLetters = new CultureInfo("es-pe").DateTimeFormat.GetMonthName(fistDay.Month).ToUpper().Truncate(3);

        System.Console.WriteLine($"{counterDisplay1} {displayMonthTreeLetters} I {inicioPrimeraQuincena} - {finPrimeraQuincena}");
        System.Console.WriteLine($"{counterDisplay2} {displayMonthTreeLetters} II {inicioSegundaQuincena} - {finSegundaQuincena}");

        fistDay = fistDay.AddMonths(1);
        counter = counter + 2;
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

        var finCondicional = fin;

        // si el fin es el proximo anio, usar el ultimo dia de este año
        if (fin.Year != year)
        {
            finCondicional = endDay;
        }
        System.Console.WriteLine($"semana {counterDisplay} {inicio} - {finCondicional}");

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

InsertarSprint(2023, SprintType.Senamal);

//SprintQuincenal(2023);
//ShowForInsertDaily(DateTime.Now, DateTime.Now.AddMonths(1));


static (int Anios, int Meses, int Dias) CalcularEdad(DateTime fechaNacimiento)
{
    DateTime fechaActual = DateTime.Now;
    int anios = fechaActual.Year - fechaNacimiento.Year;
    int meses = fechaActual.Month - fechaNacimiento.Month;
    int dias = fechaActual.Day - fechaNacimiento.Day;

    if (meses < 0 || (meses == 0 && dias < 0))
    {
        anios--;
        meses += 12;
    }

    return (anios, meses, dias);
}

void CalcularEdad2(DateTime fechaNacimiento)
{
    // Obtiene la fecha actual.
    DateTime fechaActual = DateTime.Now;

    // Calcula la diferencia entre la fecha actual y la fecha de nacimiento.
    TimeSpan diferencia = fechaActual - fechaNacimiento;

    // Convierte la diferencia en años, meses y días.
    
    int años = diferencia.Days / 365;
    int meses = (diferencia.Days % 365) / 30;
    int dias = (diferencia.Days % 365) % 30;

    // Imprime la edad calculada.
    Console.WriteLine($"Tu edad: {años} años + {meses} meses + {dias} días");
}




DateTime fechaNacimiento = new DateTime(1988, 12,8);

// Llama a la función para calcular la edad.
CalcularEdad2(fechaNacimiento);


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