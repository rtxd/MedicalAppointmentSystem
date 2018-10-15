using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        public string getName(int id)
        {
            foreach (User user in _context.Users)
            {
                if (user.ID == id)
                    if (user.Role == "Doctor")
                    {
                        return "Dr " + user.LastName;
                    }
                    else return user.FirstName + " " + user.LastName;
            }
            return "Error: Invalid User";

        }

        private readonly MedicalSystemContext _context;

        public AppointmentsController(MedicalSystemContext context)
        {
            _context = context;
        }

        public static List<User> listOfUsers = new List<User>();

        // GET: Appointments
        [Authorize]
        public async Task<IActionResult> Index()
        {
            
            foreach(User user in _context.Users)
            {
                listOfUsers.Add(user);
            }

            var medicalSystemContext = _context.Appointments.Include(a => a.Patient);
            
            

            //Only display appointments for the currently logged in user
            foreach (User user in _context.Users)
            {
                if (Common.GetUserAspNetId(User) == user.AspNetUserId)
                {
                    foreach (Appointment appointment in _context.Appointments)
                        if (user.ID == appointment.PatientID || user.ID == appointment.DoctorID || user.Role == "Receptionist" || user.Role == "Admin")
                        {
                            //Set variables for view here
                            ViewBag.role = user.Role;
                            ViewBag.thisUsersID = user.ID;
                            // This sets every label using .doctorName to the same thing - only holds 1 val
                            ViewBag.doctorName = getName(appointment.DoctorID);
                            ViewBag.patientName = getName(appointment.PatientID);

                            //Return the view
                            return View(await medicalSystemContext.ToListAsync());
                        }
                }
            }

            //If the user made it this far then the user has no appointments so an error message will be displayed
            ViewBag.thisUsersID = null;
            return View(await medicalSystemContext.ToListAsync());
        }

        //GET: Appointments/PatientDetails/5
        [Authorize]
        public async Task<IActionResult> PatientDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Users
                .Include(a => a.Appointments)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Appointments/Details/5
        [Authorize]
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

        
            ViewBag.Patientname = getName(appointment.PatientID);
            ViewBag.Doctorname = getName(appointment.DoctorID);
            ViewBag.AppointmentTime = appointment.Time.ToShortTimeString();

          
            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize]
        public IActionResult Create()
        {
            List<SelectListItem> dList = _context.Users.Where(d => d.Role == "Doctor").Select(d => new SelectListItem
            {
                Value = d.ID.ToString(),
                Text = "Dr. " + d.FirstName.Substring(0,1) + ". " + d.LastName
            }).ToList();
            List<SelectListItem> pList = _context.Users.Where(p => p.Role == "Patient").Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.LastName + ", " + p.FirstName
            }).ToList();

            dList.Insert(0, (new SelectListItem { Text = "No Preference", Value = "-1" }));

            ViewData["DoctorID"] = dList;
            ViewData["PatientID"] = pList;

            var userId = Common.GetUserAspNetId(User);



            var curUserRole = from u in _context.Users
                              where u.AspNetUserId == (Common.GetUserAspNetId(User))
                              select u.Role.ToString();



            if (!String.IsNullOrEmpty(curUserRole.FirstOrDefault()))
            {
                ViewData["Role"] = curUserRole.FirstOrDefault();
            }
            else
            {
                ViewData["Role"] = "None";
            }

            //foreach(string role in curUserRole.ToList())
            //{
            //    if (role == null) ViewData["Role"] = "None";
            //    else ViewData["Role"] = role.ToString();
            //}

            return View();
        }


        // POST: Appointments/Create -- Query Appointment Available Time Slots
        [HttpPost]
        public IActionResult GetTimeSlots([FromBody]Newtonsoft.Json.Linq.JObject message)
        {
            string selDate = (string)message["date"];
            int selDoctor = (int)message["doctor"];

            if (String.IsNullOrEmpty(selDate))
            {
                selDate = DateTime.Today.ToString("dd/MM/yyyy");
            }



            List<string> BookedSlots = new List<string>();

            if (selDoctor != -1) 
            {
                // Doctor Selected
                BookedSlots = (from a in _context.Appointments
                                  where a.Time.Date.ToString("dd/MM/yyyy") == selDate && a.DoctorID == selDoctor
                                  select a.Time.ToString("hh:mm tt")).ToList();
            } else {
                //No Doctor Selected
                var doctorCount = _context.Users.Count(n => n.Role == "Doctor");

                var AllBookedSlots = (from a in _context.Appointments
                                      where a.Time.Date.ToString("dd/MM/yyyy") == selDate
                                      select a.Time.ToString("hh:mm tt")).ToList();

                var dict = AllBookedSlots.GroupBy(s => s).ToDictionary(g => g.Key, g => g.Count());

                foreach (var d in dict)
                {
                    if (d.Value == doctorCount)
                    {
                        BookedSlots.Add(d.Key);
                    }
                }
            }


            List<string> timeSlots = new List<string> {"09:00 AM", "09:30 AM", "10:00 AM", "10:30 AM", "11:00 AM", "11:30 AM", "12:00 PM", "12:30 PM",
                                                        "01:00 PM", "01:30 PM", "02:00 PM", "02:30 PM", "03:00 PM", "03:30 PM", "04:00 PM", "04:30 PM"};

            //Removed Booked Slots
            foreach (var slot in BookedSlots)
            {
                timeSlots.Remove(slot);
            }

            //foreach (var slot in timeSlots)
            //{
            //    int tmp = int.Parse(slot.ToString().Substring(0, 2));
            //    if (DateTime.Now.Hour < tmp)
            //    {
            //        timeSlots.Remove(tim);
            //    }
            //}


            List<SelectListItem> slotsList = timeSlots.ConvertAll(a =>
            {              
                return new SelectListItem()
                {
                    Text = a.ToString(),
                    Value = a.ToString(),
                    Selected = false

                };
            });

            return Json(slotsList);
        }


        // POST: Appointments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Notes,Location,Time,DoctorID,PatientID")] Appointment appointment)
        {
            //Link Patient ID if Patient Using System
            if (appointment.PatientID == -1)
            {
                var pat = from u in _context.Users
                          where u.AspNetUserId == Common.GetUserAspNetId(User)
                          select u.ID;
                if (pat.Count() != 0)
                {
                    appointment.PatientID = pat.FirstOrDefault();
                }
                
            }


            //Handle No Doctor Preference
            if (appointment.DoctorID == -1)
            {
                var busyDoctors = from s in _context.Appointments
                           where s.Time == appointment.Time
                           select s.DoctorID;

                var freeDoctors = from d in _context.Users
                                  where d.Role == "Doctor" && busyDoctors.All(b => b != d.ID)
                                  select d.ID;

                if (freeDoctors.Count() != 0)
                {
                    appointment.DoctorID = freeDoctors.First();
                }

            }

            if (ModelState.IsValid)
            {
                // All Appt Details Valid - Save and Redirect
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Error - Reload Page

            List<SelectListItem> dList = _context.Users.Where(d => d.Role == "Doctor").Select(d => new SelectListItem
            {
                Value = d.ID.ToString(),
                Text = "Dr. " + d.FirstName.Substring(0, 1) + ". " + d.LastName
            }).ToList();
            List<SelectListItem> pList = _context.Users.Where(p => p.Role == "Patient").Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.LastName + ", " + p.FirstName
            }).ToList();

            dList.Insert(0, (new SelectListItem { Text = "No Preference", Value = "-1" }));

            ViewData["DoctorID"] = dList;
            ViewData["PatientID"] = pList;

            var curUserRole = from u in _context.Users
                              where u.AspNetUserId == (Common.GetUserAspNetId(User))
                              select u.Role.ToString();

            if (!String.IsNullOrEmpty(curUserRole.FirstOrDefault()))
            {
                ViewData["Role"] = curUserRole.FirstOrDefault();
            }
            else
            {
                ViewData["Role"] = "None";
            }

            return View();
        }

        // GET: Appointments/Edit/5
        [Authorize]
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

            List<SelectListItem> dList = _context.Users.Where(d => d.Role == "Doctor").Select(d => new SelectListItem
            {
                Value = d.ID.ToString(),
                Text = "Dr. " + d.FirstName.Substring(0, 1) + ". " + d.LastName
            }).ToList();
            List<SelectListItem> pList = _context.Users.Where(p => p.Role == "Patient").Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.LastName + ", " + p.FirstName
            }).ToList();

            dList.Insert(0, (new SelectListItem { Text = "No Preference", Value = "-1" }));

            ViewData["DoctorID"] = dList;
            ViewData["PatientID"] = pList;

            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Notes,Location,Time,DoctorID,PatientID")] Appointment appointment)
        {
            //Handle No Doctor Preference
            if (appointment.DoctorID == -1)
            {
                var busyDoctors = from s in _context.Appointments
                                  where s.Time == appointment.Time
                                  select s.DoctorID;

                var freeDoctors = from d in _context.Users
                                  where d.Role == "Doctor" && busyDoctors.All(b => b != d.ID)
                                  select d.ID;

                if (freeDoctors.Count() != 0)
                {
                    appointment.DoctorID = freeDoctors.First();
                }

            }

            if (id != appointment.ID)
            {
                return NotFound();
            }
            //appointment.ID = id;

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
        [Authorize]
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
