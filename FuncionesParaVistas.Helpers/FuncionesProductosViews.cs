namespace POS.FuncionesParaVistas.Helpers
{
    public static class FuncionesProductosViews
    {
        public static string DevolverPesoSegunCategoria(string categoria)
        {
            if(categoria == "Bebidas c/a" || categoria == "Bebidas s/a")
            {
                return "ml ";
            }
            else
            {
                return "gr ";
            }
        }
    }
}
