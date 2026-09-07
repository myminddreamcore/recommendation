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
namespace Recommendations.Controllers;

[ApiController]
[Route("[controller]")]
public class RatingController : ControllerBase
{
 
    [Authorize]
    [HttpGet("/users/{id}/rating")]
    public ActionResult rating(int id)
    {
        using(var context = new RecommendationContext() )
        {
            Rating r = context.Ratings.OrderByDescending(x=>x.DateRating).FirstOrDefault(x=>x.UserId==id);
            if(r==null){
                return NotFound();
            }
            return Ok(r);
        }
    }
    [HttpPost("/users/{id}/ratingskills")]
    public ActionResult ratingskills(int id)
    {
        using(var context = new RecommendationContext() )
        {
            double? r = context.Ratings.Where(x=>x.UserId==id && x.IdSkill!=null).Sum(x=>x.LevelRating) / context.Ratings.Where(x=>x.UserId==id && x.IdSkill!=null).Count();
           
            return Ok(r);
        }
    }
    [HttpPost("/users/me/rating/history/{id}/{date}")]
    public ActionResult history(int id,string date)
    {
        using(var context = new RecommendationContext() )
        {
            List<Rating> r = context.Ratings.AsEnumerable().Where(x=>x.UserId==id && (DateTime.Now - x.DateRating).TotalDays <= Convert.ToInt32(date)).ToList();
            if(r.Count==0){
                return NotFound();
            }
            return Ok(r);
        }
    }
     [HttpGet("/skills/top")]
    public ActionResult top()
    {
        using(var context = new RecommendationContext() )
        {
            List<ratingDTO> r = context.Ratings.OrderByDescending(x=>x.LevelRating).Include(x=>x.IdSkillNavigation).Where(x=>x.TypeRating=="Компетенция").Take(5).Select(x=>new ratingDTO{
                r =x ?? null,
                Name = x.IdSkillNavigation.NameSkill ?? ""
            }).ToList();
            if(r.Count==0){
                return NotFound();
            }
            return Ok(r);
        }
    }
    [HttpGet("/dashbord/summary/{id}")]
    public ActionResult summary(int id)
    {
        using(var context = new RecommendationContext() )
        {
            Dashbord d = new Dashbord(){
                ColProfils = context.Users.ToList().Count(),
                ProfilsPlus = context.Users.Where(x=>x.DateCreate == DateTime.Today).ToList().Count(),
                Vacancy = context.Ratings.OrderByDescending(x=>x.DateRating).FirstOrDefault(x=>x.UserId==id).LevelRating,
                VacancyProzent = context.Ratings
                    .Where(x => x.UserId == id)
                    .OrderByDescending(x => x.DateRating)
                    .Skip(1) 
                    .FirstOrDefault()?.LevelRating,
                Rating = context.Ratings.OrderByDescending(x=>x.DateRating).FirstOrDefault(x=>x.UserId==id).LevelRating


            };
            return Ok(d);
        }
    }
   
    
    
}
