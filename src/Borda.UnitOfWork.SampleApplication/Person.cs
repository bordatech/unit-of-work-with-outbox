using System.ComponentModel.DataAnnotations;

namespace Borda.UnitOfWork.SampleApplication
{
    public class Person
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}