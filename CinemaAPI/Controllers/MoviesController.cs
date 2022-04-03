using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult AllMovies(string? sort, int pageNumber, int pageSize)
        {
            if(pageNumber <= 0 || pageSize <= 0)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "The params pageNumber and pageSize are mandatory");
            }

            var movies = _dbContext.Movies.Select(m => new
            { 
                Id = m.Id,
                Name = m.Name,
                Duration = m.Duration,
                Language = m.Language,
                Rating = m.Rating,
                Genre = m.Genre,
                ImageUrl = m.ImageUrl
            });

            switch (sort)
            {
                case "desc":
                    return StatusCode(StatusCodes.Status200OK, movies.OrderByDescending(m => m.Rating).Skip((pageNumber - 1) * pageSize).Take(pageSize));
                case "asc":
                    return StatusCode(StatusCodes.Status200OK, movies.OrderBy(m => m.Rating).Skip((pageNumber - 1) * pageSize).Take(pageSize));
                default:
                    return StatusCode(StatusCodes.Status200OK, movies.Skip((pageNumber - 1) * pageSize).Take(pageSize));
            }
        }

        [Authorize]
        [HttpGet("[action]/{id}")]
        public IActionResult MovieDetail(int id)
        {
            var movie = _dbContext.Movies.Find(id);

            if (movie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "No record found against this id");
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, movie);
            }
        }

        [Authorize]
        [HttpGet("[action]")]
        public IActionResult FindMovies(string movieName)
        {
            var movies = _dbContext.Movies.Where(m => m.Name.Contains(movieName)).Select(m => new
            {
                Id = m.Id,
                Name = m.Name,
                ImageUrl = m.ImageUrl
            });

            return StatusCode(StatusCodes.Status200OK, movies);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromForm] Movie movie)
        {

            if (movie.Image != null)
            {
                movie.ImageUrl = SaveImage(movie);
            }

            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
            var movie = _dbContext.Movies.Find(id);

            if (movie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "No record found against this id");
            }
            else
            {
                if (movieObj.Image != null)
                {
                    DeleteImage(movie);
                    movie.ImageUrl = SaveImage(movieObj);
                }

                movie.Name = movieObj.Name;
                movie.Description = movieObj.Description;
                movie.Language = movieObj.Language;
                movie.Duration = movieObj.Duration;
                movie.PlayingDate = movieObj.PlayingDate;
                movie.PlayingTime = movieObj.PlayingTime;
                movie.Rating = movieObj.Rating;
                movie.Genre = movieObj.Genre;
                movie.TrailorUrl = movieObj.TrailorUrl;
                movie.TicketPrice = movieObj.TicketPrice;
                _dbContext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, "Record updated successfully");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movie = _dbContext.Movies.Find(id);

            if (movie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "No record found against this id");
            }
            else
            {
                DeleteImage(movie);

                _dbContext.Movies.Remove(movie);
                _dbContext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, "Record deleted");
            }
        }

        private string? SaveImage(Movie movie)
        {
            if (movie.Image != null)
            {
                var guid = Guid.NewGuid();
                var filePath = Path.Combine("wwwroot", $"{guid}.jpg");
                var fileStream = new FileStream(filePath, FileMode.Create);

                movie.Image.CopyTo(fileStream);
                return filePath.Remove(0, 8);
            }
            else
            {
                return null;
            }
        }

        private void DeleteImage(Movie movie)
        {
            if (movie.ImageUrl != null)
            {
                var filePath = Path.Combine("wwwroot", movie.ImageUrl.ToString());
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
    }
}
