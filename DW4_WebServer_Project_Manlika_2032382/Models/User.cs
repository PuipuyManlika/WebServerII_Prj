using System.ComponentModel.DataAnnotations;

namespace DW4_WebServer_Project_Manlika_2032382.Model
{
    public class User
    {
        [Key]
        public string Uid { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public User() { }

        public User(string name, string email, string password)
        {
            Uid = Guid.NewGuid().ToString();
            Name = name;
            Email = email;
            Password = password;
        }

        public bool CheckPassword(string password)
        {
            if (Password == password)
            {
                return true;
            }
            return false;
        }
    }
}
