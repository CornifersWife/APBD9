using APBD9.Context;
using APBD9.Models;
using Microsoft.AspNetCore.Mvc;

namespace APBD9.Controllers;

[ApiController]
[Route("api")]
public class TripsController(HospitalDbContext dbContext) : ControllerBase {
    private readonly HospitalDbContext dbContext = dbContext;

    [HttpPost]
    [Route("prescriptions")]
    public IActionResult AddPrescription(Patient patient,[FromBody] int idDoctor, IEnumerable<PrescribedMedicament> medicaments, DateTime date,
        DateTime dueDate) {

        var foundPatient = dbContext.Patients
            .FirstOrDefault(e => 
                e.IdPatient == patient.IdPatient &&
                e.FirstName == patient.FirstName &&
                e.LastName == patient.LastName &&
                e.BirthDate == patient.BirthDate,
                null);

        var doctor = dbContext.Doctors.FirstOrDefault(e => e.IdDoctor == idDoctor,null);
        if (doctor is null)
            return BadRequest("No doctor with given Id Exists");

        if (medicaments.Count() > 10)
            return BadRequest("The prescription exceeds the maximum amount of medicaments for one prescriptions");

        var medicamentIds = dbContext.Medicaments
            .Select(e => e.IdMedicament);

        var medicamentsMissing = medicaments.Where(e => !medicamentIds.Contains(e.IdMedicament)).ToList();
        if (medicamentsMissing.Any())
            return BadRequest($"one or more medicaments are not in the database\n {medicamentsMissing}");

        if (dueDate < date)
            return BadRequest("'DueDate' has to be after 'Date'");


        using (var trans = dbContext.Database.BeginTransaction()) {
            if (foundPatient is null) {
                dbContext.Patients.Add(patient);
                dbContext.SaveChanges();
            }


            var newPrescription = new Prescription() {
                Date = date,
                DueDate = dueDate,
                IdDoctor = doctor.IdDoctor,
                IdPatient = patient.IdPatient
            };
            
            dbContext.Prescriptions.Add(newPrescription);
            dbContext.SaveChanges();

            foreach (var prescribedMedicament in medicaments) {
                dbContext.PrescriptionMedicaments.Add(new Prescription_Medicament() {
                    Details = prescribedMedicament.Descritpion,
                    Dose = prescribedMedicament.Dose,
                    idMedicament = prescribedMedicament.IdMedicament,
                    idPrescription = newPrescription.IdPrescription
                });
            }

            dbContext.SaveChanges();
            trans.Commit();
        }
        
        return StatusCode(201,"Object created successfully");
    }
}