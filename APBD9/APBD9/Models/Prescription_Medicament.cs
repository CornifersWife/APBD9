namespace APBD9.Models;

public class Prescription_Medicament {
    public int idMedicament { get; set; }
    public int idPrescription { get; set; }
    public int? Dose { get; set; }
    public string Details { get; set; }

    public Medicament Medicament { get; set; }
    public Prescription Prescription{ get; set; }
}