using Microsoft.AspNetCore.Mvc;
using Recommendations.Models;
using Recommendations.Source;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Recommendations.Source;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
namespace Recommendations.Controllers;

[ApiController]
[Route("[controller]")]
public class EducationController : ControllerBase
{
    [HttpGet("/users/me/export")]
public ActionResult Export(string? format = "pdf")
{
    using(var context = new RecommendationContext())
    {
        User current = context.Users.FirstOrDefault(x => x.IdUser == 1);
      
        
        if (format?.ToLower() == "json")
        {
            var jsonData = new
            {
                id = current.IdUser,
                name = current.NameUser,
                surname = current.SurnameUser,
                patronymic = current.PatronymicUser,
                phone = current.PhoneUser,
                email = current.EmailUser,
                exportDate = DateTime.Now
            };
            
            return Ok(jsonData);
        }
        
        try
        {
            string font = "/usr/share/fonts/dejavu-sans-mono-fonts/DejaVuSerif.ttf";
            BaseFont ft = BaseFont.CreateFont(font, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font f = new Font(ft, 12);
            Document doc = new Document(PageSize.A4);
            
            using var stream = new MemoryStream();
            PdfWriter.GetInstance(doc, stream);
            doc.Open();
            
            doc.Add(new Paragraph($"ID: {current.IdUser}", f));
            doc.Add(new Paragraph($"Имя: {current.NameUser}", f));
            doc.Add(new Paragraph($"Фамилия: {current.SurnameUser}", f));
            doc.Add(new Paragraph($"Отчество: {current.PatronymicUser}", f));
            doc.Add(new Paragraph($"Телефон: {current.PhoneUser}", f));
            doc.Add(new Paragraph($"Email: {current.EmailUser}", f));
            doc.Add(new Paragraph($"Дата экспорта: {DateTime.Now}", f));
            
            doc.Close();
            byte[] pdfbytes = stream.ToArray();
            return File(pdfbytes, "application/pdf", $"profile_{DateTime.Now:yyyy-MM-dd}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Ошибка создания PDF", details = ex.Message });
        }
    }
}

    [HttpPost("eduation")]
    public ActionResult Get(Education e)
    {
        using(var context = new RecommendationContext() )
        {
            e.IdEducation = context.Educations.Any() ? context.Educations.Max(x=>x.IdEducation) +1 : 1;
            context.Educations.Add(e);
            context.SaveChanges();
            return Ok(e);
        }
    }
    [HttpPost("/User/users/{id}/education")]
    public ActionResult Get(int id)
    {
        using(var context = new RecommendationContext() )
        {
            List<Education> ed = context.Educations.Where(x=>x.IdUser==id).ToList();

            return ed.Count()>0 ? Ok(ed) : NotFound();
        }
    }
    [HttpPost("/User/users/{id}/experience")]
    public ActionResult experience(int id)
    {
        using(var context = new RecommendationContext() )
        {
            List<Experience> ed = context.Experiences.Where(x=>x.IdUser==id).ToList();

            return ed.Count()>0 ? Ok(ed) : NotFound();
        }
    }
    [HttpPost("/User/users/{id}/skills")]
    public ActionResult skills(int id)
    {
        using(var context = new RecommendationContext() )
        {
            List<UserSkill> ed = context.UserSkills.Include(x=>x.IdSkillNavigation).Where(x=>x.UserId==id).ToList();

            return ed.Count()>0 ? Ok(ed) : NotFound();
        }
    }
    [HttpPut("eduation")]
    public ActionResult edit(Education e)
    {
        using(var context = new RecommendationContext() )
        {
            Education current = context.Educations.FirstOrDefault(x=>x.IdEducation == e.IdEducation);
            current = e;
            context.SaveChanges();
            return Ok(current);
        }
    }
    [HttpDelete("eduation")]
    public ActionResult delete(Education e)
    {
        using(var context = new RecommendationContext() )
        {
            context.Educations.Remove(e);
            context.SaveChanges();
            return Ok(e);
        }
    }
    
}
