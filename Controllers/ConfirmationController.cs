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
public class ConfirmationController : ControllerBase
{
 

    [HttpPost("confirmations/request")]
    public ActionResult request(Confirmation e)
    {
        using(var context = new RecommendationContext() )
        {
            e.IdConfirmation = context.Confirmations.Any() ? context.Confirmations.Max(x=>x.IdConfirmation) +1 : 1;
            e.Status = "Запрошено";
            context.Confirmations.Add(e);
            context.SaveChanges();
            return Ok(e);
        }
    }
    [HttpGet("user/{id}/confirmations")]
    public ActionResult request(int id)
    {
        using(var context = new RecommendationContext() )
        {
           List<Confirmation> e = context.Confirmations.Where(x=>x.UserTo==id && x.Status=="Подтверждено").ToList();
            return e.Count()>0 ? Ok(e): NotFound();
        }
    }
    [HttpPut("confirmations/request/{id}/accept")]
    public ActionResult accept(int id)
    {
        using(var context = new RecommendationContext() )
        {
            Confirmation e = context.Confirmations.FirstOrDefault(x=>x.IdConfirmation==id);
            e.Status = "Подтверждено";
            
            context.SaveChanges();
            return Ok(e);
        }
    }
    [HttpPut("confirmations/request/{id}/ot")]
    public ActionResult ot(int id)
    {
        using(var context = new RecommendationContext() )
        {
            Confirmation e = context.Confirmations.FirstOrDefault(x=>x.IdConfirmation==id);
            e.Status = "Отозвано";
            
            context.SaveChanges();
            return Ok(e);
        }
    }
     [HttpPut("confirmations/request/{id}/reject")]
    public ActionResult reject(int id)
    {
        using(var context = new RecommendationContext() )
        {
            Confirmation e = context.Confirmations.FirstOrDefault(x=>x.IdConfirmation==id);
            e.Status = "Отклонено";
            
            context.SaveChanges();
            return Ok(e);
        }
    }
    [HttpGet("confirmations")]
    public ActionResult confirmations()
    {
        using(var context = new RecommendationContext() )
        {
            List<Confirmation> e = context.Confirmations.Where(x=>x.Status=="Запрошено" || x.Status=="Подтверждено").ToList();
           
            
            return e.Count()>0 ? Ok(e) : NotFound("не найдено");
        }
    }
    
    
}
