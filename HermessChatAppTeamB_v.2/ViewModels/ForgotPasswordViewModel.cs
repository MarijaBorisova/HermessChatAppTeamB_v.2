using System.ComponentModel.DataAnnotations;

namespace HermessChatAppTeamB_v._2.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}