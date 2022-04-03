using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public ReservationsController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody]Reservation reservation)
        {
            reservation.ReservationTime = DateTime.Now;
            _dbContext.Reservations.Add(reservation);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status200OK);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetReservation()
        {
            var reservations = _dbContext.Reservations.Include(r => r.Movie).Include(r => r.User).Select(r => new
            {
                Id = r.Id,
                ReservationTime = r.ReservationTime,
                CustomerName = r.User != null ? r.User.Name : null,
                MovieName = r.Movie != null ? r.Movie.Name : null
            });

            return StatusCode(StatusCodes.Status200OK, reservations);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetReservationDetail(int id)
        {
            var reservation = _dbContext.Reservations.Where(r => r.Id == id).Include(r => r.Movie).Include(r => r.User).Select(r => new
            {
                Id = r.Id,
                ReservationTime = r.ReservationTime,
                CustomerName = r.User != null ? r.User.Name : null,
                MovieName = r.Movie != null ? r.Movie.Name : null,
                Email = r.User != null ? r.User.Email : null,
                Qty = r.Qty,
                Price = r.Price,
                Phone = r.Phone,
                PlayingDate = r.Movie != null ? r.Movie.PlayingDate.ToString() : null,
                PlayingTime = r.Movie != null ? r.Movie.PlayingTime.ToString() : null,
            }).FirstOrDefault();

            if(reservation == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, reservation);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var reservation = _dbContext.Reservations.Find(id);

            if(reservation == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            else
            {
                _dbContext.Reservations.Remove(reservation);
                _dbContext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK);
            }
        }
    }
}
