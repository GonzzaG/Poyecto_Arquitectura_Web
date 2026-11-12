namespace BEL
{
    // Copia de los últimos valores aceptados por una escritura legítima.
    public class IntegritySnapshot
    {
        public string Tabla { get; set; }
        public int RegistroId { get; set; }
        public string Datos { get; set; }
        public string DVH { get; set; }
    }
}
