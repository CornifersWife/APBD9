namespace APBD9.Models;

public class PrescribedMedicament {
    public int IdMedicament { get; set; }
    public int? Dose { get; set; }
    public string Descritpion { get; set; } = null!;
}