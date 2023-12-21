using Microsoft.AspNetCore.Mvc;
using Npgsql;
using c_sharp_lab_15_1.Models;

namespace c_sharp_lab_15_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        [HttpGet]
        [Route("select")]
        public IActionResult Select()
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            List<ClubModel> clubs = new List<ClubModel>();
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("select * from sport_clubs", connection);
                var firstReader = command.ExecuteReader();
                while (firstReader.Read())
                {
                    var club = new ClubModel
                    {
                        ClubId = firstReader.GetInt32(0),
                        Name = firstReader.GetString(1),
                        Photo = firstReader.IsDBNull(2) ? null : firstReader.GetFieldValue<byte[]>(2),
                        KindName = firstReader.IsDBNull(3) ? null : firstReader.GetString(3),
                    };
                    clubs.Add(club);
                }
                connection.Close();
            }
            return Ok(clubs);
        }

        [HttpPost]
        [Route("insert")]
        public IActionResult Insert(IFormFile photo, string name, string kind_name)
        {
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                photo.CopyTo(memoryStream);
                imageBytes = memoryStream.ToArray();
            }
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var command = new NpgsqlCommand("insert into sport_clubs (name, kind_name, photo) values (@name, @kind_name, @photo)", connection);
                command.Parameters.AddWithValue("name", name);
                command.Parameters.AddWithValue("kind_name", kind_name);
                command.Parameters.AddWithValue("photo", imageBytes);
                command.ExecuteNonQuery();
                connection.Close();
            }
            return Ok(true);
        }

        [HttpPut]
        [Route("update")]
        public IActionResult Update(int id, IFormFile photo, string kind_name, string name, int positionid, int experience)
        {
            try
            {
                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    photo.CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }
                var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
                using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("update sport_clubs set name = @name, kind_name = @kind_name, photo = @photo where id = @id", connection);
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("name", name);
                    command.Parameters.AddWithValue("kind_name", kind_name);
                    command.Parameters.AddWithValue("photo", imageBytes);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpDelete]
        [Route("delete")]
        public IActionResult Delete(int id)
        {
            try
            {
                var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.Development.json").Build();
                using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("delete from sport_clubs where id = @id", connection);
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return Ok(true);
            }
            catch
            {
                return Ok(false);
            }
        }
    }
}
