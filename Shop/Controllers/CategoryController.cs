using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Controller{

    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices]DataContext context
        ){
            //Quando se faz um select do banco se obtem um proxy da classe de context
            //obtem informações adicionais, que servem para casos igual do delete
            //No caso do get, que deseja somente listar os dados é uma boa pratica usar o AsNoTracking
            //Isso desabilita a captura de dados adicionais pelo EF
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices]DataContext context
            )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(category==null)
                return NotFound(new {message = "Categoria não encontrada"});

            try{
                return Ok(category);
            }catch{
                return BadRequest(new {message="Não foi possível deletar a categoria"});
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post(
            [FromBody]Category model,
            [FromServices]DataContext context
            )
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try{
                context.Categories.Add(model);
                await context.SaveChangesAsync();
            
                return Ok(model);
            }catch
            {
                return BadRequest(new {message="Não foi possível criar a categoria"});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(
            int id, 
            [FromBody]Category model,
            [FromServices]DataContext context
            )
        {
            if(model.Id != id)
                return NotFound(new {message = "Categoria não encontrada"});    

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try{
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
            
                return Ok(model);
            }
            catch
            {
                return BadRequest(new {message="Não foi possível atualizar a categoria"});
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<string>> Delete(
            int id,
            [FromServices]DataContext context
            )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(category==null)
                return NotFound(new {message = "Categoria não encontrada"});

            try{
                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new {message="Categoria removida"});
            }catch{
                return BadRequest(new {message="Não foi possível deletar a categoria"});
            }
        }
    }
}