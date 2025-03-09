using BarMenu.Entities.AppEntities;

namespace BarMenu.Abstract
{
    public interface IMenuRepository
    {
        Menu AddMenu (Menu menu);
        Menu UpdateMenu (Menu menu);
        Menu DeleteMenu (Menu menu);
        List<Menu> GetAllMenus ();
        Menu GetMenuById (int id);
        List<Menu> GetMenusByCategory (string category);
    }
}
