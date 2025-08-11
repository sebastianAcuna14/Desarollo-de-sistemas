document.addEventListener('DOMContentLoaded', function () {
    const tablaVentas = document.getElementById('tablaVentas');
    if (tablaVentas) {
        new simpleDatatables.DataTable(tablaVentas, {
            perPage: 10,
            labels: {
                placeholder: "Buscar ventas...",
                noRows: "No se encontraron ventas",
                info: "Mostrando {start} a {end} de {rows} ventas",
            },
            columns: [
                { select: 0, sort: "desc" },
                { select: 1, type: "date", format: "DD/MM/YYYY" }, 
                { select: 4, type: "number" } 
            ],
   
            labels: {
                placeholder: "Buscar...",
                noRows: "No se encontraron ventas",
                info: "Mostrando {start} a {end} de {rows} ventas (Página {page} de {pages})",
                loading: "Cargando...",
                infoFiltered: "(filtrado de {rows} ventas totales)",
                search: "Buscar",
                noResults: "No hay resultados que coincidan con la búsqueda"
            }
        });
    }
});