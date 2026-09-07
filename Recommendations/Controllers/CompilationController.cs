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
public class CompilationController : ControllerBase
{
 

    [HttpGet("GetAll")]
    public ActionResult GetAll()
    {
        using(var context = new RecommendationContext() )
        {
            List<string> all = context.Compilations.Select(x=>x.NameCompilation).ToList();
            return all.Count()>0 ? Ok(all) : NotFound();
        }
    }
    [HttpPut("Rename/{id}/{name}")]
    public ActionResult Rename(int id, string name)
    {
        using(var context = new RecommendationContext() )
        {
            Compilation all = context.Compilations.FirstOrDefault(x=>x.IdCompilation==id);
            all.NameCompilation=name;
            context.SaveChanges();
            return all!=null ? Ok(all) : NotFound();
        }
    }
     [HttpPut("Delete/{id}")]
    public ActionResult Delete(int id)
    {
        using(var context = new RecommendationContext() )
        {
            Compilation all = context.Compilations.FirstOrDefault(x=>x.IdCompilation==id);
            List<UserCompilation> a = context.UserCompilations.Where(x=>x.IdCompilation==id).ToList();
            context.UserCompilations.RemoveRange(a);
            context.Compilations.Remove(all);
            context.SaveChanges();
            return all!=null ? Ok(all) : NotFound();
        }
    }
    [HttpPost("AddNew/{id}/{name}")]
    public ActionResult GetAll(int id, string name)
    {
        using(var context = new RecommendationContext() )
        {
            Compilation all = context.Compilations.FirstOrDefault(x=>x.NameCompilation==name);
            UserCompilation current = context.UserCompilations.FirstOrDefault(x=>x.IdUser == id && x.IdCompilation == all.IdCompilation);
            if(current!=null){
                return BadRequest("");
            }
            UserCompilation a = new UserCompilation(){
                IdUserCompilation = context.UserCompilations.Any() ? context.UserCompilations.Max(x=>x.IdUserCompilation)+1:1,
                IdUser = id,
                IdCompilation = all.IdCompilation
            };
            context.UserCompilations.Add(a);
            context.SaveChanges();
            return Ok(a) ;
        }
    }
    [HttpGet("GetPodborka")]
    public ActionResult GetPodborka()
    {
        using(var context = new RecommendationContext() )
        {
            List<Compilation> all = context.Compilations.ToList();
            List<PodborDTO> itog = new List<PodborDTO>();
            foreach(var item in all)
            {
                List<FinderrDTO> itog2 = new  List<FinderrDTO>();
                List<UserCompilation> comp = context.UserCompilations.Where(x=>x.IdCompilation==item.IdCompilation).ToList();
                foreach(var item2 in comp)
                {
                    User y = context.Users.FirstOrDefault(x=>x.IdUser == item2.IdUser);
                     FinderrDTO i = new FinderrDTO()
                    {
                        Id = y.IdUser,
                        Name = y.NameUser,
                        Skills = "",
                        Rating = context.Ratings.OrderByDescending(x=>x.DateRating).FirstOrDefault(x=>x.UserId==y.IdUser).LevelRating
                    };
                    itog2.Add(i);
                }
                PodborDTO d = new PodborDTO(){
                    Id = item.IdCompilation,
                    Name = item.NameCompilation,
                    Date = item.Date,
                    users = itog2
                };
                itog.Add(d);
            }
            return Ok(itog) ;
        }
    }
  
    
    
}
