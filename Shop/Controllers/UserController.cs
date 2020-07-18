using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles="manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();
            return users;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Post(
            [FromBody]User model,
            [FromServices]DataContext context
            )
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try{
                //Forces user aways be an employee
                model.Role = "employee";
                
                context.Users.Add(model);
                await context.SaveChangesAsync();
            
                //Don't send password to screen
                model.Password="";

                return Ok(model);
            }catch
            {
                return BadRequest(new {message="Não foi possível criar o usuario"});
            }
        }

        [HttpPut]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            int id,
            [FromBody] User model,
            [FromServices] DataContext context
        ){
            if(model.Id != id)
                return NotFound(new {message = "Usuario não encontrado"});    

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try{
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
            
                //Don't send password to screen
                model.Password="";

                return Ok(model);
            }
            catch
            {
                return BadRequest(new {message="Não foi possível atualizar o Usuario"});
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromBody]User model,
            [FromServices]DataContext context
            )
        {
            var user = await context.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Username == model.Username && x.Password == model.Password);
            
            if(user == null)
                return NotFound(new {message = "Usuário ou senha inválidos"});
            
            var token = TokenService.GenerateToken(user);

            //Don't send password to screen
            user.Password="";

            return new{
                user = user,
                token = token
            };
        }
    }
}