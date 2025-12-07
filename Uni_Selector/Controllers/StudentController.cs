using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uni_Selector.Data;
using Uni_Selector.Models;
using Uni_Selector.ViewModels.Student;
using Uni_Selector.Models.Enums;

namespace Uni_Selector.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<StudentController> _logger;
        private readonly IConfiguration _configuration;

        public StudentController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env,
            ILogger<StudentController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
            _logger = logger;
            _configuration = configuration;
        }


   

    }
}
