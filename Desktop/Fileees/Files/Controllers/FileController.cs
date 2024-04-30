using Files.Data;
using Files.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Security.Claims;

namespace Files.Controllers
{
    public class FileController : Controller
    {
        private readonly AppDbContext _context;

        public FileController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles ="admin,regular")]
        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> Download(string fileId)
        {
            var file = await _context.Files.FindAsync(fileId);
            if (file == null)
            {
                return NotFound();
            }

            if(!User.IsInRole("admin") && file.OwnerId != GetUserId())
            {
                return Forbid();
            }
            byte[] fileBytes = Convert.FromBase64String(file.Data);

            file.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return File(fileBytes, "application/pdf", $"file_{fileId}.pdf");
        }

        [Authorize(Roles ="admin")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                return BadRequest();
            }

            if(file.Length > 1048576)
            {
                return BadRequest("File size is bigger than 1MB.");
            }

            byte[] fileBytes;
            using(var ms=new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            string base64= Convert.ToBase64String(fileBytes);

            var files = new FileClass
            {
                Data=base64, 
                
                UpdatedAt = DateTime.UtcNow,
            };

            _context.Files.Add(files);
            await _context.SaveChangesAsync();

            return Ok("File uploaded successfully");
        }


        private int GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userId !=null && int.TryParse(userId.Value, out int idOfUser))
            {
                return idOfUser;
            }
            return -1;
        }
    }
}
