using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreAI.Project1_Api101.Context;
using NetCoreAI.Project1_Api101.Entities;

namespace NetCoreAI.Project1_Api101.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApiContext _context;

        public CustomerController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult CustomerList()
        {
            var value = _context.Customers.ToList();

            return Ok(value);
        }

        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();

            return Ok("Customer successfully added!");
        }

        [HttpDelete]
        public IActionResult DeleteCustomer(int id)
        {
            var value = _context.Customers.Find(id);
            _context.Customers.Remove(value);
            _context.SaveChanges();

            return Ok("Customer deleted successfully.");
        }

        // If the same HTTP method's to be used multiple times (as in HttpGet here), 2nd and other occurrences must specify an additional parameter/name within parentheses, as shown below.
        [HttpGet("GetCustomer")]
        public IActionResult GetCustomer(int id)
        {
            var value = _context.Customers.Find(id);

            return Ok(value);
        }

        [HttpPut]
        public IActionResult UpdateCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Customers.Update(customer);
            _context.SaveChanges();

            return Ok("Customer updated successfully.");
        }
    }
}
