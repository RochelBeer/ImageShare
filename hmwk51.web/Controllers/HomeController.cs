using hmwk51.data;
using hmwk51.web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace hmwk51.web.Controllers
{
    public class HomeController : Controller
    {
        private string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=ImageShare; Integrated Security=true;";
        private IWebHostEnvironment _webHostEnvironment;
        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upload(IFormFile image, string password)
        {
            var fileName = $"{Guid.NewGuid()} - {image.FileName}";
            var filepath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filepath, FileMode.CreateNew);
            image.CopyTo(fs);

            Manager manager = new(connectionString);
            int id = manager.Add(fileName, password);

            ViewModel viewModel = new()
            {
                Image = new()
                {
                    Id = id,
                    Password = password,
                    FileName = fileName
                }
            };
            return View(viewModel);
        }

        public IActionResult ViewImage(string password, int id)
        {
            Manager manager = new(connectionString);
            Image image = manager.GetImage(id);
            ViewModel viewModel = new()
            {
                Image = image
            };
            List<int> idsInSession = HttpContext.Session.Get<List<int>>("IdsInSession");
            if (idsInSession != null && idsInSession.Contains(id))
            {
                return View(viewModel);
            }
            if (password == null)
            {
                return View(viewModel);
            }

            viewModel.CorrectPassword = image.Password == password;
            if (viewModel.CorrectPassword)
            {

                int viewCount = viewModel.Image.Views;
                viewCount++;
                manager.UpdateViews(id, viewCount);
                if (idsInSession == null)
                {
                    idsInSession = new();
                }
                idsInSession.Add(id);
                HttpContext.Session.Set("IdsInSession", idsInSession);
            }
            else
            {
                viewModel.InvalidPassword = true;
            }

            return View(viewModel);




        }
    }
}