// Para el datatable de Pedidos
document.addEventListener('DOMContentLoaded', function () {
    const datatablesPedido = document.getElementById('datatablesPedido');
    if (datatablesPedido) {
        new simpleDatatables.DataTable(datatablesPedido, {
            perPage: 10,
            labels: {
                placeholder: "Buscar pedido...",
                noRows: "No se encontraron pedidos",
                info: "Mostrando {start} a {end} de {rows} pedidos",
            },
            columns: [
                { select: 0, sort: "asc" }, // Número de Pedido
                { select: 4, type: "date", format: "DD/MM/YYYY" } // Fecha del Pedido
            ]
        });
    }
});

