namespace GGS.Scripts
{
    /// <summary>
    /// Ofrece utilidades al momento de actualizar registros con el EntityFramework
    /// para satisfacer las necesidades de la lógica de negocios
    /// </summary>
    public static class Moon
    {
        /// <summary>
        /// Recibe dos arreglos de elementos, uno de los elementos antes del cambio, y otro de los elementos después del cambio,
        /// regresa una tupla, el primer elemento es un arreglo de los elementos que deben ser borrados, es decir, estaban en los elementos
        /// antes del cambio, pero no en los elementos después de este. El segundo elemento es otro arreglo de los elementos que deben ser
        /// creador, es decir, elementos que no estaban antes del cambio, pero sí después.
        /// 
        /// Por ejemplo, si antes los ids asignados a un material son [1,2,3] y después son [3,4,5], se regresará una tupla, donde el primer
        /// elemento será [1,2], los ids a eliminar, y el segundo elemento será [4,5], los ids a crear.
        /// </summary>        
        public static (IEnumerable<T> itemsToDelete, IEnumerable<T> itemsToInsert) GetElementsToDeleteAndInsert<T>(IEnumerable<T> itemsBefore, IEnumerable<T> itemsAfter)
        {
            var itemsToDelete = itemsBefore.Except(itemsAfter);
            var itemsToInsert = itemsAfter.Except(itemsBefore);

            return (itemsToDelete, itemsToInsert);
        }

        /// <summary>
        /// Recibe un arreglo con los probables cambios, que puede haber, y compara los valores de antes y despues, y regresa un arreglo
        /// con los cambios que realmente se hicieron.
        /// 
        /// Por ejemplo, si ingreso un arreglo como [["Nombre", "Dariana", "Dariana"], ["Edad", "20", "21"], ["Creditos", "5", "4"]], se regresará
        /// un arreglo con los cambios que realmente se hicieron, es decir: [["Edad", "20", "21"], ["Creditos", "5", "4"]].
        /// </summary>
        public static IEnumerable<(string field, string valueBefore, string valueAfter)> GetChanges(IEnumerable<(string field, string valueBefore, string valueAfter)> changes)
        {
            var realChanges = new List<(string field, string valueBefore, string valueAfter)>();
            foreach(var change in changes) 
            {
                if (change.valueBefore != change.valueAfter) realChanges.Add((change.field, change.valueBefore, change.valueAfter));
            }

            return realChanges;
        }
    }
}
