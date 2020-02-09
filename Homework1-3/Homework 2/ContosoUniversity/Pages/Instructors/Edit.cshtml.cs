using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

//The instructor edit code must handle the following scenarios:
//If the user clears the office assignment, delete the OfficeAssignment entity.
//If the user enters an office assignment and it was empty, create a new OfficeAssignment entity.
//If the user changes the office assignment, update the OfficeAssignment entity.

//Gets the current Instructor entity from the database using eager loading for the OfficeAssignment, CourseAssignment, and CourseAssignment.Course navigation properties.
//Updates the retrieved Instructor entity with values from the model binder.TryUpdateModel prevents overposting.
//If the office location is blank, sets Instructor.OfficeAssignment to null. When Instructor.OfficeAssignment is null, the related row in the OfficeAssignment table is deleted.
//Calls PopulateAssignedCourseData in OnGetAsync to provide information for the checkboxes using the AssignedCourseData view model class.
//Calls UpdateInstructorCourses in OnPostAsync to apply information from the checkboxes to the Instructor entity being edited.
//Calls PopulateAssignedCourseData and UpdateInstructorCourses in OnPostAsync if TryUpdateModel fails.These method calls restore the assigned course data entered on the page when it is redisplayed with an error message.

namespace ContosoUniversity.Pages.Instructors
{
    public class EditModel : InstructorCoursesPageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public EditModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Instructor Instructor { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Instructor == null)
            {
                return NotFound();
            }
            PopulateAssignedCourseData(_context, Instructor);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (instructorToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,
                "Instructor",
                i => i.FirstMidName, i => i.LastName,
                i => i.HireDate, i => i.OfficeAssignment))
            {
                if (String.IsNullOrWhiteSpace(
                    instructorToUpdate.OfficeAssignment?.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;
                }
                UpdateInstructorCourses(_context, selectedCourses, instructorToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            UpdateInstructorCourses(_context, selectedCourses, instructorToUpdate);
            PopulateAssignedCourseData(_context, instructorToUpdate);
            return Page();
        }
    }
}