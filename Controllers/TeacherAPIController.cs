using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Cummulative1.Models;
using System.Collections.Generic;

namespace Cummulative1.Controllers
{
    [Route("api/Teacher")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDBContext _context;

        public TeacherAPIController(SchoolDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of all teachers.
        /// </summary>
        /// <returns>A list of teacher objects.</returns>
        [HttpGet]
        [Route("ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            List<Teacher> teachers = new List<Teacher>();

            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM teachers";

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        teachers.Add(new Teacher
                        {
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString(),
                            EmployeeNumber = reader["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(reader["hiredate"]),
                            Salary = Convert.ToDecimal(reader["salary"])
                        });
                    }
                }
            }

            return teachers;
        }

        /// <summary>
        /// Finds a teacher by their ID.
        /// </summary>
        /// <param name="id">The ID of the teacher to find.</param>
        /// <returns>A teacher object if found, otherwise null.</returns>
        [HttpGet]
        [Route("FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher selectedTeacher = null;

            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        selectedTeacher = new Teacher
                        {
                            TeacherId = Convert.ToInt32(reader["teacherid"]),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString(),
                            EmployeeNumber = reader["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(reader["hiredate"]),
                            Salary = Convert.ToDecimal(reader["salary"])
                        };
                    }
                }
            }

            return selectedTeacher;
        }



        /// <summary>
        /// Adds a new teacher to the database.
        /// </summary>
        /// <param name="newTeacher">The teacher object to add.</param>
        /// <returns>The ID of the newly added teacher.</returns>
        [HttpPost]
        [Route("AddTeacher")]
        public int AddTeacher([FromBody] Teacher newTeacher)
        {
            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = @"
            INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary)
            VALUES (@TeacherFName, @TeacherLName, @EmployeeNumber, @HireDate, @Salary);
            SELECT LAST_INSERT_ID();";

                command.Parameters.AddWithValue("@TeacherFName", newTeacher.TeacherFName);
                command.Parameters.AddWithValue("@TeacherLName", newTeacher.TeacherLName);
                command.Parameters.AddWithValue("@EmployeeNumber", newTeacher.EmployeeNumber);
                command.Parameters.AddWithValue("@HireDate", newTeacher.HireDate);
                command.Parameters.AddWithValue("@Salary", newTeacher.Salary);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }




        /// <summary>
        /// Deletes a teacher from the database by their ID.
        /// </summary>
        /// <param name="id">The ID of the teacher to delete.</param>
        /// <returns>The number of rows affected.</returns>
        [HttpDelete]
        [Route("DeleteTeacher/{id}")]
        public int DeleteTeacher(int id)
        {
            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates an existing teacher in the database.
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="updatedTeacher">The updated teacher object.</param>
        /// <returns>An HTTP response indicating success or failure.</returns>
        [HttpPut]
        [Route("UpdateTeacher/{id}")]
        public IActionResult UpdateTeacher(int id, [FromBody] Teacher updatedTeacher)
        {
            if (id != updatedTeacher.TeacherId)
            {
                return BadRequest("Teacher ID mismatch.");
            }

            if (string.IsNullOrWhiteSpace(updatedTeacher.TeacherFName) || string.IsNullOrWhiteSpace(updatedTeacher.TeacherLName))
            {
                return BadRequest("Teacher name cannot be empty.");
            }

            if (updatedTeacher.HireDate > DateTime.Now)
            {
                return BadRequest("Hire date cannot be in the future.");
            }

            if (updatedTeacher.Salary < 0)
            {
                return BadRequest("Salary cannot be less than 0.");
            }

            using (MySqlConnection connection = _context.AccessDatabase())
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = @"
            UPDATE teachers
            SET teacherfname = @TeacherFName,
                teacherlname = @TeacherLName,
                employeenumber = @EmployeeNumber,
                hiredate = @HireDate,
                salary = @Salary
            WHERE teacherid = @TeacherId";

                command.Parameters.AddWithValue("@TeacherFName", updatedTeacher.TeacherFName);
                command.Parameters.AddWithValue("@TeacherLName", updatedTeacher.TeacherLName);
                command.Parameters.AddWithValue("@EmployeeNumber", updatedTeacher.EmployeeNumber);
                command.Parameters.AddWithValue("@HireDate", updatedTeacher.HireDate);
                command.Parameters.AddWithValue("@Salary", updatedTeacher.Salary);
                command.Parameters.AddWithValue("@TeacherId", updatedTeacher.TeacherId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound("Teacher not found.");
                }
            }

            return Ok("Teacher updated successfully.");
        }

    }
}
