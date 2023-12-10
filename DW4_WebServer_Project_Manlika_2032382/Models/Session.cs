using System.ComponentModel.DataAnnotations;

namespace DW4_WebServer_Project_Manlika_2032382.Model
{
    public class Session
    {
        [Key]
        public string Token { get; set; }

        [Required]
        public string Email { get; set; }

        public Session() { }

        public Session(string email)
        {
            Token = Guid.NewGuid().ToString();
            Email = email;
        }
    }
}
