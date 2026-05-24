using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace H.DTOs
{
    public class ProveedorListadoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Ruc { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Contacto { get; set; }
        public bool Activo { get; set; }
    }

    public class ProveedorDetalleDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Ruc { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Contacto { get; set; }
        public bool Activo { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class ProveedorRequestDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }
        [JsonPropertyName("ruc")]
        public string? Ruc { get; set; }
        [JsonPropertyName("telefono")]
        public string? Telefono { get; set; }
        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }
        [JsonPropertyName("contacto")]
        public string? Contacto { get; set; }
        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }
    }
}