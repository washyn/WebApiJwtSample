namespace QuestPdfDocument;

public enum RolTutoria
{
    TutorAcademico,
    CoordinadorTutoria,
    DocenteTutor
}

public enum ModalidadTutoria
{
    Presencial,
    Virtual,
    Hibrida
}

public class Universidad
{
    public string Nombre { get; set; } = "Universidad Nacional de Juliaca";
    public string Facultad { get; set; } = "Facultad de Ingeniería de Minas, Geología y Civil";
    public string Escuela { get; set; } = "Escuela Profesional de Ingeniería de Sistemas";
    public string Unidad { get; set; } = "Unidad de Tutoría y Bienestar Universitario";
    public string Direccion { get; set; } = "Av. Nueva Zelandia N° 631 Urbanización la Capilla";
    public string Telefono { get; set; } = "(051) 328722";
    public string Ubicacion { get; set; } = "Juliaca - Puno - Perú";
    public string Rector { get; set; } = "Dr. Carlos Alberto Quispe Mamani";
    public string Decano { get; set; } = "Mg. Juan Pedro Ramos Condori";
    public string JefeUnidadTutoria { get; set; } = "Mg. Rosa Elena Choquecahua Quispe";
    public string LogoBase64 { get; set; } = Data.File;
}

public class Docente
{
    public string Dni { get; set; } = "45896321";
    public string Nombres { get; set; } = "Ernesto Rafael";
    public string Apellidos { get; set; } = "Mamani Quispe";
    public string FullName => $"{Apellidos}, {Nombres}";
    public string Genero { get; set; } = "Masculino";
    public string CorreoInstitucional { get; set; } = "ernesto.mamani@unj.edu.pe";
    public string Telefono { get; set; } = "+51 987 654 321";
    public string CodigoDocente { get; set; } = "DOC-IS-00256";
    public string GradoAcademico { get; set; } = "Magister en Ingeniería de Software";
    public string Condicion { get; set; } = "Contratado";
    public string Categoria { get; set; } = "Principal Asociado";
    public string Dedicacion { get; set; } = "Tiempo Parcial";
}

public class Estudiante
{
    public string CodigoUniversitario { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string FullName => $"{Apellidos}, {Nombres}";
    public string Dni { get; set; } = string.Empty;
    public string SemestreAcademico { get; set; } = string.Empty;
    public string CorreoInstitucional { get; set; } = string.Empty;
    public string ModalidadIngreso { get; set; } = string.Empty;
    public decimal PromedioPonderado { get; set; }
}

public class PeriodoAcademico
{
    public string Semestre { get; set; } = "2024 - II";
    public string Ciclo { get; set; } = "2024 - II Semestre Académico";
    public DateTime FechaInicio { get; set; } = new DateTime(2024, 08, 15);
    public DateTime FechaFin { get; set; } = new DateTime(2024, 12, 20);
    public DateTime FechaInicioTutorias { get; set; } = new DateTime(2024, 09, 02);
    public DateTime FechaFinTutorias { get; set; } = new DateTime(2024, 11, 29);
    public int CantidadSesionesRequeridas { get; set; } = 8;
}

public class Tutoria
{
    public string CodigoTutoria { get; set; } = "TUT-IS-2024-II-00158";
    public RolTutoria RolTutor { get; set; } = RolTutoria.TutorAcademico;
    public ModalidadTutoria Modalidad { get; set; } = ModalidadTutoria.Presencial;
    public string Horario { get; set; } = "Miércoles de 03:00 pm a 05:00 pm";
    public string Local { get; set; } = "Aula 205 - Pabellón de Sistemas";
    public int CantidadEstudiantesAsignados { get; set; } = 25;
    public List<string> EjesTematicos { get; set; } = new()
    {
        "Integración y adaptación a la vida universitaria",
        "Hábitos y técnicas de estudio",
        "Orientación académica y curricular",
        "Desarrollo de competencias transversales",
        "Prevención de riesgos académicos",
        "Proyecto de vida y profesional",
        "Bienestar emocional y salud mental",
        "Vinculación con la sociedad y extensión"
    };
}

public class DocumentoOpciones
{
    public string NumeroCarta { get; set; } = "00158";
    public string SequenceStart { get; set; } = "UT-BU-";
    public string FechaGenerada { get; set; } = "Juliaca, 20 de agosto de 2024";
    public string NombreDocumento { get; set; } = "CARTA DE DESIGNACIÓN COMO TUTOR ACADÉMICO";
}

public static class MockData
{
    public static Universidad Universidad => new();
    public static Docente Docente => new();
    public static PeriodoAcademico Periodo => new();
    public static Tutoria Tutoria => new();
    public static DocumentoOpciones OpcionesDocumento => new();

    public static List<Estudiante> ObtenerEstudiantesAsignados()
    {
        return new List<Estudiante>
        {
            new() { CodigoUniversitario = "2024110001", Nombres = "Ana Carolina", Apellidos = "Condori Flores", Dni = "76543210", SemestreAcademico = "I", CorreoInstitucional = "ana.condori@unj.edu.pe", ModalidadIngreso = "CEPREUNJ", PromedioPonderado = 14.25m },
            new() { CodigoUniversitario = "2024110015", Nombres = "Luis Alberto", Apellidos = "Quispe Ccahuana", Dni = "76543211", SemestreAcademico = "I", CorreoInstitucional = "luis.quispe@unj.edu.pe", ModalidadIngreso = "Examen Ordinario", PromedioPonderado = 13.80m },
            new() { CodigoUniversitario = "2024110028", Nombres = "María Fernanda", Apellidos = "Ramos Huanca", Dni = "76543212", SemestreAcademico = "I", CorreoInstitucional = "maria.ramos@unj.edu.pe", ModalidadIngreso = "CEPREUNJ", PromedioPonderado = 15.10m },
            new() { CodigoUniversitario = "2023110145", Nombres = "José Miguel", Apellidos = "Zapata Laura", Dni = "74859612", SemestreAcademico = "III", CorreoInstitucional = "jose.zapata@unj.edu.pe", ModalidadIngreso = "Examen Ordinario", PromedioPonderado = 12.65m },
            new() { CodigoUniversitario = "2023110178", Nombres = "Gladys Maritza", Apellidos = "Poma Arapa", Dni = "74859613", SemestreAcademico = "III", CorreoInstitucional = "gladys.poma@unj.edu.pe", ModalidadIngreso = "Deportistas Calificados", PromedioPonderado = 11.90m },
            new() { CodigoUniversitario = "2022110220", Nombres = "Edgar Rolando", Apellidos = "Cruz Mayta", Dni = "72365410", SemestreAcademico = "V", CorreoInstitucional = "edgar.cruz@unj.edu.pe", ModalidadIngreso = "Examen Ordinario", PromedioPonderado = 10.85m },
            new() { CodigoUniversitario = "2022110234", Nombres = "Flor de María", Apellidos = "Lázaro Chambi", Dni = "72365411", SemestreAcademico = "V", CorreoInstitucional = "flor.lazaro@unj.edu.pe", ModalidadIngreso = "CEPREUNJ", PromedioPonderado = 13.45m },
            new() { CodigoUniversitario = "2021110310", Nombres = "Percy Omar", Apellidos = "Mamani Vilca", Dni = "70123654", SemestreAcademico = "VII", CorreoInstitucional = "percy.mamani@unj.edu.pe", ModalidadIngreso = "Examen Ordinario", PromedioPonderado = 12.20m },
            new() { CodigoUniversitario = "2021110345", Nombres = "Janeth Carolina", Apellidos = "Torres Tito", Dni = "70123655", SemestreAcademico = "VII", CorreoInstitucional = "janeth.torres@unj.edu.pe", ModalidadIngreso = "Primeros Puntajes", PromedioPonderado = 15.60m },
            new() { CodigoUniversitario = "2020110456", Nombres = "Williams Daniel", Apellidos = "Huanca Choque", Dni = "68974512", SemestreAcademico = "IX", CorreoInstitucional = "williams.huanca@unj.edu.pe", ModalidadIngreso = "Examen Ordinario", PromedioPonderado = 11.50m }
        };
    }

    public static List<string> ObtenerCompromisosTutor()
    {
        return new List<string>
        {
            "Realizar como mínimo ocho (08) sesiones de tutoría presencial o virtual en el semestre académico",
            "Elaborar y presentar el Plan de Trabajo de Tutoría dentro de los diez (10) días hábiles siguientes de iniciadas las clases",
            "Llevar el registro individual y grupal de las sesiones de tutoría en el sistema académico institucional",
            "Identificar oportunamente los factores de riesgo académico que afecten el rendimiento de los estudiantes",
            "Derivar a los servicios correspondientes (psicopedagógico, médico, social) a los estudiantes que lo requieran",
            "Participar en las reuniones de coordinación y capacitación docente en materia de tutoría programadas por la Unidad",
            "Elaborar y presentar el informe final de actividades de tutoría al concluir el semestre académico",
            "Garantizar la confidencialidad de la información personal y académica de los estudiantes bajo su tutoría"
        };
    }
}
