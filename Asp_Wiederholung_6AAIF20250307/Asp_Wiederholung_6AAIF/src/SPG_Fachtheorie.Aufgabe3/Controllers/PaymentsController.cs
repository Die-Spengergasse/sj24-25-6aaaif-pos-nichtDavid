using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe3.Dtos;

namespace SPG_Fachtheorie.Aufgabe3.Controllers
{

    [Route("api/[controller]")]  // [controller] --> Name vor "Controller" --> /api/payments
    [ApiController]              // Wird als controller berücksichtigt.
    public class PaymentsController : ControllerBase
    {
        private readonly AppointmentContext _db;
        public PaymentsController(AppointmentContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<List<PaymentsDto>> GetAllPayments([FromQuery] int? number, [FromQuery] DateTime? dateFrom)
        {

            return Ok(_db.Payments
                //.Where(e => string.IsNullOrEmpty(type) ? true : e.Type.ToLower() == type.ToLower())
                .Select(e => new PaymentsDto(
                    e.Id,
                    e.Employee.FirstName,
                    e.Employee.LastName,
                    e.CashDesk.Number,
                    e.PaymentType.ToString(),
                    e.PaymentItems.Where(i => i.Payment == e).Sum(i => i.Price)))
                .ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<PaymentDetailDto> GetPayment(int id)
        {
            var payment = _db.Payments
                .Where(e => e.Id == id)
                .Select(e => new PaymentDetailDto(
                    e.Id,
                    e.Employee.FirstName,
                    e.Employee.LastName,
                    e.CashDesk.Number,
                    e.PaymentType.ToString(),
                    e.PaymentItems.Select(i =>
                            new PaymentItemDto(
                                i.ArticleName,
                                i.Amount,
                                i.Price
                            )
                        ).ToList()
                    )
                ).FirstOrDefault();
            if (payment is null)
                return NotFound();
            return Ok(payment);
        }
    }
}
