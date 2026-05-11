public class FotoCorrectivoPendienteLocal {
    public int IdLocal { get; set; }

    // Relación con CorrectivoMPendienteLocal
    public int IdCorrectivoLocal { get; set; }

    // Ruta local del archivo
    public string PathFoto { get; set; }

    // Firma o foto normal
    public bool EsFirma { get; set; }

    // Control de sincronización
    public bool Sincronizado { get; set; }

    public DateTime FechaRegistro { get; set; }
}