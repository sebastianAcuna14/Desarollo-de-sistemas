// Para el datatable del catálogo de productos
document.addEventListener('DOMContentLoaded', function () {
    const datatableCatalogo = document.getElementById('datatablesSimple');
    if (datatableCatalogo) {
        new simpleDatatables.DataTable(datatableCatalogo, {
            perPage: 9,
            labels: {
                placeholder: "Buscar producto...",
                noRows: "No se encontraron productos",
                info: "Mostrando {start} a {end} de {rows} productos"
            }
        });
    }
});
