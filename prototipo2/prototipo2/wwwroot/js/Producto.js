//Para el datatable de productos
document.addEventListener('DOMContentLoaded', function () {
    const datatablesSimple = document.getElementById('datatablesSimple');
    if (datatablesSimple) {
        new simpleDatatables.DataTable(datatablesSimple, {
            perPage: 10,
            labels: {
                placeholder: "Buscar Empleado...",
                noRows: "No se encontraron Empleados",
                info: "Mostrando {start} a {end} de {rows} empleados",
            }
            // No definir columnas si no hay columnas especiales como fechas
        });
    }
});
