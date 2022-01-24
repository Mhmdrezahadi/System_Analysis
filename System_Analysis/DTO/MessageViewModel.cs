using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Web.ViewModels
{
    public class MessageViewModel
    {
        [Required]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public UserViewModel From { get; set; }
        public UserViewModel To { get; set; }
        [Required]
        public GroupViewModel Group { get; set; }
        public bool IsMine { get; set; }
        public string Avatar { get; set; }
    }
}
