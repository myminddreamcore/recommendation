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
using Recommendations;
namespace Recommendations.Controllers;

[ApiController]
[Route("[controller]")]
public class SkillController : ControllerBase
{
 

    [HttpPost("skills")]
    public ActionResult Get(Skill e)
    {
        using(var context = new RecommendationContext() )
        {
            e.IdSkill = context.Skills.Any() ? context.Skills.Max(x=>x.IdSkill) +1 : 1;
            context.Skills.Add(e);
            context.SaveChanges();
            return Ok(e);
        }
    }
    [HttpGet("skills")]
    public ActionResult find()
    {
        using(var context = new RecommendationContext() )
        {
            List<string> skills = context.Skills.Select(x=>x.NameSkill).ToList();
            return skills.Count()>0 ? Ok(skills) : NotFound();
        }
    }
    
    [HttpGet("skillsall")]
    public ActionResult skillsall()
    {
        using(var context = new RecommendationContext() )
        {
            List<Skill> skills = context.Skills.ToList();
            return skills.Count()>0 ? Ok(skills) : NotFound();
        }
    }
    [HttpGet("skillsuser/{id}")]
    public ActionResult skillsuser(int id)
    {
        using(var context = new RecommendationContext() )
        {
            List<Skillss> skills = context.UserSkills.Include(x=>x.IdSkillNavigation).Where(x=>x.UserId==id).Select(x=>new Skillss{
                Id = x.IdSkill,
                Name = x.IdSkillNavigation.NameSkill,
                Rating = x.MarkSkill,
                Apply = context.Confirmations.Where(y=>y.UserTo==id && y.IdSkill==x.Id && y.Status=="Подтверждено").Count(),
                Date = context.Confirmations.OrderByDescending(x=>x.Date).FirstOrDefault(y=>y.UserTo==id && y.IdSkill==x.Id && y.Status=="Подтверждено").Date,
            }).ToList();
            return skills.Count()>0 ? Ok(skills.OrderByDescending(x=>x.Rating)) : NotFound();
        }
    }
    [HttpGet("worksuser/{id}")]
    public ActionResult worksuser(int id)
    {
        using(var context = new RecommendationContext() )
        {
            List<Experience> skills = context.Experiences.Where(x=>x.IdUser==id).ToList();
            return skills.Count()>0 ? Ok(skills) : NotFound();
        }
    }
    [HttpPut("skills")]
    public ActionResult edit(Skill e)
    {
        using(var context = new RecommendationContext() )
        {
            Skill current = context.Skills.FirstOrDefault(x=>x.IdSkill == e.IdSkill);
            current = e;
            context.SaveChanges();
            return Ok(current);
        }
    }
    [HttpDelete("skills")]
    public ActionResult delete(Skill e)
    {
        using(var context = new RecommendationContext() )
        {
            context.Skills.Remove(e);
            context.SaveChanges();
            return Ok(e);
        }
    }
    
}
