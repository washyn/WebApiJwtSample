using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace CertificadoTutoriaQuestPdf;

public class Program
{
    public static void Main(string[] args)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var model = new CertificadoTutoriaModel();

        Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Content().Column(col =>
                    {
                        col.Spacing(8);

                        // Encabezado
                        col.Item().AlignCenter().Text(model.Universidad).SemiBold().FontSize(16);
                        col.Item().AlignCenter().Text(model.EscuelaProfesional).SemiBold().FontSize(10);
                        col.Item().AlignCenter().Text(model.TipoTutoria).SemiBold().FontSize(10);

                        col.Item().PaddingTop(12).AlignCenter().Text(model.AnnoNombre).FontSize(8).Italic();

                        // Título de la Constancia
                        col.Item().PaddingTop(20).AlignCenter().Text(model.CodigoConstancia).ExtraBold().FontSize(18);

                        // Cuerpo
                        col.Item().PaddingTop(15).AlignCenter().Text(text =>
                        {
                            text.Span(
                                "El que suscribe, Docente Tutor o Coordinador de Tutoría de la Escuela Profesional de ");
                            text.Span(model.EscuelaProfesional).SemiBold();
                            text.Span(" de la ");
                            text.Span(model.Universidad).SemiBold();
                            text.Span(".");
                        });

                        col.Item().PaddingTop(12).AlignCenter().Text("HACE CONSTAR QUE:").ExtraBold().FontSize(20);

                        col.Item().PaddingTop(12).AlignCenter().Text(text =>
                        {
                            text.Span("El Estudiante ");
                            text.Span(model.NombreEstudiante).SemiBold();
                            text.Span(
                                " ha cumplido con las actividades programadas de tutoría en el semestre académico ");
                            text.Span(model.SemestreAcademico).SemiBold();
                            text.Span(
                                ", en concordancia al Reglamento y Normatividad del \"Sistema de Tutoría Universitaria\", (aprobado según Resolución ");
                            text.Span(model.Resolucion).SemiBold();
                            text.Span(").");
                        });

                        col.Item().PaddingTop(8).AlignCenter()
                            .Text(
                                "Se expide la presente constancia a solicitud del interesado para los fines pertinentes.");

                        // Fecha y Firma
                        col.Item().PaddingTop(15).AlignCenter().Text(model.Fecha);

                        col.Item().PaddingTop(40).AlignCenter().Column(firma =>
                        {
                            firma.Item().Width(200).BorderTop(1).PaddingTop(5).AlignCenter().Text(model.NombreFirmante)
                                .SemiBold();
                        });
                    });
                });
            })
            .ShowInPreviewer();

        Console.WriteLine("PDF generado exitosamente: CertificadoTutoria.pdf");
    }
}

public class CertificadoTutoriaModel
{
    public string Universidad { get; set; } = "Universidad Nacional de Juliaca";
    public string EscuelaProfesional { get; set; } = "ESCUELA PROFESIONAL DE INGENIERIA TEXTIL Y DE CONFECCIONES";
    public string TipoTutoria { get; set; } = "TUTORIA UNIVERSITARIA";
    public string AnnoNombre { get; set; } = "\"Año de la lucha contra la corrupción e impunidad\"";
    public string CodigoConstancia { get; set; } = "CONSTANCIA DE TUTORIA 8601-2026-1-UNAJ";
    public string NombreEstudiante { get; set; } = "IDME MAMANI MARIENNY";
    public string SemestreAcademico { get; set; } = "2026-1";
    public string Resolucion { get; set; } = "N° 1415-2015-R-UNA";
    public string Fecha { get; set; } = "Juliaca C.U. Abril del 2026";
    public string NombreFirmante { get; set; } = "HUANATICO SUAREZ ELIZABETH";
}
