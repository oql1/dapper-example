using System.ComponentModel.DataAnnotations;

namespace Orders.Api.Models.Dtos;

public abstract class OrderWriteDto : IValidatableObject
{
    [Required, StringLength(20, MinimumLength = 1)]
    public required string OrderNumber { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? VehicleId { get; set; }

    [Required, StringLength(80, MinimumLength = 1)]
    public required string OriginCity { get; set; }

    [Required, StringLength(80, MinimumLength = 1)]
    public required string DestinationCity { get; set; }

    [Required, StringLength(200, MinimumLength = 1)]
    public required string CargoDescription { get; set; }

    [Range(typeof(decimal), "0.01", "99999999.99")]
    public decimal WeightKg { get; set; }

    [Range(typeof(decimal), "0", "9999999999.99")]
    public decimal FreightAmount { get; set; }

    [Required, RegularExpression("^(MXN|USD)$")]
    public string CurrencyCode { get; set; } = "MXN";

    [Required, AllowedValues("Created", "Assigned", "InTransit", "Delivered", "Cancelled")]
    public required string Status { get; set; }

    [Required]
    public DateTime? PickupDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.Equals(OriginCity?.Trim(), DestinationCity?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult(
                "El origen y el destino deben ser diferentes.",
                [nameof(OriginCity), nameof(DestinationCity)]);
        }

        if (PickupDate.HasValue && DeliveryDate.HasValue && DeliveryDate < PickupDate)
        {
            yield return new ValidationResult(
                "La fecha de entrega no puede ser anterior a la fecha de recolección.",
                [nameof(DeliveryDate), nameof(PickupDate)]);
        }

        if ((Status is "Assigned" or "InTransit") && VehicleId is null)
        {
            yield return new ValidationResult(
                "Se requiere un vehículo para las órdenes asignadas o en tránsito.",
                [nameof(VehicleId), nameof(Status)]);
        }

        if (Status == "Delivered" && DeliveryDate is null)
        {
            yield return new ValidationResult(
                "La fecha de entrega es obligatoria cuando la orden está entregada.",
                [nameof(DeliveryDate), nameof(Status)]);
        }
    }
}
