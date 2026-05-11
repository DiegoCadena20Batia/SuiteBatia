public class CorrectivoMPendienteLocal {
    public int IdLocal { get; set; }

    public int IdClaveCM { get; set; }

    public int TrabajosGeneral { get; set; }

    public int TecnicosUniforme { get; set; }

    public int TratoTecnicos { get; set; }

    public int TrabajosOrden { get; set; }

    public int MaterialesAdecuados { get; set; }

    public string Encuestado { get; set; }

    public string FirmaPath { get; set; }

    public bool Sincronizado { get; set; }

    public DateTime FechaRegistro { get; set; }
}