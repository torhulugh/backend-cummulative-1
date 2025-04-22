using Microsoft.AspNetCore.Mvc;
using Cummulative1.Models;
using System.Collections.Generic;

namespace Cummulative1.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly TeacherAPIController _api;

        public TeacherPageController(TeacherAPIController api)
        {
            _api = api;
        }

        /// <summary>
        /// Displays a list of all teachers.
        /// </summary>
        /// <returns>A view displaying the list of teachers.</returns>
        [HttpGet]
        [Route("[controller]/List")]
        public IActionResult List()
        {
            List<Teacher> teachers = _api.ListTeachers();
            return View(teachers);
        }

        /// <summary>
        /// Displays details of a specific teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to display.</param>
        /// <returns>A view displaying the teacher's details.</returns>
        [HttpGet]
        [Route("[controller]/Show/{id}")]
        public IActionResult Show(int id)
        {
            Teacher teacher = _api.FindTeacher(id);
            if (teacher == null)
            {
                return NotFound("Teacher not found.");
            }
            return View(teacher);
        }

        /// <summary>
        /// Displays a form to add a new teacher.
        /// </summary>
        /// <returns>A view with the form to add a new teacher.</returns>
        [HttpGet]
        [Route("[controller]/New")]
        public IActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Handles the submission of the new teacher form.
        /// </summary>
        /// <param name="newTeacher">The teacher object to add.</param>
        /// <returns>Redirects to the list of teachers.</returns>
        [HttpPost]
        [Route("[controller]/Create")]
        public IActionResult Create(Teacher newTeacher)
        {
            if (ModelState.IsValid)
            {
                int teacherId = _api.AddTeacher(newTeacher);
                return RedirectToAction("Show", new { id = teacherId });
            }
            return View("New", newTeacher);
        }

        /// <summary>
        /// Displays a confirmation page for deleting a teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>A view to confirm the deletion.</returns>
        [HttpGet]
        [Route("[controller]/DeleteConfirm/{id}")]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher teacher = _api.FindTeacher(id);
            if (teacher == null)
            {
                return NotFound("Teacher not found.");
            }
            return View(teacher);
        }

        /// <summary>
        /// Handles the deletion of a teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>Redirects to the list of teachers.</returns>
        [HttpPost]
        [Route("[controller]/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            int rowsAffected = _api.DeleteTeacher(id);
            if (rowsAffected == 0)
            {
                return NotFound("Teacher not found or could not be deleted.");
            }
            return RedirectToAction("List");
        }

        /// <summary>
        /// Displays a form to edit an existing teacher.
        /// </summary>
        /// <param name="id">The ID of the teacher to edit.</param>
        /// <returns>A view with the form to edit the teacher.</returns>
        [HttpGet]
        [Route("[controller]/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            Teacher teacher = _api.FindTeacher(id);
            if (teacher == null)
            {
                return NotFound("Teacher not found.");
            }
            return View(teacher);
        }


        /// <summary>
        /// Handles the submission of the edit teacher form.
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="updatedTeacher">The updated teacher object.</param>
        /// <returns>Redirects to the teacher details page or displays validation errors.</returns>
        [HttpPost]
        [Route("[controller]/Update/{id}")]
        public IActionResult Update(int id, Teacher updatedTeacher)
        {
            if (id != updatedTeacher.TeacherId)
            {
                ModelState.AddModelError(string.Empty, "Teacher ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                IActionResult result = _api.UpdateTeacher(id, updatedTeacher);
                if (result is OkObjectResult)
                {
                    return RedirectToAction("Show", new { id = id });
                }
                else if (result is BadRequestObjectResult badRequest)
                {
                    ModelState.AddModelError(string.Empty, badRequest.Value.ToString());
                }
                else if (result is NotFoundObjectResult notFound)
                {
                    ModelState.AddModelError(string.Empty, notFound.Value.ToString());
                }
            }

            return View("Edit", updatedTeacher);
        }


    }
}
