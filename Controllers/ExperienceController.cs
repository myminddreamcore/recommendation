using Microsoft.AspNetCore.Mvc;
using Recommendations.Models;
using Recommendations.Source;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Recommendations.Source;
namespace Recommendations.Controllers;

[ApiController]
[Route("[controller]")]
public class ExperienceController : ControllerBase
{
 

    [HttpPost("experience")]
    public ActionResult Get(Experience e)
    {
        using(var context = new RecommendationContext() )
        {
            e.Id = context.Experiences.Any() ? context.Experiences.Max(x=>x.Id) +1 : 1;
            context.Experiences.Add(e);
            context.SaveChanges();
            return Ok(e);
        }
    }
    [HttpPut("experience")]
    public ActionResult edit(Experience e)
    {
        using(var context = new RecommendationContext() )
        {
            Experience current = context.Experiences.FirstOrDefault(x=>x.Id == e.Id);
            current = e;
            context.SaveChanges();
            return Ok(current);
        }
    }
    [HttpDelete("experience")]
    public ActionResult delete(Experience e)
    {
        using(var context = new RecommendationContext() )
        {
            context.Experiences.Remove(e);
            context.SaveChanges();
            return Ok(e);
        }
    }
    
}
