using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class TodoBasicDto
    {
        public int Id { get; set;}
        public string OwnerName { get; set;}
        public string Title { get; set;}

        public bool IsCompleted { get; set;}

        public TodoBasicDto(int id, string ownername, string title, bool iscompleted)
        {
        Id = id;OwnerName = ownername; Title = title; IsCompleted = iscompleted;    
        
        }


    }
}
