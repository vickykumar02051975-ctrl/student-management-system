
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.data;
using StudentManagementSystem.Models;
using System.Text.Json;
using System.Text;

namespace StudentManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // READ: Display students with Search and Filters
        public async Task<IActionResult> Index(
            string? searchName,
            string? rollNo,
            string? gender)
        {
            var query = _context.Students.AsQueryable();

            // Search by Student Name
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(s =>
                    s.FullName.Contains(searchName));
            }

            // Search by Roll Number
            if (!string.IsNullOrWhiteSpace(rollNo))
            {
                query = query.Where(s =>
                    s.RollNo.Contains(rollNo));
            }

            // Filter by Gender
            if (!string.IsNullOrWhiteSpace(gender))
            {
                query = query.Where(s =>
                    s.Gender == gender);
            }

            // Preserve filter values
            ViewBag.SearchName = searchName;
            ViewBag.RollNo = rollNo;
            ViewBag.Gender = gender;

            // Fetch matching students
            var students = await query
                .OrderBy(s => s.StudentId)
                .ToListAsync();

            return View(students);
        }

        // DASHBOARD: Display student statistics
        public async Task<IActionResult> Dashboard()
        {
            // Total Students
            ViewBag.TotalStudents =
                await _context.Students.CountAsync();

            // Total Courses
            ViewBag.TotalCourses = await _context.Students
                .Select(s => s.Course)
                .Distinct()
                .CountAsync();

            // Male Students
            int maleStudents = await _context.Students
                .CountAsync(s => s.Gender == "Male");

            ViewBag.MaleStudents = maleStudents;

            // Female Students
            int femaleStudents = await _context.Students
                .CountAsync(s => s.Gender == "Female");

            ViewBag.FemaleStudents = femaleStudents;

            // Recent 5 Students
            ViewBag.RecentStudents = await _context.Students
                .OrderByDescending(s => s.StudentId)
                .Take(5)
                .ToListAsync();

            // Gender Chart Data
            ViewBag.GenderChartData = JsonSerializer.Serialize(new
            {
                labels = new[] { "Male", "Female" },
                values = new[] { maleStudents, femaleStudents }
            });

            return View();
        }

        // CREATE: Open form
        public IActionResult Create()
        {
            return View();
        }

        // CREATE: Save student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }

        // DETAILS: Show student information
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        // EDIT: Open edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students.FindAsync(id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        // EDIT: Update student
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(student);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }

        // DELETE: Confirmation page
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
                return NotFound();

            return View(student);
        }

        // DELETE: Remove student
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        // EXPORT STUDENTS TO CSV
public async Task<IActionResult> ExportCSV(
    string? searchName,
    string? rollNo,
    string? gender)
{
    var query = _context.Students.AsQueryable();

    if (!string.IsNullOrWhiteSpace(searchName))
        query = query.Where(s => s.FullName.Contains(searchName));

    if (!string.IsNullOrWhiteSpace(rollNo))
        query = query.Where(s => s.RollNo.Contains(rollNo));

    if (!string.IsNullOrWhiteSpace(gender))
        query = query.Where(s => s.Gender == gender);

    var students = await query
        .OrderBy(s => s.StudentId)
        .ToListAsync();

    var csv = new StringBuilder();

    csv.AppendLine("Student ID,Roll No,Full Name,Gender,Email,Phone,Course,Address");

    foreach (var s in students)
    {
        string Escape(string? value)
        {
            return "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";
        }

        csv.AppendLine(
            $"{s.StudentId}," +
            $"{Escape(s.RollNo)}," +
            $"{Escape(s.FullName)}," +
            $"{Escape(s.Gender)}," +
            $"{Escape(s.Email)}," +
            $"{Escape(s.Phone)}," +
            $"{Escape(s.Course)}," +
            $"{Escape(s.Address)}"
        );
    }

    byte[] fileBytes = Encoding.UTF8.GetPreamble()
        .Concat(Encoding.UTF8.GetBytes(csv.ToString()))
        .ToArray();

    return File(
        fileBytes,
        "text/csv",
        "Students.csv"
    );
}

    }
}
