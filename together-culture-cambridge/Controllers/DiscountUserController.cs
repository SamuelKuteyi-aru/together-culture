using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using together_culture_cambridge.Data;
using together_culture_cambridge.Models;

namespace together_culture_cambridge.Controllers
{
    public class DiscountUserController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public DiscountUserController(ApplicationDatabaseContext context)
        {
            _context = context;
        }
        
       

        private bool DiscountUserExists(int id)
        {
            return _context.DiscountUser.Any(e => e.Id == id);
        }
    }
}
