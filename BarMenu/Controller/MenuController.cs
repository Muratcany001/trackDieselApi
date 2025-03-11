using BarMenu.Abstract;
using BarMenu.Entities.AppEntities;
using Microsoft.AspNetCore.Mvc;

namespace BarMenu.Controller
{
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly ICarRepository _menuRepository;
        public MenuController(ICarRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        [HttpPost("menus/AddMenu")]
        public ActionResult<Car> AddMenu(Car menu)
        {
            {
                if (menu == null)
                {
                    return BadRequest("Geçersiz menü verisi");
                }
                var AddMenu = _menuRepository.AddMenu(menu);
                return CreatedAtAction(nameof(AddMenu), AddMenu);
            }
        }
        [HttpGet("menus/GetAll")]
        public ActionResult<Car>  GetAllMenu() {

            var users = _menuRepository.GetAllMenus();
            return Ok(users);
        }
        [HttpGet("menus/GetMenuByCategory/{category}")]
        public ActionResult<Car> GetMenuByCategory(string category) { 
            var MenuCategory = _menuRepository.GetMenusByCategory(category);
            if (MenuCategory == null)
            {
                return NotFound("Kategori bulunamadı");
            }
            return Ok(MenuCategory);
        }
        [HttpPatch("menus/UpdateMenu}")]
        public ActionResult<Car> UpdateMenu(int id, [FromBody] Car updateMenu) { 
            var existingMenu = _menuRepository.GetMenuById(id);
            if (existingMenu == null)
            {
                return NotFound("Menü bulunamadı");
            }
            existingMenu.Title = updateMenu.Title ?? existingMenu.Title; // Name varsa güncellenir
            existingMenu.Description = updateMenu.Description ?? existingMenu.Description; // Description varsa güncellenir
            existingMenu.Ingeredents = updateMenu.Ingeredents ?? existingMenu.Ingeredents;
            existingMenu.Category = existingMenu.Category ?? existingMenu.Category;

            _menuRepository.UpdateMenu(existingMenu);
            return Ok(existingMenu);
        }
        [HttpDelete("menus/DeleteMenu/{id}")]
        public ActionResult DeleteMenu(int id)
        {
            var selectedMenu = _menuRepository.GetMenuById(id);
            if (selectedMenu == null)
            {
                return NotFound("Menü bulunamadı");
            }
            _menuRepository.DeleteMenu(selectedMenu);
            return NoContent();
        }
    }
}
