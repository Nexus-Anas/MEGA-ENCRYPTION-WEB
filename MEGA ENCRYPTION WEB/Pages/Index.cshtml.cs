using MEGA_ENCRYPTION_WEB.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MEGA_ENCRYPTION_WEB.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _env;

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        [BindProperty]
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }

        public bool isFileEmpty;



        public void OnGet()
        {

        }


        public async Task<IActionResult> OnPostEncrypt()
        {
            try
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var file = HttpContext.Request.Form.Files[0];
                    if (file != null && file.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        var data = memoryStream.ToArray();
                        var encryptedData = await Cipher.EncryptFileAsync(data, Password);

                        if (encryptedData != null)
                        {
                            var newFileName = $"{file.FileName}";

                            var contentType = "application/octet-stream"; // Set the appropriate content type
                            return File(encryptedData, contentType, newFileName);
                        }
                    }
                }
                else
                {
                    isFileEmpty = true;
                    return Page();
                }
            }
            catch (Exception)
            {
                // Handle exceptions here
                return NotFound(); // or any other suitable response
            }

            return NotFound(); // No file was uploaded
        }






        public async Task<IActionResult> OnPostDecrypt()
        {
            try
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var file = HttpContext.Request.Form.Files[0];
                    if (file != null && file.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        var encryptedData = memoryStream.ToArray();
                        var decryptedData = await Cipher.DecryptBytesAsync(encryptedData, Password);

                        if (decryptedData != null)
                        {
                            var newFileName = $"{file.FileName}";

                            var contentType = "application/octet-stream"; // Set the appropriate content type
                            return File(decryptedData, contentType, newFileName);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Handle exceptions here
                return NotFound(); // or any other suitable response
            }

            return NotFound(); // No file was uploaded
        }



    }
}
