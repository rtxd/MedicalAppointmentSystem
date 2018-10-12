using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UTSMedicalSystem.FrontEnd.BusinessLogic;
using UTSMedicalSystem.FrontEnd.Data;
using UTSMedicalSystem.FrontEnd.Models;

namespace UTSMedicalSystem.FrontEnd.Controllers
{
    public class AppointmentsController : Controller
    {
        private string getDoctorName(int id)
        {
            foreach(User doctor in _context.Users)
            {
                if (doctor.ID == id)
                    return "Dr " + doctor.LastName;
            }
            return "Error: Invalid Doctor ID";

        }

        private readonly MedicalSystemContext _context;

        public AppointmentsController(MedicalSystemContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var medicalSystemContext = _context.Appointments.Include(a => a.Patient);
            //medicalSystemContext = _context.Appointments.Include(a => a.Doctor);

            //Only display appointments for the currently logged in user
            foreach (User user in _context.Users)
            {
                if (Common.GetUserAspNetId(User) == user.AspNetUserId)
                {
                    foreach (Appointment appointment in _context.Appointments)
                        if (user.ID == appointment.PatientID)
                        {
                            ViewBag.role = user.Role;
                            ViewBag.thisUsersID = user.ID;
                            ViewBag.doctorName = getDoctorName(appointment.DoctorID);
                            return View(await medicalSystemContext.ToListAsync());
                        }
                }
            }

            //If the user made it this far then the user has no appointments so an error message will be displayed
            ViewBag.thisUsersID = null;
            return View(await medicalSystemContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["PatientID"] = new SelectList(_context.Users, "ID", "ID");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Notes,Location,Time,DoctorID,PatientID")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientID"] = new SelectList(_context.Users, "ID", "ID", appointment.PatientID);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.SingleOrDefaultAsync(m => m.ID == id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["PatientID"] = new SelectList(_context.Users, "ID", "ID", appointment.PatientID);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Notes,Location,Time,DoctorID,PatientID")] Appointment appointment)
        {
            if (id != appointment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientID"] = new SelectList(_context.Users, "ID", "ID", appointment.PatientID);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.SingleOrDefaultAsync(m => m.ID == id);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.ID == id);
        }
    }
}
