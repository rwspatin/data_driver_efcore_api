using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
namespace Shop.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
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
            var products = await context
                                    .Products
                                    .Include(x => x.Category)
                                    .AsNoTracking()
                                    .ToListAsync();
            
            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices]DataContext context
            )
        {
            var category = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if(category==null)
                return NotFound(new {message = "Produto não encontrada"});

            try{
                return Ok(category);
            }catch{
                return BadRequest(new {message="Não foi possível encontrar o Produto"});
            }
        }


        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetByCategory(
            int id,
            [FromServices]DataContext context
            )
        {
            var category = await context
                            .Products
                            .Include(x => x.Category)
                            .AsNoTracking()
                            .Where(x => x.CategoryId == id)
                            .ToListAsync();
            if(category==null)
                return NotFound(new {message = "Produto não encontrado"});

            try{
                return Ok(category);
            }catch{
                return BadRequest(new {message="Não foi possível encontrar o Produto"});
            }
        }
        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post(
            [FromBody]Product model,
            [FromServices]DataContext context
            )
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try{
                context.Products.Add(model);
                await context.SaveChangesAsync();
            
                return Ok(model);
            }catch
            {
                return BadRequest(new {message="Não foi possível criar o Produto"});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(
            int id, 
            [FromBody]Product model,
            [FromServices]DataContext context
            )
        {
            if(model.Id != id)
                return NotFound(new {message = "Produto não encontrado"});    

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try{
                context.Entry<Product>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
            
                return Ok(model);
            }
            catch
            {
                return BadRequest(new {message="Não foi possível atualizar o Produto"});
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
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if(product==null)
                return NotFound(new {message = "Produto não encontrado"});

            try{
                context.Products.Remove(product);
                await context.SaveChangesAsync();

                return Ok(new {message="Produto removido"});
            }catch{
                return BadRequest(new {message="Não foi possível deletar o Produto"});
            }
        }
    }
}