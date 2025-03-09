using BarMenu.Abstract;
using BarMenu.Entities;
using BarMenu.Entities.AppEntities;
using System.Linq;

namespace BarMenu.Concrete
{
    public class MenuRepository : IMenuRepository
    {
        private readonly Context _context;
        public MenuRepository(Context context) { 
        _context = context;
        }

       public Menu AddMenu(Menu menu)
        {
            _context.Menus.Add(menu);
            _context.SaveChanges();
            return menu;
        }
        public List<Menu> GetAllMenus()
        {
            return _context.Menus.ToList();
        }
        public Menu GetMenuById(int id)
        {
            var menu = _context.Menus.FirstOrDefault(m => m.Id == id);
            return menu;

        }
        public Menu UpdateMenu(Menu menu)
        {
            _context.Menus.Update(menu);
            _context.SaveChanges();
            return menu;
        }
        public Menu DeleteMenu(int id)
        {
            var menu = _context.Menus.FirstOrDefault(x => x.Id == id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
                _context.SaveChanges(true);
            }
            return menu;
        }
        public List<Menu> GetMenusByCategory(string category)
        {
            var menus = _context.Menus.Where(x => x.Category == category).ToList();
            return menus;
        } 
    }
}
