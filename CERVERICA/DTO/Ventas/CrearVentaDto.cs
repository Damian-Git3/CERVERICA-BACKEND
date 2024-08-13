using CERVERICA.Dtos;
using CERVERICA.Models;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class CrearVentaDto
    {
        //ATRIBUTOS DIRECCIÓN ENVIO
        public string? NombrePersonaRecibe { get; set; }
        public string? Calle { get; set; }
        public string? NumeroCasa { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        //ATRIBUTOS DIRECCIÓN ENVIO

        //ATRIBUTOS TARJETA
        public string? NombrePersonaTarjeta { get; set; }
        public string? NumeroTarjeta { get; set; }
        public string? MesExpiracion { get; set; }
        public string? AnioExpiracion { get; set; }
        public string? CVV { get; set; }
        //ATRIBUTOS TARJETA

        public MetodoPago MetodoPago { get; set; }
        public MetodoEnvio MetodoEnvio { get; set; }

        public List<DetalleVentaDto> Detalles { get; set; }

        // Implementación de la validación condicional
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MetodoEnvio == MetodoEnvio.EnvioDomicilio)
            {
                if (string.IsNullOrWhiteSpace(NombrePersonaRecibe))
                {
                    yield return new ValidationResult("El nombre de la persona que recibe es obligatorio.", new[] { nameof(NombrePersonaRecibe) });
                }

                if (string.IsNullOrWhiteSpace(Calle))
                {
                    yield return new ValidationResult("La calle es obligatoria.", new[] { nameof(Calle) });
                }

                if (string.IsNullOrWhiteSpace(NumeroCasa))
                {
                    yield return new ValidationResult("El número de casa es obligatorio.", new[] { nameof(NumeroCasa) });
                }

                if (string.IsNullOrWhiteSpace(CodigoPostal))
                {
                    yield return new ValidationResult("El código postal es obligatorio.", new[] { nameof(CodigoPostal) });
                }

                if (string.IsNullOrWhiteSpace(Ciudad))
                {
                    yield return new ValidationResult("La ciudad es obligatoria.", new[] { nameof(Ciudad) });
                }

                if (string.IsNullOrWhiteSpace(Estado))
                {
                    yield return new ValidationResult("El estado es obligatorio.", new[] { nameof(Estado) });
                }
            }

            if (MetodoPago == MetodoPago.TarjetaCredito)
            {
                if (string.IsNullOrWhiteSpace(NombrePersonaTarjeta))
                {
                    yield return new ValidationResult("El nombre de la persona en la tarjeta es obligatorio.", new[] { nameof(NombrePersonaTarjeta) });
                }

                if (string.IsNullOrWhiteSpace(NumeroTarjeta))
                {
                    yield return new ValidationResult("El número de tarjeta es obligatorio.", new[] { nameof(NumeroTarjeta) });
                }

                if (string.IsNullOrWhiteSpace(MesExpiracion))
                {
                    yield return new ValidationResult("El mes de expiración es obligatorio.", new[] { nameof(MesExpiracion) });
                }

                if (string.IsNullOrWhiteSpace(AnioExpiracion))
                {
                    yield return new ValidationResult("El año de expiración es obligatorio.", new[] { nameof(AnioExpiracion) });
                }

                if (string.IsNullOrWhiteSpace(CVV))
                {
                    yield return new ValidationResult("El CVV es obligatorio.", new[] { nameof(CVV) });
                }
            }
        }
    }
}
