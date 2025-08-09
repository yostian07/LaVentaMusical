using System.Web;
using LaVentaMusical.Models.ViewModels;

namespace LaVentaMusical.Helpers
{
    public static class CarritoHelper
    {
        private const string KEY = "CARRITO";

        public static CarritoVM Get(HttpSessionStateBase session)
        {
            var c = session[KEY] as CarritoVM;
            if (c == null) { c = new CarritoVM(); session[KEY] = c; }
            return c;
        }

        public static void Clear(HttpSessionStateBase session) => session[KEY] = new CarritoVM();
    }
}
