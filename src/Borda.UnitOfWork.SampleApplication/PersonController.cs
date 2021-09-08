using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Borda.UnitOfWork.SampleApplication
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public PersonController(ApplicationContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Persons.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            _context.Persons.Add(person);
            
            _unitOfWork.AddEvent(new PersonCreatedEvent()
            {
                FullName = $"{person.FirstName} {person.LastName}"
            });

            await _unitOfWork.CompleteAsync();
            
            return Ok(person);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var person = _context.Persons.Find(id);
            _context.Persons.Remove(person);
            _context.SaveChanges();

            return NoContent();
        }
    }
}