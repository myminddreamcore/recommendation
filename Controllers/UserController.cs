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
public class UserController : ControllerBase
{
 

    [Authorize]
    [HttpGet("users/me/{id}")]
    public ActionResult Get(int id)
    {
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
       
        if (userIdFromToken != id.ToString())
        {
            return Unauthorized("Нельзя смотреть чужой профиль через /me");
        }
        
        using(var context = new RecommendationContext())
        {
            User s = context.Users.FirstOrDefault(x=>x.IdUser == id);
            if(s==null)
            {
                return NotFound("");
            }
            return Ok(s);
        }
    }
 [HttpPost("users/find")]
public ActionResult Get(Find f)
{
    using (var context = new RecommendationContext())
    {
        var allUsers = context.Users.ToList();
        var result = new List<object>();

        foreach (var user in allUsers)
        {
            var userSkills = context.UserSkills
                .Include(x => x.IdSkillNavigation)
                .Where(x => x.UserId == user.IdUser)
                .ToList();

            var userRating = context.Ratings
                .OrderByDescending(x => x.DateRating)
                .FirstOrDefault(x => x.UserId == user.IdUser);

            double ratingValue = userRating?.LevelRating ?? 0;

            bool shouldInclude = true;

            if (f.RatingOt != null && ratingValue < f.RatingOt)
                shouldInclude = false;

            if (f.ratingDo != null && ratingValue > f.ratingDo)
                shouldInclude = false;

            if (f.skills != null && f.skills.Any() && shouldInclude)
            {
                bool hasMatchingSkill = false;

                foreach (var searchSkill in f.skills)
                {
                    var matchingSkill = userSkills
                        .FirstOrDefault(x => x.IdSkillNavigation != null && 
                            x.IdSkillNavigation.NameSkill != null &&
                            x.IdSkillNavigation.NameSkill.Equals(searchSkill, StringComparison.OrdinalIgnoreCase));

                    if (matchingSkill != null)
                    {
                        bool levelOk = true;

                        if (f.UrovenOt != null && matchingSkill.MarkSkill < f.UrovenOt)
                            levelOk = false;

                        if (f.UrovenODo != null && matchingSkill.MarkSkill > f.UrovenODo)
                            levelOk = false;

                        if (levelOk)
                        {
                            hasMatchingSkill = true;
                            break;
                        }
                    }
                }

                if (!hasMatchingSkill)
                    shouldInclude = false;
            }

            if (shouldInclude)
            {
                var skillNames = userSkills
                    .Where(x => x.IdSkillNavigation != null && x.IdSkillNavigation.NameSkill != null)
                    .Select(x => x.IdSkillNavigation.NameSkill)
                    .ToList();

                result.Add(new
                {
                    Id = user.IdUser,
                    Name = $"{user.SurnameUser} {user.NameUser} {user.PatronymicUser}".Trim(),
                    Skills = string.Join(", ", skillNames),
                    Rating = ratingValue
                });
            }
        }

        return result.Any() ? Ok(result) : NotFound();
    }
}
    [HttpPost("auth/login/{email}/{password}")]
    public ActionResult auth(string email, string password)
    {
        using(var context = new RecommendationContext())
        {
            User s = context.Users.FirstOrDefault(x=>x.EmailUser == email && x.PasswordUser==password);
            if(s==null)
            {
                return NotFound("");
            }
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, s.IdUser.ToString())
            };
            var jwt = new JwtSecurityToken(
                issuer: Source.JWT.issuer,
                audience: Source.JWT.audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(Source.JWT.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            var tokenn = EncryptSimple(token);
            System.IO.File.WriteAllText("auth.json", $"{{\"token\":\"{tokenn}\"}}");
            
            return StatusCode(201, new UserDTO{
                user = s,
                token = token
            });
        }
    }
    [HttpPost("auth/mobile/{password}")]
    public ActionResult mobile(string password)
    {
        using(var context = new RecommendationContext())
        {
            User s = context.Users.FirstOrDefault(x=>x.Pin==password);
            if(s==null)
            {
                return NotFound("");
            }
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, s.IdUser.ToString())
            };
            var jwt = new JwtSecurityToken(
                issuer: Source.JWT.issuer,
                audience: Source.JWT.audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(Source.JWT.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            var tokenn = EncryptSimple(token);
            System.IO.File.WriteAllText("auth.json", $"{{\"token\":\"{tokenn}\"}}");
            
            return Ok(new UserDTO{
                user = s,
                token = tokenn
            });
        }
    }
    [HttpPost("auth/register")]
    public ActionResult auth(User s)
    {
        using(var context = new RecommendationContext())
        {
            User current = context.Users.FirstOrDefault(x=>x.EmailUser == s.EmailUser);
            if(current!=null)
            {
                return BadRequest("");
            }
            s.IdUser = context.Users.Any() ? context.Users.Max(x=>x.IdUser)+1:1;
           context.Users.Add(s);
           context.SaveChanges();
           return Ok(s);
        }
    }
     [HttpPost("auth/forgot/{email}")]
    public ActionResult forgot(string email)
    {
        using(var context = new RecommendationContext())
        {
            User current = context.Users.FirstOrDefault(x=>x.EmailUser == email);
            if(current==null)
            {
                return NotFound("");
            }
            
           return Ok(current.PasswordUser);
        }
    }
        // Шифрование 
    [ApiExplorerSettings(IgnoreApi = true)] 
    public string EncryptSimple(string text) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));

    // Расшифровка
    [ApiExplorerSettings(IgnoreApi = true)] 
    public string DecryptSimple(string encodedText) => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
   

    
    [Authorize]
    [HttpPost("auth/logout")]
    public ActionResult logout()
    {
        if(System.IO.File.Exists("auth.json"))
        {
            System.IO.File.Delete("auth.json");
        }
        return Ok("Выход выполнен");
    }
    
    [Authorize]
    [HttpPost("auth/refresh")]
    public ActionResult refresh()
    {
        var us = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        using(var context = new RecommendationContext())
        {
            User s = context.Users.FirstOrDefault(x=>x.IdUser == int.Parse(us));
            if(s==null)
            {
                return NotFound("");
            }
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, s.IdUser.ToString())
            };
            var jwt = new JwtSecurityToken(
                issuer: Source.JWT.issuer,
                audience: Source.JWT.audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(Source.JWT.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            var tokenn = EncryptSimple(token);
            System.IO.File.WriteAllText("auth.json", $"{{\"token\":\"{tokenn}\"}}");
            
            return Ok(tokenn);
        }
    }
    
    [Authorize]
    [HttpPut("users/me")]
    public ActionResult edit(User current)
    {
        var userId = int.Parse((User.FindFirst(ClaimTypes.NameIdentifier)?.Value));

        current.IdUser = userId;
        
        using(var context = new RecommendationContext())
        {
            User s = context.Users.FirstOrDefault(x=>x.IdUser == userId);
            if(s==null)
            {
                return NotFound("");
            }
            
            s.NameUser = current.NameUser ?? s.NameUser;
            s.SurnameUser = current.SurnameUser ?? s.SurnameUser;
            s.PatronymicUser = current.PatronymicUser ?? s.PatronymicUser;
            s.PhoneUser = current.PhoneUser ?? s.PhoneUser;
            s.EmailUser = current.EmailUser ?? s.EmailUser;
            
            context.SaveChanges();
            return Ok(s);
        }
    }
    [Authorize]
    [HttpPost("users/me/education")]
    public ActionResult education(Education ed)
    {
        var userId = int.Parse((User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        
        
        using(var context = new RecommendationContext())
        {
            ed.IdEducation = context.Educations.Any() ? context.Educations.Max(x=>x.IdEducation)+1 : 1;
            context.Educations.Add(ed);
            context.SaveChanges();
            return Ok(ed);
        }
    }
    [Authorize]
    [HttpDelete("users/me/education/{id}")]
    public ActionResult educationdel(int id)
    {
        var userId = int.Parse((User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        
        
        using(var context = new RecommendationContext())
        {
            Education d = context.Educations.FirstOrDefault(x=>x.IdEducation==id);
            context.Educations.Remove(d);
            context.SaveChanges();
            return Ok(d);
        }
    }
    [Authorize]
    [HttpPost("users/me/experience")]
    public ActionResult experience(Experience ed)
    {
        var userId = int.Parse((User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        
        
        using(var context = new RecommendationContext())
        {
            ed.Id = context.Experiences.Any() ? context.Experiences.Max(x=>x.Id)+1 : 1;
            context.Experiences.Add(ed);
            context.SaveChanges();
            return Ok(ed);
        }
    }
    [Authorize]
    [HttpDelete("users/me/experience/{id}")]
    public ActionResult experiencedel(int id)
    {
        var userId = int.Parse((User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        
        
        using(var context = new RecommendationContext())
        {
            Experience d = context.Experiences.FirstOrDefault(x=>x.Id==id);
            context.Experiences.Remove(d);
            context.SaveChanges();
            return Ok(d);
        }
    }
    [Authorize]
    [HttpPost("users/me/skills")]
    public ActionResult skills(UserSkill ed)
    {
        var userId = int.Parse((User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        
        
        using(var context = new RecommendationContext())
        {
            ed.Id = context.UserSkills.Any() ? context.UserSkills.Max(x=>x.Id)+1 : 1;
            context.UserSkills.Add(ed);
            context.SaveChanges();
            return Ok(ed);
        }
    }
    [Authorize]
    [HttpDelete("users/me/skills/{id}")]
    public ActionResult skillsdel(int id)
    {
        var userId = int.Parse((User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        
        
        using(var context = new RecommendationContext())
        {
            UserSkill d = context.UserSkills.FirstOrDefault(x=>x.Id==id);
            context.UserSkills.Remove(d);
            context.SaveChanges();
            return Ok(d);
        }
    }
    [Authorize]
    [HttpPut("users/me/skills/{id}")]
    public ActionResult skillsup(UserSkill s)
    {
        var userId = int.Parse((User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        
        
        using(var context = new RecommendationContext())
        {
            UserSkill d = context.UserSkills.FirstOrDefault(x=>x.Id==s.Id);
            d.MarkSkill = s.MarkSkill;
            context.SaveChanges();
            return Ok(d);
        }
    }
    [HttpGet("users/{id}/confirmations/incoming")]
    public ActionResult skillsup(int id)
    {
        using(var context = new RecommendationContext())
        {
            List<ConfirmationDTO> list = context.Confirmations.Include(x=>x.UserFromNavigation).Include(x=>x.IdSkillNavigation).Where(x=>x.UserTo==id).Select(x=>new ConfirmationDTO{
                c= x,
                NameUser = x.UserFromNavigation.SurnameUser,
                NameSkill = x.IdSkillNavigation.NameSkill
            }).ToList();
            return list.Count()>0 ? Ok(list) : NotFound("");
        }
    }
    [HttpGet("users/{id}/confirmations/outgoing")]
    public ActionResult outgoing(int id)
    {
        using(var context = new RecommendationContext())
        {
            List<ConfirmationDTO> list = context.Confirmations.Include(x=>x.UserFromNavigation).Include(x=>x.IdSkillNavigation).Where(x=>x.UserFrom==id).Select(x=>new ConfirmationDTO{
                c= x,
                NameUser = x.UserFromNavigation.SurnameUser,
                NameSkill = x.IdSkillNavigation.NameSkill
            }).ToList();
            return list.Count()>0 ? Ok(list) : NotFound("");
        }
    }
    [Authorize]
    [HttpGet("users/{id}")]
    public ActionResult anon(int id, [FromQuery] string mode = null)
    {
        using(var context = new RecommendationContext())
        {
            if (mode == "employer")
            {
                User s = context.Users.FirstOrDefault(x=>x.IdUser == id);
                if(s==null)
                {
                    return NotFound("");
                }
                var anonUser = new User
                {
                    IdUser = s.IdUser,
                    NameUser = "И",
                    SurnameUser = "Фамилия",
                    PatronymicUser = "И",
                    PhoneUser = "скрыто",
                    EmailUser = "скрыто",
                    PasswordUser = "скрыто"
                };
                return Ok(anonUser);
            }
            else
            {
                User s = context.Users.FirstOrDefault(x=>x.IdUser == id);
                if(s==null)
                {
                    return NotFound("");
                }
                return Ok(s);
            }
        }
    }
}
